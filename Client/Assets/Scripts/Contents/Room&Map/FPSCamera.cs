using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensitivity = 100;
    public float rotationX;
    public float rotationY;

    // Update is called once per frame
    void Update()
    {
        float mouseMoveX = Input.GetAxis("Mouse X");
        float mouseMoveY = Input.GetAxis("Mouse Y");

        rotationY += mouseMoveX * sensitivity * Time.deltaTime;
        rotationX += mouseMoveY * sensitivity * Time.deltaTime;

        if(rotationX > 35) { rotationX = 35f; }
        if(rotationX < -30f) { rotationX = -30f; }

        transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
    }
}
