using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DreamManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject[] stages;
    int stageIndex;
    void Awake(){stages[0].SetActive(true);}
    public void MoveToNextInteraction(){
        stages[stageIndex].SetActive(false);
        stageIndex ++;
        stages[stageIndex].SetActive(true);
    }
    public void EndDream(){
        stages[stageIndex].SetActive(false);
        gameController.GoToOpenFileInteraction();
    }
}
