using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHoldPaper : MonoBehaviour
{
    [SerializeField] private FingerPutter[] fingers;
    [SerializeField] private Transform target;
    private int fingerCounter = 0;
    private bool allFingerOn = false;
    private bool handOnPos = false;
    void OnEnable(){
        EventHandler.E_OnPutOnFingers += TestFingers;
        EventHandler.E_OnStartANewPaper += RefreshHandTarget;
    }
    void OnDisable(){
        EventHandler.E_OnPutOnFingers -= TestFingers;
        EventHandler.E_OnStartANewPaper -= RefreshHandTarget;
    }
    public void TestFingers(bool putOnFinger){
        if(putOnFinger){
            fingerCounter ++;
            if(!handOnPos){
                handOnPos = false;
                StartCoroutine(coroutineMoveToTargetPos());
            }
        }
        else{
            fingerCounter --;
        }

        if(fingerCounter == fingers.Length){
            if(!allFingerOn){
                allFingerOn = true;
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }
        else{
            if(allFingerOn){
                allFingerOn = false;
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }        
    }
    void RefreshHandTarget(PaperControl paper){
        handOnPos = false;
        Debug.Log("ASDF");
        target = paper.LeftHandTarget;
    }
    IEnumerator coroutineMoveToTargetPos(){        
        float lerp = 0;
        Vector3 initpos = transform.position;
        Quaternion initRot = transform.rotation;
        for(float t=0; t<1; t+=Time.deltaTime*8){
            lerp = Mathf.Lerp(0, 1, EasingFunc.Easing.QuadEaseOut(t));
            transform.position = Vector3.Lerp(initpos, target.position, lerp);
            transform.rotation = Quaternion.Lerp(initRot, target.rotation, lerp);
            yield return null;
        }
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
