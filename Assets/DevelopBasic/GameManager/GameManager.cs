using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleSaveSystem;
//Please make sure "GameManager" is excuted before every custom script
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool LoadInitScene = false;
    public static Camera mainCam;
    private static bool isSwitchingScene = false;
    private static bool isPaused = false;
    protected override void Awake()
    {
        base.Awake();
        SaveManager.LoadGame();
        GameManager.mainCam = Camera.main;
        if(LoadInitScene)SceneManager.LoadSceneAsync("Level-0", LoadSceneMode.Additive);
    }
    public void SwitchingScene(string from, string to){
        if(!isSwitchingScene){
            StartCoroutine(SwitchSceneCoroutine(from, to));
        }
    }
    public void SwitchingScene(string to){
        if(!isSwitchingScene){
            StartCoroutine(SwitchSceneCoroutine(to));
        }
    }
    public void PauseTheGame(){
        if(isPaused) return;
        
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
    }
    public void ResumeTheGame(){
        if(!isPaused) return;

        AudioListener.pause = false;
        Time.timeScale = 1;
        isPaused = false;
    }
    public void EndGame(){
        Application.Quit();
    }
    void OnApplicationQuit(){
        SaveManager.SaveGame();
    }
    /// <summary>
    /// This method is good for load scene in an additive way, having a persistance scene
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    IEnumerator SwitchSceneCoroutine(string from, string to){
        isSwitchingScene = true;

        if(from != string.Empty){
            //TO DO: do something before the last scene is unloaded. e.g: call event of saving 
            yield return SceneManager.UnloadSceneAsync(from);
        }
        //TO DO: do something after the last scene is unloaded.
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(to));
        //TO DO: do something after the next scene is loaded. e.g: call event of loading

        isSwitchingScene = false;
    }
    /// <summary>
    /// This method is good for load one scene each time, no persistance scene
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    IEnumerator SwitchSceneCoroutine(string to){
        isSwitchingScene = true;

        //TO DO: do something before the next scene is loaded. e.g: call event of saving 
        yield return SceneManager.LoadSceneAsync(to);
        //TO DO: do something after the next scene is loaded. e.g: call event of loading

        isSwitchingScene = false;
    }
}
