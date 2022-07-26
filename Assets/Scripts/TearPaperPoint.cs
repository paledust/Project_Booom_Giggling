using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearPaperPoint : MonoBehaviour
{
    [SerializeField] private float TearLength;
[Header("纸张")]
    [SerializeField] private Transform paper_tear_reversed;
[Header("反馈")]
    [SerializeField] private SpriteRenderer fingerPrint;
    [SerializeField] private float endRange = 0.1f;
    private Color initcolor;
    private Vector3 initPos;
    private Vector3 tearDir;
    private Vector3 endPos;
    Vector3 initPosToPaper;
    bool initialized;
    void Awake(){
        initPos = transform.position;
        tearDir = transform.right.normalized;
        endPos = transform.position + transform.right * TearLength;
        initcolor = fingerPrint.color;
        initialized = true;
    }
    void OnEnable(){
        initPosToPaper = paper_tear_reversed.position - transform.position;
    }
    public void StartDragThisPoint(){
        fingerPrint.color = Color.red;
    }
    public void ReleaseThisPoint(){
        fingerPrint.color = initcolor;
    }
    public void MoveTheTearPoint(Vector2 inputDirection){
        transform.position += Mathf.Max(0, Vector2.Dot(inputDirection, tearDir)) * tearDir * Time.deltaTime;
        paper_tear_reversed.position = transform.position + initPosToPaper;
        
        Vector3 foldPoint = (initPos + transform.position)/2f;
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(foldPoint.x, foldPoint.y, foldPoint.z, 1));

        Vector3 diff = endPos - transform.position;
        if(Vector3.Dot(diff, tearDir)<endRange){
            
        }
    }
    void OnDrawGizmosSelected(){
        if(initialized){
            Gizmos.DrawLine(transform.position, endPos);
            Gizmos.DrawWireSphere(endPos, endRange);
        }
        else{
            Vector3 endPos = transform.position + transform.right * TearLength;
            Gizmos.DrawLine(transform.position, endPos);
            Gizmos.DrawWireSphere(endPos, endRange);
        }
    }
}
