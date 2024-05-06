using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CargoPassageGate : NetworkBehaviour
{
    public void OpenDoor()
    {
        GetComponent<NetworkMecanimAnimator>().Animator.SetBool("DoorOpen", true);
        GetComponent<AudioSource>().Play();
    }
}
