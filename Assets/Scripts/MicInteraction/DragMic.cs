using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
public class DragMic : MonoBehaviour{
    [SerializeField] DreamManager beginningManager;
    [SerializeField] private MicRightHandController rightHand;
    [SerializeField] private Transform rotatePoint;
[Header("Feedback")]
    [SerializeField] private PostProcessVolume ppVolume;
    [SerializeField] private Animation endAnime;
    [SerializeField] private float angleRange = 10;
    [SerializeField] private float targetAngle =10;
    float angle;
    bool dragging = false;
    bool tuning = false;
    bool willFinish = false;
    float radius;
    void Awake(){
        Vector3 diff = transform.position - rotatePoint.position;
        radius = diff.magnitude;
    }
    void Update()
    {
        if(Mouse.current.leftButton.isPressed){
            if(!dragging){
                dragging = true;
                Vector3 mousePoint = GameManager.mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Collider2D hit = Physics2D.OverlapCircle(mousePoint, 0.3f, Service.InteractableLayer);
                if(hit!=null){
                    tuning = true;
                    rightHand.SnapRightHandToTarget(transform);
                }
                else{
                    tuning = false;
                }
            }
        }
        else{
            if(dragging){
                tuning = false;
                dragging = false;
                rightHand.SnapRightHandToTarget(null);
                if(willFinish){
                    if(!Mouse.current.leftButton.isPressed){
                        this.enabled = false;
                        StartCoroutine(coroutineEndDream());
                        return;
                    }
                }
            }
        }

        if(tuning){
            Vector2 mouseInput = Mouse.current.delta.ReadValue();
            Vector2 offset;
            offset.y = mouseInput.y;
            offset.x = mouseInput.x;
            transform.position += (Vector3)offset * Time.deltaTime;
            Vector3 diff = transform.position - rotatePoint.position;
            diff.z = 0;
            transform.position = rotatePoint.position + diff.normalized*radius;
            angle = 180+Vector3.SignedAngle(diff, Vector3.right, Vector3.back);
            transform.rotation = Quaternion.Euler(0,0,angle);

            if(Mathf.Abs(angle-targetAngle)<=angleRange){
                ppVolume.weight = 1;
                willFinish = true;
            }
            else{
                ppVolume.weight = 0;
                willFinish = false;
            }
        }
    }
    IEnumerator coroutineEndDream(){
        endAnime.Play();
        yield return new WaitForSeconds(endAnime.clip.length+2);
        beginningManager.EndDream();
    }
}
