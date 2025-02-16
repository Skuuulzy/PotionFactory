using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Components.Tools.ExtensionMethods
{
    public static class ListExtensionsMethods
    {
        public static List<int> GetRandomIndexes(int listCount, int numberOfIndexes)
        {
            Random random = new Random();
            HashSet<int> selectedIndexes = new HashSet<int>();
        
            // Ensure we don't select more indexes than available elements in the list
            if (numberOfIndexes > listCount)
            {
                Debug.LogError("Number of indexes exceeds the list count.");
                return new List<int>();
            }
        
            while (selectedIndexes.Count < numberOfIndexes)
            {
                int randomIndex = random.Next(0, listCount);
                selectedIndexes.Add(randomIndex); // HashSet prevents duplicates automatically
            }
        
            return new List<int>(selectedIndexes);
        }
    }
}