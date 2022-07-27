using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FingerPutter : MonoBehaviour
{
    [SerializeField] private float ScaledDown = 0.9f;
    [SerializeField] private Key key;
    private Vector3 initScale;
    void Awake(){
        initScale = transform.localScale;
    }
    void Update(){
        if(Keyboard.current[key].wasPressedThisFrame){
            Vector3 scale = initScale;
            scale.y *= ScaledDown;
            transform.localScale = scale;
            EventHandler.Call_OnPutOnFingers(true);
        }
        else if(Keyboard.current[key].wasReleasedThisFrame){
            transform.localScale = initScale;
            EventHandler.Call_OnPutOnFingers(false);
        }
    }
}
