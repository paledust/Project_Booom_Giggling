using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : Singleton<TearPaperManager>
{
    [SerializeField] private PaperControl[] paperControls;
    [SerializeField] private TearPaperPoint currentTearingPoint;
    public float tearingProgress{get{
        if(currentTearingPoint==null) return 0;
        else return currentTearingPoint.Progress;
    }}
    // [SerializeField] private float tearSpeed;
    private bool canTear = false;
    private int paperIndex = 0;
    void OnEnable(){
        EventHandler.E_OnFinishCurrentTear += FinishCurrentPaper;
    }
    void OnDisable(){
        EventHandler.E_OnFinishCurrentTear -= FinishCurrentPaper;
    }
    void Start(){
        paperControls[paperIndex].StartThisPaper();
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
            if(hit!=null && canTear){
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
        if(paperIndex<paperControls.Length-1){
            StartCoroutine(CoroutineGoToNextPaper());
        }
        else{
            StartCoroutine(CoroutineStackUpAllPaper());
        }
    }
    void ReleaseCurrentPoint(){
        currentTearingPoint?.ReleaseThisPoint();
        currentTearingPoint = null;        
    }
    void OnMousePosition(InputValue value){
        if(canTear){
            currentTearingPoint?.MoveTheTearPointToMousePos(value.Get<Vector2>());
        }
    }
    IEnumerator CoroutineGoToNextPaper(){
        paperControls[paperIndex].OnFinishThisPaper();
        yield return null;
        paperIndex ++;
        paperControls[paperIndex].StartThisPaper();
    }
    IEnumerator CoroutineStackUpAllPaper(){
        for(;paperIndex>=0;paperIndex--){
            paperControls[paperIndex].ShowLeftPaper();
            yield return new WaitForSeconds(0.2f);
        }
    }
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);
}
