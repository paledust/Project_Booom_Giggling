using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperControl : MonoBehaviour
{
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TearPaperPoint[] tearPaperPoints;
    [SerializeField] private Texture2D tearTexture;
    [SerializeField] private SpriteRenderer paperStay;
    private SpriteRenderer spriteRenderer;
    public Transform LeftHandTarget{get{return leftHandTarget;}}
    void Awake() { spriteRenderer = GetComponent<SpriteRenderer>(); }
    void Start()
    {
        Vector3 paperScale = transform.localScale;
        for(int i = 2;i >= 1; i++)
        {
            transform.localScale.Set(i, i, 0f);
        }


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
    public void ShowLeftPaper(){
        gameObject.SetActive(true);
    }

    // IEnumerator PaperFadeOut(Transform target)
    // {


    //     spriteRenderer.color = new Color(
    //         spriteRenderer.color.r,
    //         spriteRenderer.color.g, 
    //         spriteRenderer.color.b,
    //         )
    // }
}
