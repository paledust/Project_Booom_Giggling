using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperControl : MonoBehaviour
{
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TearPaperPoint[] tearPaperPoints;
    [SerializeField] private Texture2D tearTexture;
    [SerializeField] private SpriteRenderer paperStay;
    public Transform LeftHandTarget{get{return leftHandTarget;}}
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
    public void ShowLeftPaper(){
        gameObject.SetActive(true);
    }
}
