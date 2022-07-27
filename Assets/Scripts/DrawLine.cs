using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private Transform target;
    private LineRenderer m_line;
    void Awake(){
        m_line = GetComponent<LineRenderer>();
    }
    void Update(){
        Vector3 pos = (transform.position + target.position)/2.0f;
        m_line.SetPosition(1, transform.InverseTransformPoint(pos));
    }
}
