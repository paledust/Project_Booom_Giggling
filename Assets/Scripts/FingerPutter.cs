using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerPutter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fingerPrint;
    [SerializeField] private KeyCode key;
    private Color initcolor;
    void Awake(){
        initcolor = fingerPrint.color;
    }
    void Update(){
        if(Input.GetKeyDown(key)){
            fingerPrint.color = Color.red;
        }
        if(Input.GetKeyUp(key)){
            fingerPrint.color = initcolor;
        }
    }
}
