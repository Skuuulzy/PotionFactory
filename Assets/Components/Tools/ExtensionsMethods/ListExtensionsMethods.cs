using System;
using System.Collections.Generic;

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
                throw new ArgumentException("Number of indexes exceeds the list count.");
        
            while (selectedIndexes.Count < numberOfIndexes)
            {
                int randomIndex = random.Next(0, listCount - 1);
                selectedIndexes.Add(randomIndex); // HashSet prevents duplicates automatically
            }
        
            return new List<int>(selectedIndexes);
        }
    }
}