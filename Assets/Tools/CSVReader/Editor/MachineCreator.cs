using Components.Machines;
using UnityEditor;

namespace VComponent.Tools.CSVReader
{
    public class MachineCreator
    {
        private const string SO_PATH = "Assets/Components/Machines/ScriptableObjects/";
        
        public static MachineTemplate GetMachine(string name)
        {
            return AssetDatabase.LoadAssetAtPath<MachineTemplate>(MachinePath(name));
        }
        
        private static string MachinePath(string machineName)
        {
            return SO_PATH + machineName + ".asset";
        }
    }
}