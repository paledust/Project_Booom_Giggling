using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class TearPaperManager : Singleton<TearPaperManager>
{
    public PaperControl CurrentPaperControl{get{return currentPaperControl;}}
    [SerializeField] private AudioSource tearAudio;
    private PaperControl currentPaperControl;
    private TearPaperPoint currentTearingPoint;
    public float tearingProgress{get{
        if(currentTearingPoint==null) return 0;
        else return currentTearingPoint.Progress;
    }}
    private bool canTear = false;
    private int paperIndex = 0;
    float speed = 0;
    void OnEnable(){
        EventHandler.E_OnStartANewPaper += SetCurrentPaperControl;
        EventHandler.E_OnFinishCurrentTear += FinishCurrentPaper;
    }
    void OnDisable(){
        EventHandler.E_OnStartANewPaper += SetCurrentPaperControl;
        EventHandler.E_OnFinishCurrentTear -= FinishCurrentPaper;
    }
    void Update(){
        float inputSpeed = Mouse.current.delta.ReadValue().magnitude;
        speed = Mathf.Lerp(speed, inputSpeed, Time.deltaTime * 50);
        if(Mathf.Abs(speed - inputSpeed)<=0.01f){
            speed = inputSpeed;
        }
        if(speed == 0){
            StopPlayingTearLoop();
        }
    }
    public void SetCanTear(bool value){
        canTear = value;
        if(!canTear){
            ReleaseCurrentPoint();
        }
    }
    public void StartPlayingTearLoop(){
        if(!tearAudio.isPlaying) {
            float clipLength = tearAudio.clip.length;
            tearAudio.time = Random.Range(0f, clipLength);
            tearAudio.Play();
        }
    }
    public void StopPlayingTearLoop(){
        if(tearAudio.isPlaying) tearAudio.Stop();
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
    void SetCurrentPaperControl(PaperControl paper){
        currentPaperControl = paper;
    }
    void FinishCurrentPaper(PaperControl paper){
        ReleaseCurrentPoint();
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
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);
}
