using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : Singleton<TearPaperManager>
{
    [SerializeField] private TearPaperPoint[] tearPapers;
    [SerializeField] private TearPaperPoint currentTearingPoint;
    [SerializeField] private float tearSpeed;
    private bool canTear = false;
    private int paperIndex = 0;
    void OnEnable(){
        tearPapers[paperIndex].gameObject.SetActive(true);
        EventHandler.E_OnFinishCurrentTear += FinishCurrentPaper;
    }
    void OnDisable(){
        EventHandler.E_OnFinishCurrentTear -= FinishCurrentPaper;
    }
    public void SetCanTear(bool value){
        canTear = value;
        if(!canTear){
            ReleaseCurrentPoint();
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
    void FinishCurrentPaper(){
        ReleaseCurrentPoint();
        if(paperIndex<tearPapers.Length-1){
            StartCoroutine(CoroutineGoToNextPaper());
        }
    }
    IEnumerator CoroutineGoToNextPaper(){
        tearPapers[paperIndex].transform.parent.gameObject.SetActive(false);
        yield return null;
        paperIndex ++;
        tearPapers[paperIndex].gameObject.SetActive(true);
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
