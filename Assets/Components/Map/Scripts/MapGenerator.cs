using System.Collections.Generic;
using UnityEngine;

namespace Components.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _nodePrefab; // Prefab du LevelNode
        [SerializeField] private int _nodeCount = 10; // Nombre total de n�uds
        [SerializeField] private Transform _mapParent; // Parent pour les n�uds dans l�UI
        [SerializeField] private float _minDistance = 50f; // Distance minimale entre n�uds
        [SerializeField] private float _maxDistance = 250f; // Distance maximale du centre pour chaque n�ud

        private List<LevelNode> _nodes = new List<LevelNode>();
        private System.Random _random = new System.Random();

        private void Start()
        {
            GenerateNodesWithDistanceConstraints();
            ConnectNodesRandomly();
            SetStartingNode();
        }

        /// <summary>
        /// G�n�re des n�uds al�atoirement tout en respectant les contraintes de distance.
        /// </summary>
        private void GenerateNodesWithDistanceConstraints()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                Vector2 position;
                bool positionValid;

                // Tente de placer chaque n�ud en respectant les distances min et max
                do
                {
                    position = new Vector2(
                        Random.Range(-_maxDistance, _maxDistance),
                        Random.Range(-_maxDistance, _maxDistance)
                    );

                    positionValid = IsPositionValid(position);

                } while (!positionValid); // Boucle jusqu'� ce qu'une position valide soit trouv�e

                GameObject nodeObj = Instantiate(_nodePrefab, _mapParent);
                nodeObj.transform.localPosition = position;

                LevelNode levelNode = nodeObj.GetComponent<LevelNode>();
                _nodes.Add(levelNode);
                levelNode.Initialize(false); // D�but avec tous les n�uds verrouill�s
            }
        }

        /// <summary>
        /// V�rifie si la position est valide en respectant les distances min et max.
        /// </summary>
        private bool IsPositionValid(Vector2 newPosition)
        {
            // V�rifie que la position est dans la distance maximale du centre
            if (newPosition.magnitude > _maxDistance) return false;

            // V�rifie la distance avec les autres n�uds
            foreach (var node in _nodes)
            {
                if (Vector2.Distance(newPosition, node.transform.localPosition) < _minDistance)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Connecte les n�uds de mani�re al�atoire en s'assurant qu'aucun n�ud n'est isol�.
        /// </summary>
        private void ConnectNodesRandomly()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                LevelNode currentNode = _nodes[i];
                int connections = _random.Next(1, 3); // 1 � 2 connexions pour chaque n�ud

                for (int j = 0; j < connections; j++)
                {
                    LevelNode randomNode = _nodes[_random.Next(_nodeCount)];

                    // �vite les connexions doubles ou les boucles vers soi-m�me
                    if (randomNode != currentNode && !currentNode.ConnectedNodes.Contains(randomNode))
                    {
                        currentNode.ConnectedNodes.Add(randomNode);
                        randomNode.ConnectedNodes.Add(currentNode); // Connexion bidirectionnelle
                    }
                }
            }
        }

        /// <summary>
        /// D�signe un n�ud de d�part al�atoire.
        /// </summary>
        private void SetStartingNode()
        {
            int startIndex = _random.Next(_nodeCount);
            _nodes[startIndex].Initialize(true); // D�bloque le n�ud de d�part
        }
    }

}
