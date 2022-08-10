using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragLamp : MonoBehaviour
{
    [SerializeField] private Rigidbody2D lampRigid;
    [SerializeField] private Transform root;
    [SerializeField] private float dragLength;
    [SerializeField] private float dragPassLength;
    [SerializeField] private SpriteRenderer lightOffSprite;
    [SerializeField] private Color lightOffColor;
[Header("Audio")]
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private AudioClip lightOff_Ready_Clip;
    [SerializeField] private AudioClip lightOff_Clip;
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
            EventHandler.Call_OnQuit();
            m_audio.PlayOneShot(lightOff_Clip);
            StartCoroutine(coroutineEnd());
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
    IEnumerator coroutineEnd(){
        for(float t=0; t<1; t+=Time.deltaTime*8){
            lightOffSprite.color = Color.Lerp(Color.clear, lightOffColor, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        yield return new WaitForSeconds(2);
        Application.Quit();
    }
}
