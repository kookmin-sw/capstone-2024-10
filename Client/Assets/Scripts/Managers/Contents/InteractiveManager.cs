using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !MapManager.baseSystem.isInteracting)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance:15.0f))
            {
                if(hit.transform.gameObject.CompareTag("Interactive"))
                {
                    Transform detectedObject = hit.transform;

                    if (!detectedObject.parent.TryGetComponent(out IInteractable interactable)) { interactable = detectedObject.GetComponent<IInteractable>(); }

                    StartCoroutine(interactable.Interact());
                }
            }
        }
    }
}
