using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VComponent.Tools.CSVReader
{
    public class CSVReaderWindow : EditorWindow
    {
        private string _filePath = "data"; // Default CSV file path in Resources
        private int _rowsPerPage = 10; // Number of rows per page

        private Vector2 _scrollPosition;
        private List<string[]> _csvData; // Store CSV data persistently
        private int[] _columnWidths;
        
        // -------------------------------------- GUI --------------------------------------------------------
        [MenuItem("Tools/CSV Reader")]
        public static void ShowWindow()
        {
            GetWindow<CSVReaderWindow>("CSV Reader");
        }

        private void OnGUI()
        {
            GUILayout.Label("CSV Reader", EditorStyles.boldLabel);

            _filePath = EditorGUILayout.TextField("File Path (Resources):", _filePath);
            _rowsPerPage = EditorGUILayout.IntField("Rows Per Page:", _rowsPerPage);

            if (GUILayout.Button("Read CSV"))
            {
                _csvData = CSVReader.ReadCSV(_filePath, _rowsPerPage);
                if (_csvData != null)
                {
                    CalculateColumnWidths(_csvData);
                }
            }

            if (_csvData != null)
            {
                DisplayCSVData(_csvData);
            }
        }

        private void DisplayCSVData(List<string[]> csvData)
        {
            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(600));

            foreach (var row in csvData)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < row.Length; i++)
                {
                    EditorGUILayout.LabelField(row[i], GUILayout.Width(_columnWidths[i]));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void CalculateColumnWidths(List<string[]> csvData)
        {
            int columnCount = csvData[0].Length;
            _columnWidths = new int[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                foreach (var row in csvData)
                {
                    _columnWidths[i] = Mathf.Max(_columnWidths[i], GetStringWidth(row[i]));
                }
            }
        }

        private int GetStringWidth(string str)
        {
            return (int)EditorStyles.label.CalcSize(new GUIContent(str)).x;
        }
    }
}