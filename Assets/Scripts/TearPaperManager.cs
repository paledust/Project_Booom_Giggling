using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : MonoBehaviour
{
    [SerializeField] private TearPaperPoint currentTearingPoint;
    [SerializeField] private float tearSpeed;
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
    void OnMouseMove(InputValue value){
        currentTearingPoint?.MoveTheTearPoint(value.Get<Vector2>() * tearSpeed);
    }
}
