using FlatKit;
using UnityEditor;

namespace ExternPropertyAttributes.Editor {
[CanEditMultipleObjects]
[CustomEditor(typeof(FogSettings))]
public class FogSettingsInspector : ExternalCustomInspector { }
}