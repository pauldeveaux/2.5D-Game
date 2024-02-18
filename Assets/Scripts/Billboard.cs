using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private GameObject m_camera;

    private void Start()
    {
        m_camera = GameObject.Find("Main Camera");
    }
    
    private void LateUpdate()
    {
        Vector3 rotation = m_camera.transform.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;

        transform.eulerAngles = rotation;
    }
}
