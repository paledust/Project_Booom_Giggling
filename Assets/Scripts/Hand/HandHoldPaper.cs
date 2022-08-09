using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHoldPaper : MonoBehaviour
{
    [SerializeField] private FingerPutter[] fingers;
    [SerializeField] private int fingerCounter = 0;
    [SerializeField] private SpriteRenderer palmRenderer;
    [SerializeField] private GameObject handPressed;
    [SerializeField] private AudioSource putFingerAudio;
    [SerializeField] private AudioClip fingerClip;
    [SerializeField] private AudioClip pressOnClip;
    private Transform target;
    private bool allFingerOn = false;
    private bool handOnPos = false;
    void OnEnable(){
        EventHandler.E_OnPutOnFingers += TestFingers;
        EventHandler.E_OnStartANewPaper += RefreshHandTarget;
        EventHandler.E_OnResetHand      += ResetHand;
    }
    void OnDisable(){
        EventHandler.E_OnPutOnFingers -= TestFingers;
        EventHandler.E_OnStartANewPaper -= RefreshHandTarget;
        EventHandler.E_OnResetHand      -= ResetHand;
    }
    public void TestFingers(bool putOnFinger){
        if(putOnFinger){
            fingerCounter ++;
            putFingerAudio.pitch = Random.Range(0.9f,1.1f);
            putFingerAudio.PlayOneShot(fingerClip);
            if(!handOnPos){
                handOnPos = false;
                StartCoroutine(coroutineMoveToTargetPos(TearPaperManager.Instance.CurrentPaperControl.LeftHandTarget));
            }
        }
        else{
            fingerCounter --;
        }

        if(fingerCounter == fingers.Length){
            if(!allFingerOn){
                allFingerOn = true;
                palmRenderer.enabled = false;
                foreach(var finger in fingers){
                    finger.SwitchSpriteRender(false);
                }
                handPressed.SetActive(true);
                putFingerAudio.PlayOneShot(pressOnClip);
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }
        else{
            if(allFingerOn){
                allFingerOn = false;
                palmRenderer.enabled = true;
                foreach(var finger in fingers){
                    finger.SwitchSpriteRender(true);
                }
                handPressed.SetActive(false);
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }        
    }
    void RefreshHandTarget(PaperControl paper){
        handOnPos = false;
    }
    void ResetHand(){
        StartCoroutine(coroutineMoveToTargetPos(TearPaperManager.Instance.rightHandReadyTrans));
    }
    IEnumerator coroutineMoveToTargetPos(Transform targetTrans){        
        float lerp = 0;
        Vector3 initpos = transform.position;
        Quaternion initRot = transform.rotation;

        for(float t=0; t<1; t+=Time.deltaTime*8){
            lerp = Mathf.Lerp(0, 1, EasingFunc.Easing.QuadEaseOut(t));
            transform.position = Vector3.Lerp(initpos, targetTrans.position, lerp);
            transform.rotation = Quaternion.Lerp(initRot, targetTrans.rotation, lerp);
            yield return null;
        }
        transform.position = targetTrans.position;
        transform.rotation = targetTrans.rotation;
    }
}
