using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLight : MonoBehaviour
{
    [HideInInspector]
    public Light light;
    
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponentInChildren<Light>();
    }
}
