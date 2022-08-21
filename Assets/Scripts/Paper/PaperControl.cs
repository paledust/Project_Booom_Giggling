using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperControl : MonoBehaviour
{
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TearPaperPoint[] tearPaperPoints;
    [SerializeField] private Texture2D tearTexture;
    [SerializeField] private SpriteRenderer paperStay;
    private float throwOutTime = .5f;
    public Transform LeftHandTarget{get{return leftHandTarget;}}
    private SpriteRenderer spriteRenderer;
    void Awake() {spriteRenderer = GetComponent<SpriteRenderer>();}
    public void FinishThisPaper(){
        EventHandler.Call_OnFinishCurrentTear(this);
    }
    public void StartThisPaper(){
        EventHandler.Call_OnStartANewPaper(this);
        paperStay.material.SetTexture("_TearMask", tearTexture);
        foreach(var tearPoint in tearPaperPoints){
            tearPoint.SetUpTextures(tearTexture);
            tearPoint.gameObject.SetActive(true);
        }
    }
    public void ChoiseThisTearPoint(TearPaperPoint tearPoint){
        foreach(var point in tearPaperPoints){
            if(point!=tearPoint){
                point.gameObject.SetActive(false);
            }
        }
    }
    public void OnFinishThisPaper(){
        gameObject.SetActive(false);
    }
    public IEnumerator FinishingThisPaper(){
        if(throwOutTime==0){
            gameObject.SetActive(false);
        }
        else{
            Vector3 initpos = transform.position;
            Quaternion initRot = transform.rotation;
            Vector3 throwPos = initpos + (Vector3)Random.insideUnitCircle.normalized*5f;
            Quaternion throwRot = transform.rotation*Quaternion.Euler(0,0,Random.Range(-40,40));

            Color initColor = paperStay.color;
            Color clearColor = initColor;
            clearColor.a = 0;

            for(float t=0; t<1; t+=Time.deltaTime/throwOutTime){
                paperStay.color = Color.Lerp(initColor, clearColor, EasingFunc.Easing.SmoothInOut(t));
                transform.position = Vector3.Lerp(initpos, throwPos, EasingFunc.Easing.SmoothInOut(t));
                transform.rotation = Quaternion.Lerp(initRot, throwRot, EasingFunc.Easing.SmoothInOut(t));
                yield return null;
            }
            paperStay.color = clearColor;
            gameObject.SetActive(false);

            transform.position = initpos;
            transform.rotation = initRot;
        }
    }
    public IEnumerator coroutinePaperFlyIn(){
        Quaternion initRot = Quaternion.Euler(0,0, Random.Range(30f,60f) * (Random.Range(0,2)*2-1));

        Vector3 initScale = paperStay.transform.localScale;
        Vector3 resizeScale = initScale * 5;

        Color initColor = paperStay.color;
        Color clearColor = initColor;
        initColor.a = 1;
        clearColor.a = 0;
        paperStay.transform.localScale = resizeScale;
        paperStay.transform.localRotation = initRot;

        gameObject.SetActive(true);
        for(float t=0; t<1; t+=Time.deltaTime * 0.6f){
            paperStay.color = Color.Lerp(clearColor, initColor, EasingFunc.Easing.SmoothInOut(t));
            paperStay.transform.localRotation = Quaternion.Lerp(initRot, Quaternion.identity, EasingFunc.Easing.SmoothInOut(t));
            paperStay.transform.localScale = Vector3.LerpUnclamped(resizeScale, initScale, EasingFunc.Easing.SmoothInOut(Mathf.Clamp01(t+0.15f)));
            yield return null;
        }
        paperStay.color = initColor;
        paperStay.transform.localScale = initScale;
        paperStay.transform.localRotation = Quaternion.identity;
    }
}
