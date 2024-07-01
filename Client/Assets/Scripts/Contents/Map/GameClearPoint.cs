using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Crew creature = other.GetComponent<Crew>();
            if (creature != null )
            {
                StartCoroutine(creature.OnWin());
            }
            
        }
    }
}
