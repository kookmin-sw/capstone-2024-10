using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Gate : NetworkBehaviour
{
    public void Open()
    {
        GetComponent<NetworkMecanimAnimator>().Animator.SetBool("IsOpen", true);
    }

    public void Close()
    {
        GetComponent<NetworkMecanimAnimator>().Animator.SetBool("IsOpen", false);
    }
}
