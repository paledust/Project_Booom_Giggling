using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RighthandController : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion lastRot;
    Vector3 lastPos;
    public Sprite sprite1;
    public Sprite sprite2;
    private int handState = 0; 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RighthandMove();
    }

    void RighthandMove()
    {
        Vector3 pos = GameManager.mainCam.WorldToScreenPoint(transform.position);
        Vector3 m_MousePos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, pos.z);
        transform.position = GameManager.mainCam.ScreenToWorldPoint(m_MousePos);
    }
    
    void HandStateChange(){
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (handState == 0) {
                handState = 1;
                renderer.sprite = this.sprite1;
            } else {
                handState = 0;
                renderer.sprite = this.sprite2;
            }
    }




}
