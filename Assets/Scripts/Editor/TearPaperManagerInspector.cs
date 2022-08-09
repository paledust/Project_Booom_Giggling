using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TearPaperManager))]
public class TearPaperManagerInspector : Editor
{
    private static bool DebugMode = false;
    private static bool HollowMode  = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUI.backgroundColor = DebugMode?Color.red:Color.white;
        if(GUILayout.Button("Toggle Shader Debug Mode")){
            DebugMode = !DebugMode;
            Shader.SetGlobalInt(Service.DEBUG_ID, DebugMode?1:0);
            SceneView.RepaintAll();
        }
        if(GUILayout.Button("Toggle Shader Hollow Mode")){
            HollowMode = !HollowMode;
            Shader.SetGlobalInt(Service.HOLLOW_ID, HollowMode?1:0);
            SceneView.RepaintAll();
        }
    }
}
