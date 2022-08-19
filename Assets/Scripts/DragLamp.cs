using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class DragLamp : MonoBehaviour
{
    [SerializeField] private Rigidbody2D lampRigid;
    [SerializeField] private Transform root;
    [SerializeField] private float dragLength;
    [SerializeField] private float dragPassLength;
[Header("Audio")]
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private AudioClip lightOff_Ready_Clip;
    [SerializeField] private AudioClip lightOff_Clip;
[Header("Trigger Event")]
    [SerializeField] private UnityEvent OnLampTriggered;
    private bool passed = false;
    private bool sound  = false;
    public void OnDrag(){
        EventHandler.Call_OnSnapToStuff(lampRigid.transform);
        lampRigid.velocity = Vector3.zero;
        lampRigid.angularVelocity = 0;
        lampRigid.isKinematic = true;
    }
    public void OnReleased(){
        lampRigid.isKinematic = false;
        EventHandler.Call_OnSnapToStuff(null);
        if(passed){
            m_audio.PlayOneShot(lightOff_Clip);
            OnLampTriggered?.Invoke();
        }
    }
    public void MoveLampRope(Vector2 mousePos){
        Vector3 worldPos = GameManager.mainCam.ScreenToWorldPoint(mousePos);
        Vector3 diff = worldPos - root.position;
        diff.z = 0;
        worldPos = root.position + Vector3.ClampMagnitude(diff, dragLength);

        if(diff.magnitude > dragPassLength){
            if(!passed) passed = true;
            if(!sound){
                 sound  = true;
                m_audio.PlayOneShot(lightOff_Ready_Clip);
            }
        }
        else{
            if(sound) sound = false;
        }

        lampRigid.MovePosition(worldPos);
    }
}
