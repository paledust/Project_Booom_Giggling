using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class DragManager : Singleton<DragManager>
{
    public PaperControl CurrentPaperControl{get{return currentPaperControl;}}
    [SerializeField] private PlayerInput input;
    [SerializeField] private DragLamp dragLamp;
    [SerializeField] private AudioSource tearAudio;
    public Transform rightHandReadyTrans;
    public Transform leftHandReadyTrans;
    private PaperControl currentPaperControl;
    private TearPaperPoint currentTearingPoint;
    public float tearingProgress{get{
        if(currentTearingPoint==null) return 0;
        else return currentTearingPoint.Progress;
    }}
    private bool canTear = false;
    private int paperIndex = 0;
    private INTERACTION_TYPE interactionType;
    float speed = 0;
    void OnEnable(){
        EventHandler.E_OnStartANewPaper += SetCurrentPaperControl;
        EventHandler.E_OnFinishCurrentTear += FinishCurrentPaper;
        EventHandler.E_OnQuit += StopControl;
    }
    void OnDisable(){
        EventHandler.E_OnStartANewPaper -= SetCurrentPaperControl;
        EventHandler.E_OnFinishCurrentTear -= FinishCurrentPaper;
        EventHandler.E_OnQuit -= StopControl;
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
            if(hit!=null){
                switch(hit.tag){
                    case "Paper":
                        interactionType = INTERACTION_TYPE.PAPER;
                        if(canTear){
                            currentTearingPoint = hit.GetComponent<TearPaperPoint>();
                            currentTearingPoint.StartDragThisPoint();
                        }
                        break;
                    case "Lamp":
                        interactionType = INTERACTION_TYPE.LAMP;
                        dragLamp.OnDrag();
                        break;
                }
            }
        }
        else{
            switch(interactionType){
                case INTERACTION_TYPE.PAPER:
                    currentTearingPoint?.ReleaseThisPoint();
                    currentTearingPoint = null;
                    break;
                case INTERACTION_TYPE.LAMP:
                    dragLamp.OnReleased();
                    break;
            }
            interactionType = INTERACTION_TYPE.NONE;
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
        switch(interactionType){
            case INTERACTION_TYPE.PAPER:
                if(canTear){
                    currentTearingPoint?.MoveTheTearPointToMousePos(value.Get<Vector2>());
                }
                break;
            case INTERACTION_TYPE.LAMP:
                dragLamp.MoveLampRope(value.Get<Vector2>());
                break;
        }
    }
    void StopControl(){
        input.actions.Disable();
    }
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);
}
