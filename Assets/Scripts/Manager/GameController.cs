using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject DreamInteraction;
    [SerializeField] private GameObject OpenFileInteraction;
    [SerializeField] private GameObject TearPhotoInteraction;
    [SerializeField] private TearPaperManager tearPaperManager;
    void Awake(){
        EventHandler.E_OnSwitchHand += SwitchTearPaperManager;
    }
    void OnDestroy(){
        EventHandler.E_OnSwitchHand -= SwitchTearPaperManager;
    }
    void SwitchTearPaperManager(bool isOut){
        tearPaperManager.gameObject.SetActive(isOut);
    }
    public void GoToOpenFileInteraction(){
        DreamInteraction.SetActive(false);
        OpenFileInteraction.SetActive(true);
    }
    public void GoToTearPhotoInteraction(){
        OpenFileInteraction.SetActive(false);
        TearPhotoInteraction.SetActive(true);
    }
}
