using System.Collections.Generic;
using UnityEngine;

namespace Components.Map
{

	public class MapGenerator : MonoBehaviour
	{
		[SerializeField] private RectTransform _nodeParent;
		[SerializeField] private RectTransform _nodeLineParent;
		[SerializeField] private GameObject _nodePrefab;
		[SerializeField] private GameObject _linePrefab;
		[SerializeField] private int _nodeCount = 20;
		[SerializeField] private float _minDistance = 50f;
		[SerializeField] private float _maxDistance = 200f;

		private List<LevelNode> _nodes = new List<LevelNode>();

		private void Start()
		{
			GenerateNodes();
			EnsureConnectivity();
			DrawConnections();

			LevelNode.OnNodeSelected += HandleNodeSelected;
		}



		private void GenerateNodes()
		{
			// G�n�ration des n�uds avec positionnement al�atoire
			for (int i = 0; i < _nodeCount; i++)
			{
				GameObject nodeObj = Instantiate(_nodePrefab, _nodeParent);
				RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
				Vector2 position;

				// Boucle de positionnement pour �viter le chevauchement et les distances aberrantes
				bool positionValid;
				int attempt = 0;
				do
				{
					position = new Vector2(
						Random.Range(-(_nodeParent.rect.width - 100) / 2, (_nodeParent.rect.width - 100) / 2),
						Random.Range(-(_nodeParent.rect.height - 100) / 2, (_nodeParent.rect.height - 100) / 2)
					);
					positionValid = IsPositionValid(position);
					attempt++;
				} while (!positionValid && attempt < 100);

				rectTransform.localPosition = position;
				LevelNode node = nodeObj.GetComponent<LevelNode>();
				_nodes.Add(node);

				//Chose any starting point
				node.Initialize(true);
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
			// �tape 1: Cr�er un arbre de connexions
			HashSet<LevelNode> visitedNodes = new HashSet<LevelNode>();
			List<LevelNode> treeNodes = new List<LevelNode>();

			// Commencer par un n�ud al�atoire
			LevelNode startingNode = _nodes[Random.Range(0, _nodes.Count)];
			CreateTree(startingNode, visitedNodes, treeNodes);

			// �tape 2: Ajouter des connexions al�atoires, mais en v�rifiant les conditions
			foreach (var node in _nodes)
			{
				// Ne pas ajouter des connexions si le n�ud a d�j� 4 voisins
				if (node.ConnectedNodes.Count < 4)
				{
					// Connexion al�atoire avec des n�uds proches
					foreach (var potentialNeighbor in _nodes)
					{
						if (node == potentialNeighbor) continue;

						float distance = Vector2.Distance(node.transform.localPosition, potentialNeighbor.transform.localPosition);

						// V�rifiez si les n�uds sont � proximit� et ajoutez une connexion bas�e sur la probabilit�
						if (distance <= _maxDistance && Random.value < 0.05f && potentialNeighbor.ConnectedNodes.Count < 4) // 5% de chance
						{
							// V�rifiez s'ils ne sont pas d�j� connect�s
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

			// Cr�e des connexions � partir de n�uds non visit�s
			foreach (var potentialNeighbor in _nodes)
			{
				if (currentNode == potentialNeighbor || visitedNodes.Contains(potentialNeighbor)) continue;

				float distance = Vector2.Distance(currentNode.transform.localPosition, potentialNeighbor.transform.localPosition);
				if (distance <= _maxDistance)
				{
					currentNode.ConnectedNodes.Add(potentialNeighbor);
					potentialNeighbor.ConnectedNodes.Add(currentNode);

					// Appel r�cursif
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

		private void HandleNodeSelected(LevelNode nodeSelected)
		{
			foreach(var node in _nodes)
			{
				if(node == nodeSelected || nodeSelected.ConnectedNodes.Contains(node))
				{
					continue;
				}
				else
				{
					node.LockNode();
				}
			}
		}

	}
}
