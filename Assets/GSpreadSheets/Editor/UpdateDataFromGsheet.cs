using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Collections;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Newtonsoft.Json;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;

public class UpdateDataFromGsheet : EditorWindow
{

    static string CLIENT_ID = "871414866606-7b9687cp1ibjokihbbfl6nrjr94j14o8.apps.googleusercontent.com";

    static string CLIENT_SECRET = "zF_J3qHpzX5e8i2V-ZEvOdGV";

    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

    // Key of the spreadsheet. Get from url of the spreadsheet.
    private string spreadSheetKey = "1Qgv3wwIRpp67Na8QeI7prMDv4VF_ApnMOLw3vJZNtQQ";

    //Sheet name (name in google sheet)
    private string ingredientsSheet = "CSV_Ingredients";
    private string recipesSheet = "CSV_Recipes";

    //Sheet save path
    private string ingredientsPath = "./Assets/Resources/JsonData/";
    private string recipesPath = "./Assets/Resources/JsonData/";

    //Name of application.
    private string appName = "Unity";

    // The root of spreadsheet's url.
    private string urlRoot = "https://spreadsheets.google.com/feeds/spreadsheets/";

    // The directory which contain json files.
    [SerializeField] private string outputDir = "./Assets/Resources/JsonData/";

    // The data types which is allowed to convert from sheet to json object
    private static List<string> allowedDataTypes = new List<string>() { "string", "int", "bool", "float", "string[]", "int[]", "bool[]", "float[]" };

    // Position of the scroll view.
    private Vector2 scrollPosition;

    // Progress of download and convert action. 100 is "completed".
    private float progress = 100;

    // The message which be shown on progress bar when action is running.
    private string progressMessage = "";

    [MenuItem("Utility/Update data from Gsheet")]
    private static void ShowWindow()
    {
        UpdateDataFromGsheet window = EditorWindow.GetWindow(typeof(UpdateDataFromGsheet)) as UpdateDataFromGsheet;
    }

    public void Init()
    {
        progress = 100;
        progressMessage = "";
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    private void OnGUI()
    {
        Init();

        GUILayout.BeginVertical();
        {
            GUI.backgroundColor = UnityEngine.Color.green;

            GUILayout.Label("Update data", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Update ingredients", GUILayout.Width(500)))
            {
                progress = 0;
                UpdateData(ingredientsSheet, ingredientsPath);
            }
            GUILayout.Label(ingredientsPath);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Update recipes", GUILayout.Width(500)))
            {
                progress = 0;
                UpdateData(recipesSheet, recipesPath);
            }
            GUILayout.Label(recipesPath);
            GUILayout.EndHorizontal();

            GUI.backgroundColor = UnityEngine.Color.white;
            if ((progress < 100) && (progress > 0))
            {
                if (EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress / 100))
                {
                    progress = 100;
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }
        try
        {
            GUILayout.EndVertical();
        }
        catch (Exception ex)
        {
            //Sometimes, Unity fire a "InvalidOperationException: Stack empty." bug when Editor want to end a group layout
        }
    }

    private void UpdateData(string sheetName, string path)
    {
        //Validate input
        if (string.IsNullOrEmpty(spreadSheetKey))
        {
            Debug.LogError("spreadSheetKey can not be null!");
            return;
        }

        Debug.Log("Start downloading from key: " + spreadSheetKey);

        //Authenticate
        progressMessage = "Authenticating...";
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GetCredential(),
            ApplicationName = appName,
        });

        progress = 5;
        EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress / 100);
        progressMessage = "Get list of spreadsheets...";
        EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress / 100);

        Spreadsheet spreadSheetData = service.Spreadsheets.Get(spreadSheetKey).Execute();
        IList<Sheet> sheets = spreadSheetData.Sheets;


        if ((sheets == null) || (sheets.Count <= 0))
        {
            Debug.LogError("Not found any data!");
            progress = 100;
            EditorUtility.ClearProgressBar();
            return;
        }

        progress = 15;

        //For each sheet in received data, check the sheet name. If that sheet is the wanted sheet, add it into the ranges.
        List<string> ranges = new List<string>();
        foreach (Sheet sheet in sheets)
        {
            if (sheetName == sheet.Properties.Title)
            {
                ranges.Add(sheet.Properties.Title);
            }
        }

        SpreadsheetsResource.ValuesResource.BatchGetRequest request = service.Spreadsheets.Values.BatchGet(spreadSheetKey);
        request.Ranges = ranges;
        BatchGetValuesResponse response = request.Execute();

        //For each wanted sheet, create a json file
        foreach (ValueRange valueRange in response.ValueRanges)
        {
            string Sheetname = valueRange.Range.Split('!')[0];
            progressMessage = string.Format("Processing {0}...", Sheetname);
            EditorUtility.DisplayCancelableProgressBar("Processing", progressMessage, progress / 100);

            //Create json file
            CreateJsonFile(Sheetname, path, valueRange);
            progress = 65;
        }
        progress = 100;
        AssetDatabase.Refresh();

        Debug.Log("Download completed.");
    }

    private void CreateJsonFile(string fileName, string outputDirectory, ValueRange valueRange)
    {
        //Get properties's name, data type and sheet data
        IDictionary<int, string> propertyNames = new Dictionary<int, string>(); //Dictionary of (column index, property name of that column)
        IDictionary<int, string> dataTypes = new Dictionary<int, string>();     //Dictionary of (column index, data type of that column)
        IDictionary<int, Dictionary<int, string>> values = new Dictionary<int, Dictionary<int, string>>();  //Dictionary of (row index, dictionary of (column index, value in cell))
        int rowIndex = 0;
        foreach (IList<object> row in valueRange.Values)
        {
            int columnIndex = 0;
            foreach (string cellValue in row)
            {
                string value = cellValue;
                if (rowIndex == 0)
                {//This row is properties's name row
                    propertyNames.Add(columnIndex, value);
                }
                else if (rowIndex == 1)
                {//This row is properties's data type row
                    dataTypes.Add(columnIndex, value);
                }
                else
                {//Data rows
                 //Because first row is name row and second row is data type row, so we will minus 2 from rowIndex to make data index start from 0
                    if (!values.ContainsKey(rowIndex - 2))
                    {
                        values.Add(rowIndex - 2, new Dictionary<int, string>());
                    }
                    values[rowIndex - 2].Add(columnIndex, value);
                }

                columnIndex++;
            }

            rowIndex++;
        }

        //Create list of Dictionaries (property name, value). Each dictionary represent for a object in a row of sheet.
        List<object> datas = new List<object>();
        foreach (int rowId in values.Keys)
        {
            bool thisRowHasError = false;
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (int columnId in propertyNames.Keys)
            {//Read through all columns in sheet, with each column, create a pair of property(string) and value(type depend on dataType[columnId])
                if (thisRowHasError) break;
                if ((!dataTypes.ContainsKey(columnId)) || (!allowedDataTypes.Contains(dataTypes[columnId])))
                    continue;   //There is not any data type or this data type is strange. May be this column is used for comments. Skip this column.
                if (!values[rowId].ContainsKey(columnId))
                {
                    values[rowId].Add(columnId, "");
                }

                string strVal = values[rowId][columnId];

                switch (dataTypes[columnId])
                {
                    case "string":
                        {
                            data.Add(propertyNames[columnId], strVal);
                            break;
                        }
                    case "int":
                        {
                            int val = 0;
                            if (!string.IsNullOrEmpty(strVal))
                            {
                                try
                                {
                                    val = int.Parse(strVal);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                    thisRowHasError = true;
                                    continue;
                                }
                            }
                            data.Add(propertyNames[columnId], val);
                            break;
                        }
                    case "bool":
                        {
                            bool val = false;
                            if (!string.IsNullOrEmpty(strVal))
                            {
                                try
                                {
                                    val = bool.Parse(strVal);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                    continue;
                                }
                            }
                            data.Add(propertyNames[columnId], val);
                            break;
                        }
                    case "float":
                        {
                            float val = 0f;
                            if (!string.IsNullOrEmpty(strVal))
                            {
                                try
                                {
                                    val = float.Parse(strVal);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                    continue;
                                }
                            }
                            data.Add(propertyNames[columnId], val);
                            break;
                        }
                    case "string[]":
                        {
                            string[] valArr = strVal.Split(new char[] { ',' });
                            data.Add(propertyNames[columnId], valArr);
                            break;
                        }
                    case "int[]":
                        {
                            string[] strValArr = strVal.Split(new char[] { ',' });
                            int[] valArr = new int[strValArr.Length];
                            if (string.IsNullOrEmpty(strVal.Trim()))
                            {
                                valArr = new int[0];
                            }
                            bool error = false;
                            for (int i = 0; i < valArr.Length; i++)
                            {
                                int val = 0;
                                if (!string.IsNullOrEmpty(strValArr[i]))
                                {
                                    try
                                    {
                                        val = int.Parse(strValArr[i]);
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                        error = true;
                                        break;
                                    }
                                }
                                valArr[i] = val;
                            }
                            if (error)
                                continue;
                            data.Add(propertyNames[columnId], valArr);
                            break;
                        }
                    case "bool[]":
                        {
                            string[] strValArr = strVal.Split(new char[] { ',' });
                            bool[] valArr = new bool[strValArr.Length];
                            if (string.IsNullOrEmpty(strVal.Trim()))
                            {
                                valArr = new bool[0];
                            }
                            bool error = false;
                            for (int i = 0; i < valArr.Length; i++)
                            {
                                bool val = false;
                                if (!string.IsNullOrEmpty(strValArr[i]))
                                {
                                    try
                                    {
                                        val = bool.Parse(strValArr[i]);
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                        error = true;
                                        break;
                                    }
                                }
                                valArr[i] = val;
                            }
                            if (error)
                                continue;
                            data.Add(propertyNames[columnId], valArr);
                            break;
                        }
                    case "float[]":
                        {
                            string[] strValArr = strVal.Split(new char[] { ',' });
                            float[] valArr = new float[strValArr.Length];
                            if (string.IsNullOrEmpty(strVal.Trim()))
                            {
                                valArr = new float[0];
                            }
                            bool error = false;
                            for (int i = 0; i < valArr.Length; i++)
                            {
                                float val = 0f;
                                if (!string.IsNullOrEmpty(strValArr[i]))
                                {
                                    try
                                    {
                                        val = float.Parse(strValArr[i]);
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debug.LogError(string.Format("There is exception when parse value of property {0} of {1} class.\nDetail: {2}", propertyNames[columnId], fileName, e.ToString()));
                                        error = true;
                                        break;
                                    }
                                }
                                valArr[i] = val;
                            }
                            if (error)
                                continue;
                            data.Add(propertyNames[columnId], valArr);
                            break;
                        }
                    default: break; //This data type is strange, may be this column is used for comments, not for store data, so do nothing and read next column.
                }
            }

            if (!thisRowHasError)
            {
                datas.Add(data);
            }
            else
            {
                Debug.LogError("There's error!");
            }
        }



        //Create json text
        string jsonText = JsonConvert.SerializeObject((object)datas);

        //Create directory to store the json file
        if (!outputDirectory.EndsWith("/"))
            outputDirectory += "/";

        // Create directory to store the json file
        Directory.CreateDirectory(outputDirectory);


        StreamWriter strmWriter = new StreamWriter(outputDirectory + fileName + ".json", false, System.Text.Encoding.UTF8);
        strmWriter.Write(jsonText);
        strmWriter.Close();

        Debug.Log("Created: " + fileName + ".json");
    }

    UserCredential GetCredential()
    {
        MonoScript ms = MonoScript.FromScriptableObject(this);
        string scriptFilePath = AssetDatabase.GetAssetPath(ms);
        FileInfo fi = new FileInfo(scriptFilePath);
        string scriptFolder = fi.Directory.ToString();
        scriptFolder.Replace('\\', '/');
        Debug.Log("Save Credential to: " + scriptFolder);

        UserCredential credential = null;
        ClientSecrets clientSecrets = new ClientSecrets();
        clientSecrets.ClientId = CLIENT_ID;
        clientSecrets.ClientSecret = CLIENT_SECRET;
        try
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(scriptFolder, true)).Result;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        return credential;
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        Debug.LogError("certificate chain is not valid");
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}
