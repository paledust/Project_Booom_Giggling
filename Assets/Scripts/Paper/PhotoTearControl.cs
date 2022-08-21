using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTearControl : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private PaperControl[] paperControls;
    [SerializeField] private Animation photoTearAnimation;
    [SerializeField] private AudioSource tearAudio;
    [SerializeField] private AudioClip[] tearClips;
    private int paperIndex = 0;
    void Awake(){paperControls = GetComponentsInChildren<PaperControl>();}
    void OnEnable(){
        EventHandler.E_OnFinishCurrentTear += FinishCurrentPaper;
        photoTearAnimation.Play();
    }
    void OnDisable(){
        EventHandler.E_OnFinishCurrentTear -= FinishCurrentPaper;
    }
    void FinishCurrentPaper(PaperControl paper){
        if(paperIndex<paperControls.Length-1){
            StartCoroutine(CoroutineGoToNextPaper());
        }
        else{
            StartCoroutine(CoroutineStackUpAllPaper());
        }
    }
    public void StartInteraction(){
        EventHandler.Call_OnSwitchHand(true);
        paperControls[paperIndex].StartThisPaper();
    }
    IEnumerator CoroutineGoToNextPaper(){
        paperControls[paperIndex].OnFinishThisPaper();
        yield return null;
        paperIndex ++;
        paperControls[paperIndex].StartThisPaper();
    }
    IEnumerator CoroutineStackUpAllPaper(){
        EventHandler.Call_OnResetHand();
        EventHandler.Call_OnNextSmileValue();
        float waitTime = 2f;
        paperIndex --;
        for(;paperIndex>=0;paperIndex--){
            StartCoroutine(paperControls[paperIndex].coroutinePaperFlyIn());
            yield return new WaitForSeconds(waitTime);
            waitTime -= 0.25f;
            waitTime = Mathf.Max(0.6f, waitTime);
        }
        gameController.GoToLampInteraction();
    }
}
