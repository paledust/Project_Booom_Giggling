using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RighthandController : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion lastRot;
    Vector3 lastPos;
    public Sprite sprite1;
    public Sprite sprite2;
    private int handState = 0; 



    // Start is called before the first frame update
    Transform snapTarget;
    void OnEnable(){EventHandler.E_OnReadyToTear += SnapRightHand;}
    void OnDisable(){EventHandler.E_OnReadyToTear -= SnapRightHand;}
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
        Vector3 m_MousePos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, pos.z);
        transform.position = GameManager.mainCam.ScreenToWorldPoint(m_MousePos);
    }
    
    void HandStateChange(){
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if(Mouse.current.leftButton.isPressed){
            handState = 1;
        }else handState = 0;

        if (handState == 0) {
            renderer.sprite = this.sprite1;
        } else renderer.sprite = this.sprite2;
    
    }


    //     transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // }
    void SnapToTarget(){
        if(snapTarget != null){
            Vector3 targetPos = snapTarget.position;
            targetPos.z = transform.position.z;
            transform.position = targetPos;
            transform.rotation = Quaternion.Euler(0,0,Mathf.Lerp(70,110,TearPaperManager.Instance.tearingProgress)) * snapTarget.rotation;
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
    IEnumerator coroutineSnapHand(Transform target){
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
}
