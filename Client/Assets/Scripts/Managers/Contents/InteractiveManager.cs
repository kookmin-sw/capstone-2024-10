using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance:15.0f))
            {
                if(hit.transform.gameObject.CompareTag("Interactive"))
                {
                    hit.collider.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }
}

public interface IInteractable
{
    public void Interact();
}
