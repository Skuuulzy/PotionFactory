using Components.Machines;
using Database;
using UnityEditor;

namespace VComponent.Tools.CSVReader
{
    public class MachinesDatabaseManager
    {
        public static MachineTemplate GetMachine(string name)
        {
            return AssetDatabase.LoadAssetAtPath<MachineTemplate>(MachinePath(name));
        }
        
        private static string MachinePath(string machineName)
        {
            return ScriptableObjectDatabase.MACHINE_SO_PATH + machineName + ".asset";
        }
    }
}