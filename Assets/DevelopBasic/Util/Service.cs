using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Service
{
#region Parameter
    public static string TestString = "This is a test string, to show how you can use string in service.";
    public static LayerMask InteractableLayer = 1 << LayerMask.NameToLayer("Interactable"); //TO Do: Name whatever the interactable layer should be
    public static int DRAG_POINT_ID = Shader.PropertyToID("DRAG_POINT");
    public static int DEBUG_ID = Shader.PropertyToID("DEBUG");
    public static int HOLLOW_ID  = Shader.PropertyToID("HOLLOW");
    public static int TIME_FADE_ID = Shader.PropertyToID("TIME_FADE");
#endregion

#region HelpFunction
    /// <summary>
    /// Return a list of all active and inactive objects of T type in loaded scenes.
    /// </summary>
    /// <typeparam name="T">Object Type</typeparam>
    /// <returns></returns>
    public static T[] FindComponentsOfTypeIncludingDisable<T>(){
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
        var MatchObjects = new List<T> ();

        for(int i=0; i<sceneCount; i++){
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);
            
            var RootObjects = scene.GetRootGameObjects ();

            foreach (var obj in RootObjects) {
                var Matches = obj.GetComponentsInChildren<T> (true);
                MatchObjects.AddRange (Matches);
            }
        }

        return MatchObjects.ToArray ();
    }
#endregion
}