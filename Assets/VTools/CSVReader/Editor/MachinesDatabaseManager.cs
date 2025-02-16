using Components.Machines;
using Database;
using UnityEditor;

namespace VComponent.Tools.CSVReader
{
    public class MachinesDatabaseManager
    {
        public static MachineTemplate GetMachine(string name)
        {
            return ScriptableObjectDatabase.GetScriptableObject<MachineTemplate>(name);
        }
        
        private static string MachinePath(string machineName)
        {
            return ScriptableObjectDatabase.MACHINE_SO_PATH + machineName + ".asset";
        }
    }
}