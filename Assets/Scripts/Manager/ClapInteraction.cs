using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClapInteraction : MonoBehaviour
{
    [SerializeField] private DreamManager manager;
    [SerializeField] private AudioClip[] clapClips;
    [SerializeField] private AudioSource clapAudio;
    [SerializeField] private ParticleSystem clapParticles;
    [SerializeField] private Vector2 range;
    [SerializeField] private float tapStep = 0.1f;
    [SerializeField] private float clapStep = 1;
    [SerializeField] private int TotalClick = 3;
    float tapTime = 0;
    float clapTime =0;
    int clickTime = 0;
    void OnClapping(InputValue value){
        if(!value.isPressed) return;
        if(tapTime>Time.time-tapStep) return;
        tapTime = Time.time;

        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-range.x, range.x) * 0.5f;
        pos.y = Random.Range(-range.y, range.y) * 0.5f;

        clapParticles.transform.position = transform.InverseTransformPoint(pos);
        clapParticles.Play();
        clapAudio.PlayRandomClipFromClips(clapClips, 1);
        if(clapTime<=Time.time-clapStep){
            clapTime = Time.time;
            clickTime ++;
            if(clickTime>=TotalClick){
                manager.MoveToNextInteraction();
                return;
            }
        }
    }
}
