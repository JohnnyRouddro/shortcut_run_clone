using UnityEditor;
using UnityEngine.UI;


[CustomEditor(typeof(IndependentRoundedCorners))]
public class IndependentRoundedCornersInspector : UnityEditor.Editor
{
    private IndependentRoundedCorners script;

    private void OnEnable()
    {
        script = (IndependentRoundedCorners)target;
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
