using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RighthandController : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion lastRot;
    Vector3 lastPos;
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

        // lastRot = transform.rotation;
        // lastPos = transform.position;
        // Debug.Log(transform.position.x > lastPos.x);

        // if(transform.position.x > lastPos.x)
        // {
        //     transform.rotation = Quaternion.Euler(0,0,lastRot.z+10);
        // }else
        // {
        //     transform.rotation = Quaternion.Euler(0,0,lastRot.z-10);
        // }





    }

    // void RighthandMove2()
    // {

    //     transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // }


}
