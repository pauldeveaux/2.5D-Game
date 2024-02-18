using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    
    private HeroKnight m_targetLogic;

    private GameObject m_camera;
    private double m_angle;
    
    [SerializeField]
    private Vector2 cameraOffset = new(-3, 2);

    [SerializeField] private float sensibility = 1;
    
    void Start()
    {
        m_camera = gameObject;
        m_targetLogic = target.GetComponent<HeroKnight>();
        m_angle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Rotate camera
        if (Input.GetAxis("Mouse X") != 0)
        {
            m_angle += Input.GetAxis("Mouse X") * -sensibility * 0.5;
           
            
            m_targetLogic.SetRotation(m_camera.transform.rotation);
        }
        
        // Move camera
        transform.position = target.transform.position + new Vector3(cameraOffset.x * (float) Math.Cos(m_angle), cameraOffset.y, 
                                                        cameraOffset.x * (float) Math.Sin(m_angle));
        
        // Rotation
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        Quaternion q = Quaternion.LookRotation(direction, Vector3.up);
        m_camera.transform.rotation = q;
    }
}
