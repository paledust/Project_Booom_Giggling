using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStateControl : MonoBehaviour
{
    [SerializeField] private Animation handStateAnime;
    [SerializeField] private AnimationClip outAnimeClip;
    [SerializeField] private AnimationClip inAnimeClip;
    void OnEnable(){
        EventHandler.E_OnSwitchHand += PlayHandAnimation;
    }
    void OnDisable(){
        EventHandler.E_OnSwitchHand -= PlayHandAnimation;
    }
    void PlayHandAnimation(bool isOut){
        handStateAnime.Play(isOut?outAnimeClip.name:inAnimeClip.name);
    }
}
