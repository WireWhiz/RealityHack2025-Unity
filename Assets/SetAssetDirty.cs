using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetAssetDirty : MonoBehaviour
{
    public ScriptableObject asset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SetAssetDirty))]
class SetAssetDirtyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Set Dirty"))
        {
            EditorUtility.SetDirty(((SetAssetDirty)target).asset);
        }
    }
}
#endif