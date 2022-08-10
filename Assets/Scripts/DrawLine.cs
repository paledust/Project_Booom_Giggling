using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private Transform[] segements;
    [SerializeField] private CS_Bezier_Quadratic_Class curve;
    [SerializeField] private int Resolution = 10;
    private LineRenderer m_line;
    private Vector3[] linePoints;
    void Awake(){
        m_line = GetComponent<LineRenderer>();
        curve.SetPointCount(0);
        for(int i=0; i<segements.Length; i++){
            curve.AddPoint(segements[i].position);
        }

        linePoints = new Vector3[segements.Length*Resolution];
        for(int i=0; i<linePoints.Length; i++){
            float t = (i+0.0f)/(linePoints.Length-1.0f);
            linePoints[i] = curve.GetPoint(t, null);
        }

        m_line.positionCount = segements.Length * Resolution;
        m_line.SetPositions(linePoints);
    }
    void Update(){
        for(int i=0; i<linePoints.Length; i++){
            float t = (i+0.0f)/(linePoints.Length-1.0f);
            linePoints[i] = curve.GetPoint(t, null);
        }
        m_line.SetPositions(linePoints);
    }
    void FixedUpdate(){
        for(int i=0; i<segements.Length; i++){
            curve.SetControlPoint(i, segements[i].position);
        }
    }
}
