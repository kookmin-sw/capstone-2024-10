using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialClearPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Crew creature = other.GetComponent<Crew>();
            if (creature != null)
            {
                GameObject.FindWithTag("Player").GetComponent<TutorialCrew>().CrewTutorialUI.gameObject.SetActive(false);
            }

        }
    }
}
