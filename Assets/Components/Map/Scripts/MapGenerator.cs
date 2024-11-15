using Components.Bundle;
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
		[SerializeField] private RectTransform _nodeParent;
		[SerializeField] private RectTransform _nodeLineParent;
		[SerializeField] private GameObject _nodePrefab;
		[SerializeField] private GameObject _linePrefab;
		[SerializeField] private int _nodeCount = 20;
		[SerializeField] private float _minDistance = 50f;
		[SerializeField] private float _maxDistance = 200f;
		[SerializeField] private Button _confirmButton;

		private List<LevelNode> _nodes = new List<LevelNode>();
		private LevelNode _selectedNode;
		private LevelNode _startingSelectedNode;
		private bool _isFirstGameChoice;

		private List<IngredientsBundle> _startingGameIngredientsBundles;
		private List<IngredientsBundle> _startingRoundIngredientsBundles;

		//Need to change this poor bool 
		public static Action<IngredientsBundle, bool> OnMapChoiceConfirm;

		//------------------------------------------------------------------------------------------- MONO -------------------------------------------------------------------------------------------
		private void Awake()
		{
			LevelNode.OnNodeSelected += HandleNodeSelected;
			MapState.OnMapStateStarted += Init;

			//Set up ingredients bundles list
			_startingGameIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => bundle.IsStartingGameBundle).ToList();
			_startingRoundIngredientsBundles = ScriptableObjectDatabase.GetAllScriptableObjectOfType<IngredientsBundle>().Where(bundle => !bundle.IsStartingGameBundle).ToList();
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
				_isFirstGameChoice = true;
				GenerateNodes();
				EnsureConnectivity();
				DrawConnections();
				SelectStartingGameNode();
			}
			else
			{
				_isFirstGameChoice = false;
				SelectStartingRoundNode(_selectedNode);
			}
		}
		//------------------------------------------------------------------------------------------- MAP GENERATION -------------------------------------------------------------------------------------------
		private void GenerateNodes()
		{
			// Génération des nœuds avec positionnement aléatoire
			for (int i = 0; i < _nodeCount; i++)
			{
				GameObject nodeObj = Instantiate(_nodePrefab, _nodeParent);
				RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
				Vector2 position;

				// Boucle de positionnement pour éviter le chevauchement et les distances aberrantes
				bool positionValid;
				int attempt = 0;
				do
				{
					position = new Vector2(
                        UnityEngine.Random.Range(-(_nodeParent.rect.width - 100) / 2, (_nodeParent.rect.width - 100) / 2),
                        UnityEngine.Random.Range(-(_nodeParent.rect.height - 100) / 2, (_nodeParent.rect.height - 100) / 2)
					);
					positionValid = IsPositionValid(position);
					attempt++;
				} while (!positionValid && attempt < 100);

				rectTransform.localPosition = position;
				LevelNode node = nodeObj.GetComponent<LevelNode>();
				_nodes.Add(node);


			}
		}

		private bool IsPositionValid(Vector2 position)
		{
			foreach (var node in _nodes)
			{
				if (Vector2.Distance(node.transform.localPosition, position) < _minDistance)
				{
					return false;
				}
			}
			return true;
		}


		private void EnsureConnectivity()
		{
			// Étape 1: Créer un arbre de connexions
			HashSet<LevelNode> visitedNodes = new HashSet<LevelNode>();
			List<LevelNode> treeNodes = new List<LevelNode>();

			// Commencer par un nœud aléatoire
			LevelNode startingNode = _nodes[UnityEngine.Random.Range(0, _nodes.Count)];
			CreateTree(startingNode, visitedNodes, treeNodes);

			// Étape 2: Ajouter des connexions aléatoires, mais en vérifiant les conditions
			foreach (var node in _nodes)
			{
				// Ne pas ajouter des connexions si le nœud a déjà 4 voisins
				if (node.ConnectedNodes.Count < 4)
				{
					// Connexion aléatoire avec des nœuds proches
					foreach (var potentialNeighbor in _nodes)
					{
						if (node == potentialNeighbor) continue;

						float distance = Vector2.Distance(node.transform.localPosition, potentialNeighbor.transform.localPosition);

						// Vérifiez si les nœuds sont à proximité et ajoutez une connexion basée sur la probabilité
						if (distance <= _maxDistance && UnityEngine.Random.value < 0.3f && potentialNeighbor.ConnectedNodes.Count < 4) // 5% de chance
						{
							// Vérifiez s'ils ne sont pas déjà connectés
							if (!node.ConnectedNodes.Contains(potentialNeighbor))
							{
								node.ConnectedNodes.Add(potentialNeighbor);
								potentialNeighbor.ConnectedNodes.Add(node);
							}
						}
					}
				}
			}
		}

		private void CreateTree(LevelNode currentNode, HashSet<LevelNode> visitedNodes, List<LevelNode> treeNodes)
		{
			visitedNodes.Add(currentNode);
			treeNodes.Add(currentNode);

			// Crée des connexions à partir de nœuds non visités
			foreach (var potentialNeighbor in _nodes)
			{
				if (currentNode == potentialNeighbor || visitedNodes.Contains(potentialNeighbor)) continue;

				float distance = Vector2.Distance(currentNode.transform.localPosition, potentialNeighbor.transform.localPosition);
				if (distance <= _maxDistance)
				{
					currentNode.ConnectedNodes.Add(potentialNeighbor);
					potentialNeighbor.ConnectedNodes.Add(currentNode);

					// Appel récursif
					CreateTree(potentialNeighbor, visitedNodes, treeNodes);
				}
			}
		}

		private void DFS(LevelNode node, HashSet<LevelNode> visitedNodes, List<LevelNode> group)
		{
			visitedNodes.Add(node);
			group.Add(node);

			foreach (var connectedNode in node.ConnectedNodes)
			{
				if (!visitedNodes.Contains(connectedNode))
				{
					DFS(connectedNode, visitedNodes, group);
				}
			}
		}

		private void DrawConnections()
		{
			HashSet<(LevelNode, LevelNode)> drawnConnections = new HashSet<(LevelNode, LevelNode)>();

			foreach (var node in _nodes)
			{
				foreach (var connectedNode in node.ConnectedNodes)
				{
					if (!drawnConnections.Contains((node, connectedNode)) && !drawnConnections.Contains((connectedNode, node)))
					{
						GameObject lineObj = Instantiate(_linePrefab, _nodeLineParent);
						RectTransform lineRect = lineObj.GetComponent<RectTransform>();

						Vector2 startPos = node.transform.localPosition;
						Vector2 endPos = connectedNode.transform.localPosition;
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

			foreach (var node in _nodes)
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
			_selectedNode = _startingSelectedNode;
			_startingSelectedNode.SelectNodeAsFirst();
			_confirmButton.interactable = false;
		}

		//Use for starting round only
		private void SelectStartingGameNode()
		{
			if (_nodes == null || _nodes.Count == 0)
			{
				Debug.LogWarning("La liste de nœuds est vide. Impossible de définir un nœud de départ.");
				return;
			}

			// Trouver le nœud avec le plus de connexions
			LevelNode startingNode = _nodes.OrderByDescending(node => node.ConnectedNodes.Count).FirstOrDefault();

			// Afficher le nœud de départ sélectionné
			if (startingNode != null)
			{
				Debug.Log($"Nœud de départ sélectionné : {startingNode.name} avec {startingNode.ConnectedNodes.Count} connexions.");
			}

			foreach (var node in _nodes) 
			{
				//Bind a starting game bundle to every connected nodes of starting node
				if (startingNode.ConnectedNodes.Contains(node))
				{
					int randomIndex = UnityEngine.Random.Range(0, _startingGameIngredientsBundles.Count);

					node.Initialize(_startingGameIngredientsBundles[randomIndex]);
					_startingGameIngredientsBundles.RemoveAt(randomIndex);
				}

				else if(node == startingNode)
				{
					node.ResetIngredientBundle();
				}

				//Bind a starting round bundle to every one else
				else
				{
					int randomIndex = UnityEngine.Random.Range(0, _startingRoundIngredientsBundles.Count);
					node.Initialize(_startingRoundIngredientsBundles[randomIndex]);
				}
			}

			SelectNodeAsDefaultForRound(startingNode);
		}

		//Use for every rounds except the first one of the game
		private void SelectStartingRoundNode(LevelNode startingNode)
		{
			foreach(var node in _nodes)
			{
				if(node.IngredientsBundle == null)
				{
					int randomIndex = UnityEngine.Random.Range(0, _startingRoundIngredientsBundles.Count);
					node.Initialize(_startingRoundIngredientsBundles[randomIndex]);
				}
				//Need to change the starting game bundle by starting round bundle
				else if (node.IngredientsBundle.IsStartingGameBundle)
				{
					int randomIndex = UnityEngine.Random.Range(0, _startingRoundIngredientsBundles.Count);
					node.Initialize(_startingRoundIngredientsBundles[randomIndex]);
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
