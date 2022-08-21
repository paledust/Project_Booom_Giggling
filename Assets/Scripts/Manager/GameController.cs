using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lightOffSprite;
    [SerializeField] private Color lightOffColor;
    [SerializeField] private GameObject DreamInteraction;
    [SerializeField] private GameObject OpenFileInteraction;
    [SerializeField] private GameObject TearPhotoInteraction;
    [SerializeField] private GameObject lampDragInteraction;
    [SerializeField] private DragManager tearPaperManager;
    void Awake(){
        Shader.SetGlobalInt("_USE_PURE_COLOR", 1);
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
    public void GoToLampInteraction(){
        lampDragInteraction.SetActive(true);
    }
    public void EndGame(){
        StartCoroutine(coroutineEndGame());
    }
    IEnumerator coroutineEndGame(){
        EventHandler.Call_OnQuit();
        for(float t=0; t<1; t+=Time.deltaTime*8){
            lightOffSprite.color = Color.Lerp(Color.clear, lightOffColor, EasingFunc.Easing.QuadEaseOut(t));
            yield return null;
        }
        yield return new WaitForSeconds(2);
        GameManager.Instance.EndGame();
    }
}
