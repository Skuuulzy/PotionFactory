using Components.Bundle;
using Components.Island;
using Components.Order;
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
		[SerializeField] private RectTransform _nodeLineParent;
		[SerializeField] private NodeLineController _linePrefab;
		[SerializeField] private Button _confirmButton;
		[SerializeField] private List<UIIslandController> _islandsControllers = new List<UIIslandController>();

		[Header("Helps")]
		[SerializeField] private OrderDialogueController _orderDialogueController;


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
			_orderDialogueController.gameObject.SetActive(true);

			//First map generation
			if (state.StateIndex == 1)
			{
				RegenerateMap();
				_orderDialogueController.SetText("Choose your departure city, young apprentice!");
			}
			else
			{
				_isFirstGameChoice = false;
				SelectStartingRoundNode(_startingSelectedNode);
				_orderDialogueController.SetText("Build a new trade route!");

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

			foreach (Transform child in _nodeLineParent)
			{
				Destroy(child.gameObject);
			}

			//_islandsControllers = new List<UIIslandController>();
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
			//No more random selection of islands for V1 

			//_allIslandTemplate = _allIslandTemplate.OrderBy(_ => UnityEngine.Random.value).ToList();
			//for (int i = 0; i < _islandsParents.Count; i++)
			//{
			//	UIIslandController controller = Instantiate(_allIslandTemplate[i].Controller, _islandsParents[i]);
			//	_islandsControllers.Add(controller);
			//	controller.SetIslandName(_allIslandTemplate[i].Name);
			//}

		}

		public void ConnectIslands()
		{
			//No more random connections of islands for V1 


			//Dictionary<LevelNode, LevelNode> confirmedConnections = new Dictionary<LevelNode, LevelNode>();

			//foreach (var island in _islandsControllers)
			//{
			//	Dictionary<LevelNode, List<LevelNode>> validConnectionByLevelNode = new Dictionary<LevelNode, List<LevelNode>>();

			//	// Get all valid connection for each node
			//	foreach (var node in island.LevelNodeList)
			//	{
			//		var validConnections = FindValidConnections(island, node);
			//		validConnectionByLevelNode.Add(node, validConnections);
			//	}


			//	foreach (var node in validConnectionByLevelNode.Keys)
			//	{
			//		foreach (var targetNode in validConnectionByLevelNode[node])
			//		{
			//			// Check if the targetNode is already connected
			//			if (confirmedConnections.TryGetValue(targetNode, out var existingNode))
			//			{
			//				// If new node is closer then we change the old one by it
			//				if (GetDistance(targetNode, node) < GetDistance(targetNode, existingNode))
			//				{
			//					confirmedConnections[targetNode] = node;
			//				}
			//			}
			//			else
			//			{
			//				// Not connected so we create it
			//				confirmedConnections[targetNode] = node;
			//			}
			//		}
			//	}
			//}

			////Create connections
			//foreach (var kvp in confirmedConnections)
			//{
			//	if(kvp.Key.ExternalConnectedNode.Count < 1 && kvp.Value.ExternalConnectedNode.Count < 1)
			//	{
			//		kvp.Key.ConnectedNodes.Add(kvp.Value);
			//		kvp.Key.ExternalConnectedNode.Add(kvp.Value);
			//		kvp.Value.ConnectedNodes.Add(kvp.Key);
			//		kvp.Value.ExternalConnectedNode.Add(kvp.Key);
			//	}
			//}
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
							NodeLineController lineObj = Instantiate(_linePrefab, _nodeLineParent);
							RectTransform lineRect = lineObj.GetComponent<RectTransform>();

							Vector2 startPos = node.transform.position;
							Vector2 endPos = connectedNode.transform.position;
							Vector2 midPoint = (startPos + endPos) / 2;

							lineRect.localPosition = midPoint;

							float distance = Vector2.Distance(startPos, endPos);
							lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);

							float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
							lineRect.rotation = Quaternion.Euler(0, 0, angle);

							//We don't need the road segment for now
							//lineObj.SetNormalizedDistance(Mathf.CeilToInt(distance / 150));
							drawnConnections.Add((node, connectedNode));
							node.Lines.TryAdd(lineObj, false);
							connectedNode.Lines.TryAdd(lineObj, true);
						}
					}
				}
			}
		}

		//------------------------------------------------------------------------------------------- NODE SELECTION -------------------------------------------------------------------------------------------
		private void HandleNodeSelected(LevelNode nodeSelected)
		{
			_confirmButton.interactable = true;



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
				node.UnlockNode(false);
			}

			foreach(LevelNode alreadyConnectedNode in _alreadyConnectedLevelNodes)
			{
				alreadyConnectedNode.SetNodeAsConnected();

				foreach (LevelNode node in alreadyConnectedNode.ConnectedNodes)
				{
					node.UnlockNode(false);
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
				Debug.LogWarning("La liste de n�uds est vide. Impossible de d�finir un n�ud de d�part.");
				return;
			}

			_startingGameIngredientsBundles = _startingGameIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			List<LevelNode> potentialStartingLevelNodes = new List<LevelNode>();

			var startingRoundIngredientsBundles = _startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			for( int i = 0; i < _islandsControllers.Count; i++)
			{
				//Set starting bundle for Starting node of each island
				_islandsControllers[i].InitStartingBundle(_startingGameIngredientsBundles[i]);
				potentialStartingLevelNodes.Add(_islandsControllers[i].StartingLevelNode);

				//checking if we have enough ingredients to fill the island
				if(startingRoundIngredientsBundles.Count < _islandsControllers[i].NumberOfNodes)
				{
					startingRoundIngredientsBundles = _startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
				}
				//Set ingredient bundle for every node except the starting node
				_islandsControllers[i].Init(startingRoundIngredientsBundles.Take(_islandsControllers[i].NumberOfNodes).ToArray());
				startingRoundIngredientsBundles.RemoveRange(0,_islandsControllers[i].NumberOfNodes);
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
						levelNode.UnlockNode(true);
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
			_selectedNode.SetConnectedNodesConstructedLineColor(true);

			if (_startingSelectedNode == null)
			{
				_startingSelectedNode = _selectedNode;
				foreach(UIIslandController island in _islandsControllers)
				{
					foreach(LevelNode node in island.LevelNodeList)
					{
						if(node == _startingSelectedNode || _startingSelectedNode.ConnectedNodes.Contains(node))
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