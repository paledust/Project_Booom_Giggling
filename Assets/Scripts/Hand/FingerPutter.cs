using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FingerPutter : MonoBehaviour
{
    [SerializeField] private float ScaledDown = 0.9f;
    [SerializeField] private Key key;
    private SpriteRenderer spriteRenderer;
    private Vector3 initScale;
    private bool pressed = false;
    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        initScale = transform.localScale;
    }
    void Update(){
        if(Keyboard.current[key].isPressed){
            if(!pressed){
                pressed = true;
                Vector3 scale = initScale;
                scale.y *= ScaledDown;
                transform.localScale = scale;
                EventHandler.Call_OnPutOnFingers(true);
            }
        }
        else if(pressed){
            pressed = false;
            transform.localScale = initScale;
            EventHandler.Call_OnPutOnFingers(false);
        }
    }
    public void SwitchSpriteRender(bool isOn){
        spriteRenderer.enabled = isOn;
    }
}
