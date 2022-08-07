using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTearing : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private Animation enterAnimation;
    [SerializeField] private AnimationClip exitClip;
    [SerializeField] private PaperControl file;
    void OnEnable(){
        enterAnimation.Play();
        EventHandler.E_OnFinishCurrentTear += PlayFinalAnimation;
    }
    void OnDisable(){
        EventHandler.E_OnFinishCurrentTear -= PlayFinalAnimation;
    }
    public void StartInteraction(){
        EventHandler.Call_OnSwitchHand(true);
        file.StartThisPaper();
    }
    public void FinishInteraction(){
        gameController.GoToTearPhotoInteraction();
    }
    void PlayFinalAnimation(PaperControl paper){
        if(paper == file){
            this.enabled = false;
            StartCoroutine(coroutinePlayFinalAnimation());
        }
    }
    IEnumerator coroutinePlayFinalAnimation(){
        EventHandler.Call_OnSwitchHand(false);
        yield return new WaitForSeconds(2);
        enterAnimation.Play(exitClip.name);
    }
}
