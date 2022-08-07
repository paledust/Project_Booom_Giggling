using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicRightHandController : MonoBehaviour
{
    [SerializeField] private RIGHT_HAND_STATE handState = RIGHT_HAND_STATE.RELEASED;
    public Sprite sprite1;
    public Sprite sprite2;
    private SpriteRenderer spriteRenderer;
    private Transform snapTarget;
    void Awake(){spriteRenderer = GetComponent<SpriteRenderer>();}
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
        }
    }
    public void SnapRightHandToTarget(Transform target){
        if(target!=null) StartCoroutine(coroutineSnapHand(target));
        else snapTarget = null;
    }
    IEnumerator coroutineSnapHand(Transform target){
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
