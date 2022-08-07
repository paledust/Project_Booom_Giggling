using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamTimer : MonoBehaviour
{
    [SerializeField] private DreamManager beginningManager;
    [SerializeField] private float time = 5;
    [SerializeField] private bool lastone = false;
    void Start(){
        StartCoroutine(coroutineCountDown());
    }
    IEnumerator coroutineCountDown(){
        yield return new WaitForSeconds(time);
        if(lastone){
            beginningManager.EndDream();
        }
        else{
            beginningManager.MoveToNextInteraction();
        }
    }
}
