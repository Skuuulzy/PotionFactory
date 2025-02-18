using UnityEngine;

namespace VTools.SoWorkflow
{
    public class BaseScriptableObject : ScriptableObject
    {
        [SerializeField, TextArea] private string _description;
    }
}