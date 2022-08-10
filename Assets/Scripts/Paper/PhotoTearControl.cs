using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTearControl : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private PaperControl[] paperControls;
    [SerializeField] private Animation photoTearAnimation;
    [SerializeField] private AudioSource giggleAudio;
    [SerializeField] private AudioClip[] giggleClips;
    [SerializeField, Range(0,1)] private float giggleChance;
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
        if(Random.value<giggleChance){
            giggleAudio.PlayRandomClipFromClips(giggleClips);
        }
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
    // [ContextMenu("Test")]
    // public void Test(){
    //     StartCoroutine(CoroutineStackUpAllPaper());
    // }
    IEnumerator CoroutineGoToNextPaper(){
        paperControls[paperIndex].OnFinishThisPaper();
        yield return null;
        paperIndex ++;
        paperControls[paperIndex].StartThisPaper();
    }
    IEnumerator CoroutineStackUpAllPaper(){
        EventHandler.Call_OnResetHand();
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
