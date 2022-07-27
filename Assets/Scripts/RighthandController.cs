using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RighthandController : MonoBehaviour
{
    Vector3 initialPosition;
    Quaternion lastRot;
    Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        RighthandMove();
        
    }

    void RighthandMove()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 m_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos.z);
        transform.position = Camera.main.ScreenToWorldPoint(m_MousePos);

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
