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
				SelectStartingRoundNode(_selectedNode);
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
			SelectStartingGameNode();
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
			foreach (var island in _islandsControllers)
			{
				foreach (var node in island.LevelNodeList)
				{
					var validConnections = FindValidConnections(island,node);

					foreach (var connection in validConnections)
					{
						if (!node.ConnectedNodes.Contains(connection) && node.ExternalConnectedNode.Count < 1 && connection.ExternalConnectedNode.Count < 1)
						{
							node.ConnectedNodes.Add(connection);
							node.ExternalConnectedNode.Add(connection);
							connection.ConnectedNodes.Add(node);
							connection.ExternalConnectedNode.Add(node);
						}
					}
				}
			}
		}

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
					if (node == otherNode) continue;

					if (AreNodesConnectable(node, otherNode))
					{
						float distance = Vector3.Distance(node.transform.position, otherNode.transform.position);

						validConnections.Add(otherNode);
					}
				}
			}

			return validConnections.OrderBy(n => Vector3.Distance(node.transform.position, n.transform.position)).ToList();
		}

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
			if (_selectedNode != _startingSelectedNode && _selectedNode != nodeSelected)
			{
				_selectedNode.UnselectNode();
			}

			_selectedNode = nodeSelected;
		}

		private void SelectNodeAsDefaultForRound(LevelNode nodeSelected)
		{
			_startingSelectedNode = nodeSelected;

			foreach (var island in _islandsControllers)
			{
				foreach(var node in island.LevelNodeList)
				{
					if (node == nodeSelected || nodeSelected.ConnectedNodes.Contains(node))
					{
						node.UnlockNode();
					}
					else
					{
						node.LockNode();
					}
				}

			}
			_selectedNode = _startingSelectedNode;
			_startingSelectedNode.SelectNodeAsFirst();
			_confirmButton.interactable = false;
		}

		//Use for starting round only
		private void SelectStartingGameNode()
		{
			if (_islandsControllers == null || _islandsControllers.Count == 0)
			{
				Debug.LogWarning("La liste de nœuds est vide. Impossible de définir un nœud de départ.");
				return;
			}

			//Select starting island
			UIIslandController startingIslandController = _islandsControllers.OrderByDescending(island => island.NumberOfNodes).FirstOrDefault();
			//Select starting node
			LevelNode startingNode = startingIslandController.StartingLevelNode;
			//Set starting game ingredient bundles to each node in this island
			_startingGameIngredientsBundles = _startingGameIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList();
			startingIslandController.Init(_startingGameIngredientsBundles.Take(startingIslandController.NumberOfNodes).ToArray());
			//Reset starting node bundle
			startingNode.ResetIngredientBundle();

			//All island except the starting one
			UIIslandController[] nonStartingIslandControllers = _islandsControllers.Where(island => island != startingIslandController).ToArray();
			foreach(UIIslandController nonStartingIslandController in nonStartingIslandControllers)
			{
				//Randomize ingredients bundle list
				_startingRoundIngredientsBundles = _startingRoundIngredientsBundles.OrderBy(_ => UnityEngine.Random.value).ToList(); 
				nonStartingIslandController.Init(_startingRoundIngredientsBundles.Take(nonStartingIslandController.NumberOfNodes).ToArray());
			}


			SelectNodeAsDefaultForRound(startingNode);
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
			SelectNodeAsDefaultForRound(startingNode);
		}

		//------------------------------------------------------------------------------------------- CONFIRM -------------------------------------------------------------------------------------------

		public void Confirm()
		{
			OnMapChoiceConfirm?.Invoke(_selectedNode.IngredientsBundle, _isFirstGameChoice);
			_mapGameObject.SetActive(false);
		}
	}
}