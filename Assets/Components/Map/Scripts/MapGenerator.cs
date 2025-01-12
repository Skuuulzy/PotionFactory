using Components.Bundle;
using Components.Island;
using Components.Recipes;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Map
{

	public class MapGenerator : MonoBehaviour
	{
		[SerializeField] private GameObject _mapGameObject;
		[SerializeField] private List<RectTransform> _islandsParents;
		[SerializeField] private RectTransform _nodeLineParent;
		[SerializeField] private GameObject _linePrefab;
		[SerializeField] private Button _confirmButton;
		private List<UIIslandController> _islandsControllers = new List<UIIslandController>();
		private LevelNode _selectedNode;
		private LevelNode _startingSelectedNode;
		private List<LevelNode> _alreadyConnectedLevelNodes;
		private bool _isFirstGameChoice;

		private List<IngredientsBundle> _startingGameIngredientsBundles;
		private List<IngredientsBundle> _startingRoundIngredientsBundles;
		private List<IslandTemplate> _allIslandTemplate;

		//Need to change this poor bool 
		public static Action<IngredientsBundle, bool> OnMapChoiceConfirm;

		//------------------------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------------------
		private void Awake()
		{
			LevelNode.OnNodeSelected += HandleNodeSelected;
			MapState.OnMapStateStarted += Init;

			//Set up ingredients bundles list


			_mapGameObject.SetActive(true);
			_confirmButton.interactable = false;
		}
		private void OnDestroy()
		{
			LevelNode.OnNodeSelected -= HandleNodeSelected;
			MapState.OnMapStateStarted -= Init;
		}

		//------------------------------------------------------------------------------------------- INITIALISATION -------------------------------------------------------------------------------------------
		private void Init(MapState state)
		{
			_mapGameObject.SetActive(true);

			//First map generation
			if (state.StateIndex == 1)
			{
				RegenerateMap();
			}
			else
			{
				_isFirstGameChoice = false;
				SelectStartingRoundNode(_startingSelectedNode);
			}
		}

		[ContextMenu("Regenerate Map")]
		public void RegenerateMap()
		{
			ClearMap();
			_isFirstGameChoice = true;
			GenerateIslands();
			ConnectIslands();
			DrawConnections();
			SetStartingGameNodes();
		}



		private void ClearMap()
		{
			for(int i = 0; i < _islandsParents.Count; i++)
			{
				foreach (Transform child in _islandsParents[i])
				{
					Destroy(child.gameObject);
				}
			}

			foreach (Transform child in _nodeLineParent)
			{
				Destroy(child.gameObject);
			}

			_islandsControllers = new List<UIIslandController>();
			_selectedNode = null;
			_startingSelectedNode = null;
			_alreadyConnectedLevelNodes = new List<LevelNode>();

			_startingGameIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => bundle.IsStartingGameBundle).ToList();
			_startingRoundIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => !bundle.IsStartingGameBundle).ToList();
			_allIslandTemplate = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IslandTemplate>().ToList();
		}

		//------------------------------------------------------------------------------------------- MAP GENERATION -------------------------------------------------------------------------------------------
		private void GenerateIslands()
		{
			_allIslandTemplate = _allIslandTemplate.OrderBy(_ => UnityEngine.Random.value).ToList();
			for (int i = 0; i < _islandsParents.Count; i++)
			{
				UIIslandController controller = Instantiate(_allIslandTemplate[i].Controller, _islandsParents[i]);
				_islandsControllers.Add(controller);
				controller.SetIslandName(_allIslandTemplate[i].Name);
			}
		}

		public void ConnectIslands()
		{
			Dictionary<LevelNode, LevelNode> confirmedConnections = new Dictionary<LevelNode, LevelNode>();

			foreach (var island in _islandsControllers)
			{
				Dictionary<LevelNode, List<LevelNode>> validConnectionByLevelNode = new Dictionary<LevelNode, List<LevelNode>>();

				// Get all valid connection for each node
				foreach (var node in island.LevelNodeList)
				{
					var validConnections = FindValidConnections(island, node);
					validConnectionByLevelNode.Add(node, validConnections);
				}

				
				foreach (var node in validConnectionByLevelNode.Keys)
				{
					foreach (var targetNode in validConnectionByLevelNode[node])
					{
						// Check if the targetNode is already connected
						if (confirmedConnections.TryGetValue(targetNode, out var existingNode))
						{
							// If new node is closer then we change the old one by it
							if (GetDistance(targetNode, node) < GetDistance(targetNode, existingNode))
							{
								confirmedConnections[targetNode] = node;
							}
						}
						else
						{
							// Not connected so we create it
							confirmedConnections[targetNode] = node;
						}
					}
				}
			}

			//Create connections
			foreach (var kvp in confirmedConnections)
			{
				if(kvp.Key.ExternalConnectedNode.Count < 1 && kvp.Value.ExternalConnectedNode.Count < 1)
				{
					kvp.Key.ConnectedNodes.Add(kvp.Value);
					kvp.Key.ExternalConnectedNode.Add(kvp.Value);
					kvp.Value.ConnectedNodes.Add(kvp.Key);
					kvp.Value.ExternalConnectedNode.Add(kvp.Key);
				}
			}
		}

		private float GetDistance(LevelNode a, LevelNode b)
		{
			
			return Vector3.Distance(a.transform.position, b.transform.position);
		}


		/// <summary>
		/// Get a list of level node which can be connected to a node
		/// </summary>
		private List<LevelNode> FindValidConnections(UIIslandController island,LevelNode node)
		{
			List<LevelNode> validConnections = new List<LevelNode>();

			foreach (var otherIsland in _islandsControllers)
			{
				if(island == otherIsland)
				{
					continue;
				}
				foreach (var otherNode in otherIsland.LevelNodeList)
				{
					//Security check but can't happen normally with the previous check
					if (node == otherNode) continue;

					if (AreNodesConnectable(node, otherNode))
					{
						validConnections.Add(otherNode);
					}
				}
			}

			return validConnections.OrderBy(n => GetDistance(node, n)).ToList();
		}

		/// <summary>
		/// Check if a node can connect to another one
		/// </summary>
		private bool AreNodesConnectable(LevelNode nodeA, LevelNode nodeB)
		{
			return (nodeA.NodeSizde == NodeSide.EAST && nodeB.NodeSizde != NodeSide.EAST && nodeB.NodeSizde != NodeSide.DEFAULT) ||
				   (nodeA.NodeSizde == NodeSide.WEST && nodeB.NodeSizde != NodeSide.WEST && nodeB.NodeSizde != NodeSide.DEFAULT) ||
				   (nodeA.NodeSizde == NodeSide.NORTH && nodeB.NodeSizde != NodeSide.NORTH && nodeB.NodeSizde != NodeSide.DEFAULT) ||
				   (nodeA.NodeSizde == NodeSide.SOUTH && nodeB.NodeSizde != NodeSide.SOUTH && nodeB.NodeSizde != NodeSide.DEFAULT)
				   ;
		}

		private void DrawConnections()
		{
			HashSet<(LevelNode, LevelNode)> drawnConnections = new HashSet<(LevelNode, LevelNode)>();

			foreach(var island in _islandsControllers)
			{
				foreach (var node in island.LevelNodeList)
				{
					foreach (var connectedNode in node.ConnectedNodes)
					{
						if (!drawnConnections.Contains((node, connectedNode)) && !drawnConnections.Contains((connectedNode, node)))
						{
							GameObject lineObj = Instantiate(_linePrefab, _nodeLineParent);
							RectTransform lineRect = lineObj.GetComponent<RectTransform>();

							Vector2 startPos = node.transform.position;
							Vector2 endPos = connectedNode.transform.position;
							Vector2 midPoint = (startPos + endPos) / 2;

							lineRect.localPosition = midPoint;

							float distance = Vector2.Distance(startPos, endPos);
							lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);

							float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
							lineRect.rotation = Quaternion.Euler(0, 0, angle);

							drawnConnections.Add((node, connectedNode));
						}
					}
				}
			}
		}

		//------------------------------------------------------------------------------------------- NODE SELECTION -------------------------------------------------------------------------------------------
		private void HandleNodeSelected(LevelNode nodeSelected)
		{
			_confirmButton.interactable = true;

			//First choice of the game
			if(_startingSelectedNode == null)
			{
				_startingSelectedNode = nodeSelected;
			}

			if (_selectedNode != null && _selectedNode != nodeSelected)
			{
				_selectedNode.UnselectNode();
			}

			_selectedNode = nodeSelected;
		}

		private void SetNodesState()
		{
			foreach(LevelNode node in _startingSelectedNode.ConnectedNodes)
			{
				node.UnlockNode();
			}

			foreach(LevelNode alreadyConnectedNode in _alreadyConnectedLevelNodes)
			{
				alreadyConnectedNode.SetNodeAsConnected();

				foreach (LevelNode node in alreadyConnectedNode.ConnectedNodes)
				{
					node.UnlockNode();
				}
			}

			_selectedNode = _startingSelectedNode;
			_startingSelectedNode.SelectNodeAsFirst();
			_confirmButton.interactable = false;
		}

		//Use for starting round only
		private void SetStartingGameNodes()
		{
			if (_islandsControllers == null || _islandsControllers.Count == 0)
			{
				Debug.LogWarning("La liste de nœuds est vide. Impossible de définir un nœud de départ.");
				return;
			}

			_startingGameIngredientsBundles = _startingGameIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			List<LevelNode> potentialStartingLevelNodes = new List<LevelNode>();

			for( int i = 0; i < _islandsControllers.Count; i++)
			{
				//Set starting bundle for Starting node of each island
				_islandsControllers[i].InitStartingBundle(_startingGameIngredientsBundles[i]);
				potentialStartingLevelNodes.Add(_islandsControllers[i].StartingLevelNode);
				//Set ingredient bundle for every node except the starting node
				_startingRoundIngredientsBundles = _startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
				_islandsControllers[i].Init(_startingRoundIngredientsBundles.Take(_islandsControllers[i].NumberOfNodes).ToArray());
			}


			SetStartingGameNodesState(potentialStartingLevelNodes);
		}

		private void SetStartingGameNodesState(List<LevelNode> potentialStartingLevelNodes)
		{
			foreach(UIIslandController islandController in _islandsControllers)
			{
				foreach(LevelNode levelNode in islandController.LevelNodeList)
				{
					if (potentialStartingLevelNodes.Contains(levelNode))
					{
						levelNode.UnlockNode();
					}
					else
					{
						levelNode.LockNode();
					}
				}
			}
		}

		//Use for every rounds except the first one of the game
		private void SelectStartingRoundNode(LevelNode startingNode)
		{
			foreach (var island in _islandsControllers)
			{
				foreach(var node in island.LevelNodeList)
				{
					if(node.IngredientsBundle == null || node.IngredientsBundle.IsStartingGameBundle)
					{
						int randomIndex = UnityEngine.Random.Range(0, _startingRoundIngredientsBundles.Count);
						node.Initialize(_startingRoundIngredientsBundles[randomIndex]);
					}
				}
			}

			startingNode.ResetIngredientBundle();
			SetNodesState();
		}

		//------------------------------------------------------------------------------------------- CONFIRM -------------------------------------------------------------------------------------------

		public void Confirm()
		{
			if(_selectedNode == _startingSelectedNode)
			{
				foreach(UIIslandController island in _islandsControllers)
				{
					foreach(LevelNode node in island.LevelNodeList)
					{
						if(node == _startingSelectedNode)
						{
							continue;
						}

						node.LockNode();
					}
				}
			}
			if(_selectedNode != _startingSelectedNode)
			{
				_alreadyConnectedLevelNodes.Add(_selectedNode);
			}

			OnMapChoiceConfirm?.Invoke(_selectedNode.IngredientsBundle, _isFirstGameChoice);
			_mapGameObject.SetActive(false);
		}
	}
}