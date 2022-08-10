using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RighthandController : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion lastRot;
    Vector3 lastPos;
    [SerializeField] private RIGHT_HAND_STATE handState = RIGHT_HAND_STATE.RELEASED;
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private AudioClip wooshClip;
    public Sprite sprite1;
    public Sprite sprite2;
    private SpriteRenderer spriteRenderer;
    private Transform snapTarget;
    private bool RotateAlong = true;
    void Awake(){spriteRenderer = GetComponent<SpriteRenderer>();}
    void OnEnable(){
        EventHandler.E_OnReadyToTear += SnapRightHand;
        EventHandler.E_OnSnapToStuff += SnapRightHandToTarget;
        EventHandler.E_OnResetHand   += ResetHand;
    }
    void OnDisable(){
        EventHandler.E_OnReadyToTear -= SnapRightHand;
        EventHandler.E_OnResetHand   -= ResetHand;
        EventHandler.E_OnSnapToStuff -= SnapRightHandToTarget;
    }
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        RighthandMove();
        SnapToTarget();

        HandStateChange();
    }

    void RighthandMove()
    {
        Vector3 pos = GameManager.mainCam.WorldToScreenPoint(transform.position);
        Vector3 mousePos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, pos.z);
        transform.position = GameManager.mainCam.ScreenToWorldPoint(mousePos);
    }
    
    void HandStateChange(){
        if(Mouse.current.leftButton.isPressed){
            handState = RIGHT_HAND_STATE.HOLD;
            spriteRenderer.sprite = this.sprite2;
        }else if(Mouse.current.leftButton.wasReleasedThisFrame) {
            handState = RIGHT_HAND_STATE.RELEASED;
            spriteRenderer.sprite = this.sprite1;
        }
    }
    void SnapToTarget(){
        if(snapTarget != null){
            Vector3 targetPos = snapTarget.position;
            targetPos.z = transform.position.z;
            transform.position = targetPos;
            if(RotateAlong) transform.rotation = Quaternion.Euler(0,0,Mathf.Lerp(70,110,DragManager.Instance.tearingProgress)) * snapTarget.rotation;
        }
    }
    void SnapRightHand(TearPaperPoint tearPoint, bool isReady){
        if(isReady){
            if(tearPoint == null){
                return;
            }
            StartCoroutine(coroutineSnapHand(tearPoint.transform));
        }
        else{
            snapTarget = null;
        }
    }
    public void SnapRightHandToTarget(Transform target){
        if(target!=null) {
            StartCoroutine(coroutineSnapHand_PositionOnly(target));
            RotateAlong = false;
        }
        else snapTarget = null;
    }
    void ResetHand(){
        StartCoroutine(coroutineResetRightHand());
    }
    IEnumerator coroutineResetRightHand(){
        Quaternion initRot = transform.rotation;

        for(float t=0; t<1; t+=Time.deltaTime*8){
            transform.rotation = Quaternion.Lerp(initRot, Quaternion.identity, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        transform.rotation = Quaternion.identity;

        yield return null;        
    }
    IEnumerator coroutineSnapHand(Transform target){
        m_audio.PlayOneShot(wooshClip);
        Vector3 initPos   = transform.position;
        Quaternion initRot = transform.rotation;

        for(float t=0; t<1; t+=Time.deltaTime*8){
            Vector3 targetPos = target.position;
            targetPos.z = initPos.z;
            transform.position = Vector3.Lerp(initPos, targetPos, EasingFunc.Easing.QuadEaseOut(t));
            transform.rotation = Quaternion.Lerp(initRot, Quaternion.Euler(0,0,70) * target.rotation, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        transform.position = target.position;
        snapTarget = target;

        yield return null;
    }
    IEnumerator coroutineSnapHand_PositionOnly(Transform target){
        Vector3 initPos   = transform.position;

        for(float t=0; t<1; t+=Time.deltaTime*8){
            Vector3 targetPos = target.position;
            targetPos.z = initPos.z;
            transform.position = Vector3.Lerp(initPos, targetPos, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        transform.position = target.position;
        snapTarget = target;

        yield return null;
    }
}
