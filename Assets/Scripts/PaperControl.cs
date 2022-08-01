using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperControl : MonoBehaviour
{
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TearPaperPoint tearPaperPoint;
    public Transform LeftHandTarget{get{return leftHandTarget;}}
    public void StartThisPaper(){
        EventHandler.Call_OnStartANewPaper(this);
        tearPaperPoint.gameObject.SetActive(true);
    }
    public void OnFinishThisPaper(){
        gameObject.SetActive(false);
    }
    public void ShowLeftPaper(){
        gameObject.SetActive(true);
    }
}
