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
        Vector3 screenPos = Camera.main.WorldToScreenPoint(m_obj.transform.position);//��ȡ��Ҫ�ƶ����������ת��Ļ����

        Vector3 mousePos = Input.mousePosition;//��ȡ���λ��

        mousePos.z = screenPos.z;//��Ϊ���ֻ��X��Y�ᣬ����Ҫ��������Z��

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);//��������Ļ����ת������������

        m_obj.position = worldPos;//���������ƶ�

    }
}
