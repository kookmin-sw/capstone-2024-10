using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerScript : MonoBehaviour, IInteractable
{
    private bool isWorking = false;
    public GameObject workingUI;

    public void Interact()
    {
        if (!isWorking)
        {
            Time.timeScale = 0;
            Debug.Log("Hello");

            workingUI.SetActive(true);
            isWorking = true;
        }
        else
        {
            workingUI.SetActive(false);
            Time.timeScale = 1;
            isWorking = false;
        }
    }

    IEnumerator IInteractable.Interact()
    {
        throw new System.NotImplementedException();
    }
}