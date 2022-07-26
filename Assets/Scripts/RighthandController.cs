using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(m_obj.transform.position);//获取需要移动物体的世界转屏幕坐标

        Vector3 mousePos = Input.mousePosition;//获取鼠标位置

        mousePos.z = screenPos.z;//因为鼠标只有X，Y轴，所以要赋予给鼠标Z轴

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);//把鼠标的屏幕坐标转换成世界坐标

        m_obj.position = worldPos;//控制物体移动

    }
}
