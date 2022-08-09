using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTearing : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private AudioSource pourAudio;
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
    void PlayPourSound(){
        pourAudio.Play();
    }
    void PlayClip(AudioClip clip){
        pourAudio.PlayOneShot(clip);
    }
    IEnumerator coroutinePlayFinalAnimation(){
        EventHandler.Call_OnSwitchHand(false);
        yield return new WaitForSeconds(.5f);
        enterAnimation.Play(exitClip.name);
    }
}
