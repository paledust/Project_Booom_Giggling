using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : MonoBehaviour
{
    [SerializeField] private FingerPutter[] fingerPutters;
    [SerializeField] private TearPaperPoint currentTearingPoint;
    [SerializeField] private float tearSpeed;
    private bool canTear = false;
    [SerializeField] int counter = 0;
    void Awake(){
        fingerPutters = FindObjectsOfType<FingerPutter>();
    }
    void OnEnable(){
        EventHandler.E_OnPutOnFingers += SwitchTearing;
        EventHandler.E_OnFinishCurrentTear += ReleaseCurrentPoint;
    }
    void OnDisable(){
        EventHandler.E_OnPutOnFingers -= SwitchTearing;
        EventHandler.E_OnFinishCurrentTear -= ReleaseCurrentPoint;
    }
    void SwitchTearing(bool putOnFinger){
        if(putOnFinger){
            counter ++;
        }
        else{
            counter --;
        }

        if(counter == fingerPutters.Length){
            canTear = true;
        }
        else{
            canTear = false;
        }
    }
    void OnGrab(InputValue value){
        if(value.isPressed){
            Vector3 mousePoint = GameManager.mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapCircle(mousePoint, 0.3f, Service.InteractableLayer);
            if(hit!=null){
                currentTearingPoint = hit.GetComponent<TearPaperPoint>();
                currentTearingPoint.StartDragThisPoint();
            }
        }
        else{
            currentTearingPoint?.ReleaseThisPoint();
            currentTearingPoint = null;
        }
    }
    void ReleaseCurrentPoint(){
        currentTearingPoint?.ReleaseThisPoint();
        currentTearingPoint = null;        
    }
    // void OnMouseMove(InputValue value){
    //     if(canTear){
    //         currentTearingPoint?.MoveTheTearPoint(value.Get<Vector2>() * tearSpeed);
    //     }
    // }
    void OnMousePosition(InputValue value){
        if(canTear){
            currentTearingPoint?.MoveTheTearPointToMousePos(value.Get<Vector2>());
        }
    }
}
