using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTearControl : MonoBehaviour
{
    [SerializeField] private PaperControl[] paperControls;
    [SerializeField] private Animation photoTearAnimation;
    [SerializeField] private AudioSource giggleAudio;
    [SerializeField] private AudioClip[] giggleClips;
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
        giggleAudio.PlayRandomClipFromClips(giggleClips);
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
        EventHandler.Call_OnSwitchHand(false);
        for(;paperIndex>=0;paperIndex--){
            paperControls[paperIndex].ShowLeftPaper();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
