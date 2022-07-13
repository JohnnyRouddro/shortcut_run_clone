using UnityEditor;
using UnityEngine.UI;


[CustomEditor(typeof(RoundedCorners))]
public class RoundedCornersInspector : UnityEditor.Editor
{
    private RoundedCorners script;

    private void OnEnable()
    {
        script = (RoundedCorners)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!script.TryGetComponent<Image>(out var _))
        {
            EditorGUILayout.HelpBox("This script requires an Image component on the same gameobject", MessageType.Warning);
        }
    }
}
