using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QskillRangeIndicator : MonoBehaviour
{
    public Material rangeIndicatorMat;
    private float length = 3f;
    private float width = 2f;

    private void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0f);
        transform.position += transform.forward / 1.5384f * 2.5f * length ;
        transform.eulerAngles = new Vector3(0, 0, 90);
        rangeIndicatorMat.SetFloat("_Falloff", 5);
        rangeIndicatorMat.SetFloat("_Inner_Band_Start", 0.1f);
        rangeIndicatorMat.SetFloat("_Inner_Band_Width", 0.2f);
        rangeIndicatorMat.SetFloat("_Oscillation_Speed", 0.1f);
        rangeIndicatorMat.SetFloat("_Oscillation_Intensity", 2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.localScale = new Vector3(1, width, length);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            transform.localScale = new Vector3(0, 0, 0f);
        }
    }
}
