using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStateControl : MonoBehaviour
{
    [SerializeField] private Animation handStateAnime;
    [SerializeField] private AnimationClip outAnimeClip;
    void OnEnable()=>EventHandler.E_OnSwitchHand += StickHandOut;
    void OnDisable()=>EventHandler.E_OnSwitchHand -= StickHandOut;
    void StickHandOut(bool isOut){
        handStateAnime.Play(outAnimeClip.name);
    }
}
