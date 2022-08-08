using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearPaperPoint : MonoBehaviour
{
    [SerializeField] private PaperControl paper;
    [SerializeField] private float TearLength;
    [SerializeField] private Collider2D m_collider;
    [SerializeField] private int tearChoice = 0;
    [Header("纸张")]
    [SerializeField] private SpriteRenderer paper_tear;
    [SerializeField] private SpriteRenderer paper_stay;
    [SerializeField] private AnimationCurve OffsetCurve;
    [Header("反馈")]
    [SerializeField] private float endRange = 0.1f;
    public float Progress{get{return (transform.position-initPos).magnitude/(endPos-initPos).magnitude;}}
    private Color initcolor;
    private Vector3 initPos;
    private Vector3 tearDir;
    private Vector3 tearNormalDir;
    private Vector3 endPos;
    private Vector3 tearPos;
    bool initialized = false;
    bool finished = false;
    bool StartTear = false;
    private static int AngleID  = Shader.PropertyToID("_FoldAngle");
    private static int TearID   = Shader.PropertyToID("_Tearing");
    private static int UseGreenMaskID = Shader.PropertyToID("_UseGreenMask");
    void Awake(){
        tearPos = initPos = transform.position;
        tearDir = transform.right.normalized;
        tearNormalDir = transform.up.normalized;
        endPos = transform.position + transform.right * TearLength;
        initialized = true;
    }
    void OnEnable(){
        float angle = Vector2.SignedAngle(tearDir, Vector2.right);
        paper_tear.material.SetFloat(AngleID, 270+angle);
        paper_stay.material.SetFloat(AngleID, 270+angle);
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(initPos.x, initPos.y, initPos.z, 1));
    }
    void OnDisable(){
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(initPos.x, initPos.y, initPos.z, 1));
    }
    void Update(){
        if(!StartTear && Vector3.Distance(transform.position, initPos)>0.01f){
            StartTear = true;
            paper_tear.material.SetFloat(UseGreenMaskID, tearChoice);
            paper_stay.material.SetFloat(UseGreenMaskID, tearChoice);
            paper.ChoiseThisTearPoint(this);
        }
        Vector3 diff = endPos - transform.position;
        
        if(Vector3.Dot(diff, tearDir)<endRange){
            if(!finished){
                finished = true;
                StartCoroutine(CoroutineFinished());
            }
        }
    }
    IEnumerator CoroutineFinished(){
        this.enabled = false;
        m_collider.enabled = false;
        TearPaperManager.Instance.StartPlayingTearLoop();
        for(float t=0; t<1; t+=Time.deltaTime){
            MoveTheTearPoint(tearDir*40);
            yield return null;
        }
        TearPaperManager.Instance.StopPlayingTearLoop();
        paper_tear.gameObject.SetActive(false);
        paper_stay.material.SetFloat("_AfterTeared", 1);
        paper.FinishThisPaper();
        gameObject.SetActive(false);
    }
    public void SetUpTextures(Texture2D tex){
        paper_tear.material.SetTexture("_TearMask", tex);
    }
    public void StartDragThisPoint(){
        if(!StartTear) {
            Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(initPos.x, initPos.y, initPos.z, 1));
            float angle = Vector2.SignedAngle(tearDir, Vector2.right);
            paper_tear.material.SetFloat(AngleID, 270+angle);
            paper_stay.material.SetFloat(AngleID, 270+angle);
        }
        EventHandler.Call_OnReadyToTear(this, true);
        paper_tear.material.SetFloat(TearID, 1);
        paper_stay.material.SetFloat(TearID, 1);
    }
    public void ReleaseThisPoint(){
        EventHandler.Call_OnReadyToTear(this, false);
        TearPaperManager.Instance.StopPlayingTearLoop();
    }
    /// <summary>
    /// 此方法会根据鼠标指针的位置，实时更新纸张撕扯的进度
    /// </summary>
    /// <param name="mousePos"></param>
    public void MoveTheTearPointToMousePos(Vector2 mousePos){
        if(finished) return;
        Vector2 calculateMouse = GameManager.mainCam.ScreenToWorldPoint(mousePos);
        Vector3 lastTearPos = tearPos;
        tearPos = Mathf.Max(0, Vector2.Dot(tearDir, calculateMouse - (Vector2)initPos)) * tearDir + initPos;
        // if(Vector3.Dot(tearPos-lastTearPos, tearDir)<=0){
        //     tearPos = lastTearPos;
        // }
        if(Vector3.Distance(transform.position, tearPos)>0.01f){
            TearPaperManager.Instance.StartPlayingTearLoop();
        }

        transform.position = tearPos;

        Vector3 foldPoint = (initPos + transform.position)/2f;
        
        float angle = Vector2.SignedAngle(tearDir, Vector2.right);
        Vector3 tear_fold_diff = paper_tear.transform.position - foldPoint;
        // paper_tear.transform.position = tearPos + initPosToPaper;
        // paper_tear.transform.rotation = Quaternion.Euler(0, 0, 90+transform.eulerAngles.z);
        
        paper_tear.material.SetFloat(AngleID, 270+angle);
        paper_stay.material.SetFloat(AngleID, 270+angle);
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(foldPoint.x, foldPoint.y, foldPoint.z, 1));
    }
    /// <summary>
    /// 此方法会根据鼠标的delta，来更新撕扯纸张的进度，因此不会跟随鼠标
    /// </summary>
    /// <param name="inputDirection"></param>
    public void MoveTheTearPoint(Vector2 inputDirection){
        tearPos += Mathf.Max(0, Vector2.Dot(inputDirection, tearDir)) * tearDir * Time.deltaTime;
        transform.position = tearPos;

        Vector3 foldPoint = (initPos + transform.position)/2f;

        float angle = Vector2.SignedAngle(tearDir, Vector2.right);
        Vector3 tear_fold_diff = paper_tear.transform.position - foldPoint;
        // paper_tear.transform.rotation = Quaternion.Euler(0, 0, 90+transform.eulerAngles.z);

        paper_tear.material.SetFloat(AngleID, 270+angle);
        paper_stay.material.SetFloat(AngleID, 270+angle);
        Shader.SetGlobalVector(Service.DRAG_POINT_ID, new Vector4(foldPoint.x, foldPoint.y, foldPoint.z, 1));
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
