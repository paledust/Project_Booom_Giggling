using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHoldPaper : MonoBehaviour
{
    [SerializeField] private FingerPutter[] fingers;
    private int fingerCounter = 0;
    private bool allFingerOn = false;
    void OnEnable()=>EventHandler.E_OnPutOnFingers += TestFingers;
    void OnDisable()=>EventHandler.E_OnPutOnFingers -= TestFingers;
    public void TestFingers(bool putOnFinger){
        if(putOnFinger){
            fingerCounter ++;
        }
        else{
            fingerCounter --;
        }

        if(fingerCounter == fingers.Length){
            if(!allFingerOn){
                allFingerOn = true;
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }
        else{
            if(allFingerOn){
                allFingerOn = false;
                TearPaperManager.Instance.SetCanTear(allFingerOn);
            }
        }        
    }
}
