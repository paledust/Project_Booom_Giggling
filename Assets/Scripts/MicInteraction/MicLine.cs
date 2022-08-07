using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class MicLine : MonoBehaviour
{
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField] private int pointIndex = 2;
    [SerializeField] private Transform micTrans;
    void Update(){
        spriteShapeController.spline.SetPosition(pointIndex, transform.InverseTransformPoint(micTrans.position));
    }
}
