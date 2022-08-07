using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject DreamInteraction;
    [SerializeField] private GameObject OpenFileInteraction;
    [SerializeField] private TearPaperManager tearPaperManager;
    public void GoToOpenFileInteraction(){
        DreamInteraction.SetActive(false);
        OpenFileInteraction.SetActive(true);
    }
}
