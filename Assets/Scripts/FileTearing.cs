using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileTearing : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightMaskSprite;
    [SerializeField] private Color lightsUpColor;
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject LampStart;
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
        LampStart.SetActive(true);
    }
    public void StartFileTearing(){
        file.StartThisPaper();
        StartCoroutine(coroutineLightsUp());
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
    IEnumerator coroutineLightsUp(){
        Color initColor = lightMaskSprite.color;
        for(float t=0; t<1; t+=Time.deltaTime*10){
            lightMaskSprite.color = Color.Lerp(initColor, lightsUpColor, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        Shader.SetGlobalInt("_USE_PURE_COLOR", 0);
        LampStart.SetActive(false);
    }
}
