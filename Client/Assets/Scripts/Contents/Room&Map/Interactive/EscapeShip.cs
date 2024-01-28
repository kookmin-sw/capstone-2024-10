using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeShip : MonoBehaviour, IInteractable
{
    //이하의 모든 조건을 만족시켜야 한다
    public bool isFuel;
    
    public IEnumerator Interact()
    {
        yield return null;

        //이하의 모든 조건을 만족시켜야한다.
        if(isFuel)
        {
            SuccessEscape();
        }
    }

    private void SuccessEscape()
    {

    }
}
