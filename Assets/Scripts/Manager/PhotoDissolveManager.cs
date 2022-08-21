using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PhotoDissolveManager : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float timeFade = 0;
    private int TimeFadeID = Shader.PropertyToID("TIME_FADE");
    void Update(){
        Shader.SetGlobalFloat(TimeFadeID, timeFade);
    }
}
