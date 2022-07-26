using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearPaperPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fingerPrint;
    private Color initcolor;
    void Awake()=>initcolor = fingerPrint.color;
    public void StartDragThisPoint(){
        fingerPrint.color = Color.red;
    }
    public void ReleaseThisPoint(){
        fingerPrint.color = initcolor;
    }
    public void MoveTheTearPoint(){
        
    }
}
