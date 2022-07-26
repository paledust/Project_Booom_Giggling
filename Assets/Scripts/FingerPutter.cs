using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FingerPutter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fingerPrint;
    [SerializeField] private Key key;
    private Color initcolor;
    void Awake()=>initcolor = fingerPrint.color;
    void Update(){
        if(Keyboard.current[key].wasPressedThisFrame){
            fingerPrint.color = Color.red;
            EventHandler.Call_OnPutOnFingers(true);
        }
        else if(Keyboard.current[key].wasReleasedThisFrame){
            fingerPrint.color = initcolor;
            EventHandler.Call_OnPutOnFingers(false);
        }
    }
}
