using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensorManager : MonoBehaviour
{
    private List<Sensor_HeroKnight> m_groundSensors;
    
    // Start is called before the first frame update
    void Start()
    {
        m_groundSensors = new List<Sensor_HeroKnight>(new Sensor_HeroKnight[]
        {
            transform.Find("GroundSensor1").GetComponent<Sensor_HeroKnight>(),
            transform.Find("GroundSensor2").GetComponent<Sensor_HeroKnight>(),
            transform.Find("GroundSensor3").GetComponent<Sensor_HeroKnight>()
        });
    }

    
    public bool State()
    {
        bool r = false;
        foreach (var sensor in m_groundSensors)
        {
            r = r || sensor.State();
        }

        return r;
    }
    
    public void Disable(float duration)
    {
        foreach (var sensor in m_groundSensors)
        {
            sensor.Disable(duration);
        }
    }
}
