using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : MonoBehaviour
{
    [SerializeField] private TearPaperPoint currentTearingPoint;
    void Update(){
        if(Mouse.current.leftButton.wasPressedThisFrame){
            Vector3 mousePoint = GameManager.mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapCircle(mousePoint, 0.3f, Service.InteractableLayer);
            if(hit!=null){
                currentTearingPoint = hit.GetComponent<TearPaperPoint>();
                currentTearingPoint.StartDragThisPoint();
            }
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame){
            currentTearingPoint?.ReleaseThisPoint();
        }
    }
}
