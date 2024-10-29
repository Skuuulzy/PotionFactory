using System.Collections.Generic;
using UnityEngine;

namespace Components.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _nodePrefab; // Prefab du LevelNode
        [SerializeField] private int _nodeCount = 10; // Nombre total de nœuds
        [SerializeField] private Transform _mapParent; // Parent pour les nœuds dans l’UI
        [SerializeField] private float _minDistance = 50f; // Distance minimale entre nœuds
        [SerializeField] private float _maxDistance = 250f; // Distance maximale du centre pour chaque nœud

        private List<LevelNode> _nodes = new List<LevelNode>();
        private System.Random _random = new System.Random();

        private void Start()
        {
            GenerateNodesWithDistanceConstraints();
            ConnectNodesRandomly();
            SetStartingNode();
        }

        /// <summary>
        /// Génère des nœuds aléatoirement tout en respectant les contraintes de distance.
        /// </summary>
        private void GenerateNodesWithDistanceConstraints()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                Vector2 position;
                bool positionValid;

                // Tente de placer chaque nœud en respectant les distances min et max
                do
                {
                    position = new Vector2(
                        Random.Range(-_maxDistance, _maxDistance),
                        Random.Range(-_maxDistance, _maxDistance)
                    );

                    positionValid = IsPositionValid(position);

                } while (!positionValid); // Boucle jusqu'à ce qu'une position valide soit trouvée

                GameObject nodeObj = Instantiate(_nodePrefab, _mapParent);
                nodeObj.transform.localPosition = position;

                LevelNode levelNode = nodeObj.GetComponent<LevelNode>();
                _nodes.Add(levelNode);
                levelNode.Initialize(false); // Début avec tous les nœuds verrouillés
            }
        }

        /// <summary>
        /// Vérifie si la position est valide en respectant les distances min et max.
        /// </summary>
        private bool IsPositionValid(Vector2 newPosition)
        {
            // Vérifie que la position est dans la distance maximale du centre
            if (newPosition.magnitude > _maxDistance) return false;

            // Vérifie la distance avec les autres nœuds
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
        /// Connecte les nœuds de manière aléatoire en s'assurant qu'aucun nœud n'est isolé.
        /// </summary>
        private void ConnectNodesRandomly()
        {
            for (int i = 0; i < _nodeCount; i++)
            {
                LevelNode currentNode = _nodes[i];
                int connections = _random.Next(1, 3); // 1 à 2 connexions pour chaque nœud

                for (int j = 0; j < connections; j++)
                {
                    LevelNode randomNode = _nodes[_random.Next(_nodeCount)];

                    // Évite les connexions doubles ou les boucles vers soi-même
                    if (randomNode != currentNode && !currentNode.ConnectedNodes.Contains(randomNode))
                    {
                        currentNode.ConnectedNodes.Add(randomNode);
                        randomNode.ConnectedNodes.Add(currentNode); // Connexion bidirectionnelle
                    }
                }
            }
        }

        /// <summary>
        /// Désigne un nœud de départ aléatoire.
        /// </summary>
        private void SetStartingNode()
        {
            int startIndex = _random.Next(_nodeCount);
            _nodes[startIndex].Initialize(true); // Débloque le nœud de départ
        }
    }

}
