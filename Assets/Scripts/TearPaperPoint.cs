using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearPaperPoint : MonoBehaviour
{
    [SerializeField] private float TearLength;
[Header("纸张")]
    [SerializeField] private SpriteRenderer paper_tear_reversed;
    [SerializeField] private float MaxOffset = 3;
[Header("反馈")]
    [SerializeField] private SpriteRenderer fingerPrint;
    [SerializeField] private float endRange = 0.1f;
    private Color initcolor;
    private Vector3 initPos;
    private Vector3 tearDir;
    private Vector3 tearNormalDir;
    private Vector3 endPos;
    private Vector3 initPosToPaper;
    private Vector3 tearPos;
    private Quaternion tearpaper_initRot;
    private float offset;
    bool initialized;
    public static int AngleID = Shader.PropertyToID("_FoldAngle");
    void Awake(){
        tearPos = initPos = transform.position;
        tearpaper_initRot = paper_tear_reversed.transform.rotation;
        tearDir = transform.right.normalized;
        tearNormalDir = transform.up.normalized;
        endPos = transform.position + transform.right * TearLength;
        initcolor = fingerPrint.color;
        initialized = true;
    }
    void OnEnable(){
        initPosToPaper = paper_tear_reversed.transform.position - transform.position;
    }
    void OnDisable(){
        // paper_tear_reversed.sharedMaterial.SetFloat(AngleID, 0);
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(initPos.x, initPos.y, initPos.z, 1));
    }
    public void StartDragThisPoint(){
        fingerPrint.color = Color.red;
    }
    public void ReleaseThisPoint(){
        fingerPrint.color = initcolor;
    }
    public void MoveTheTearPoint(Vector2 inputDirection){
        float T = Vector2.Dot(inputDirection, tearDir);
        float N = Vector3.Dot(inputDirection, tearNormalDir);
        offset += N * Time.deltaTime;
        offset = Mathf.Clamp(offset, -MaxOffset, MaxOffset);

        tearPos += Mathf.Max(0, T) * tearDir * Time.deltaTime;
        transform.position = tearPos + offset * tearNormalDir;

        Vector3 foldPoint = (initPos + transform.position)/2f;

        float angle = Vector2.SignedAngle(tearDir, transform.position - initPos);
        angle = Mathf.Clamp(angle, -5, 5);

        Vector3 tear_fold_diff = paper_tear_reversed.transform.position - foldPoint;
        paper_tear_reversed.transform.position = tearPos + initPosToPaper;
        paper_tear_reversed.transform.rotation = Quaternion.Euler(0, 0, angle) * tearpaper_initRot;

        paper_tear_reversed.sharedMaterial.SetFloat(AngleID, -angle);
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
