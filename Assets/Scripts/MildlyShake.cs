using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MildlyShake : MonoBehaviour
{
    [SerializeField] private float shakeFreq = 10;
    [SerializeField] private float shakeAmp  = 2;
    Vector3 initPos;
    void Awake(){
        initPos = transform.position;
    }
    void Update()
    {
        Vector3 pos = initPos;
        pos.x += (Mathf.PerlinNoise(Time.time*shakeFreq + 0.124546f, 0.124546f)*2-1)*shakeAmp;
        pos.y += (Mathf.PerlinNoise(Time.time*shakeFreq + 0.298574f, 0.298574f)*2-1)*shakeAmp;
        transform.position = pos;
    }
}
