using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSaveSystem;
[ExecuteInEditMode]
public class PhotoDissolveManager : SaveableBehavior
{
    [Range(0, 1)] public float timeFade = 0;
    [SerializeField, Range(0,1)] private float[] timeFadeValues;
    [SerializeField] private int iteration = 0;
    void Awake(){
        EventHandler.E_OnNextSmileValue += SetTimeFadeValue;
    }
    void OnDestroy() {
        EventHandler.E_OnNextSmileValue -= SetTimeFadeValue;
    }
    public void SetTimeFadeValue(){
        iteration ++;
        if(iteration == timeFadeValues.Length) iteration = 0;
        timeFade = timeFadeValues[iteration];
        Shader.SetGlobalFloat(Service.TIME_FADE_ID, timeFadeValues[iteration]);
    }
    public override void RestoreState(object state){
        Debug.Log("Load");
        iteration = (int)System.Convert.ToInt32(state);
        if(iteration>0){
            iteration ++;
            if(iteration == timeFadeValues.Length) iteration = 0;
        }
        timeFade = timeFadeValues[iteration];
        Shader.SetGlobalFloat(Service.TIME_FADE_ID, timeFadeValues[iteration]);
    }
    public override object CaptureState(){
        Debug.Log("Save");
        return iteration;
    }
    void Update(){
        Shader.SetGlobalFloat(Service.TIME_FADE_ID, timeFade);
    }
}
