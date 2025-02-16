using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace VComponent.Tools.CSVReader
{
    public static class CSVReader
    {
        // Regular expression to split by commas but ignore commas inside quotes
        private const string PATTERN = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        
        public static List<string[]> ReadCSV(string path, int rowsCount = - 1, bool excludeFirstLine = true)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(path);
            if (csvFile == null)
            {
                Debug.LogError($"File not found at path: {path}");
                return null;
            }

            StringReader reader = new StringReader(csvFile.text);
            List<string[]> csvData = new List<string[]>();

            var currentRow = 0;
            var startRow = excludeFirstLine ? 1 : 0;
            var endRow = rowsCount == -1 ? float.PositiveInfinity : rowsCount;

            while (reader.ReadLine() is { } line)
            {
                if (currentRow >= startRow && currentRow < endRow)
                {
                    string[] substrings = Regex.Split(line, PATTERN);

                    // Optionally, trim the quotes from the strings
                    for (int i = 0; i < substrings.Length; i++)
                    {
                        substrings[i] = substrings[i].Trim('"');
                    }

                    csvData.Add(substrings);
                }

                currentRow++;
            }

            return csvData;
        }
    }
}