using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refuel : BaseWorking, IInteractable
{
    public float requiredFuel;
    public float remainFuel;
    public float refuelSpeed;

    public EscapeShip fueledShip;
    
    // Start is called before the first frame update
    private void Start()
    {
        requiredTime = requiredFuel;
        workingTime = remainFuel;
        workingSpeed = refuelSpeed;

        isContinuable = true;
    }

    public IEnumerator Interact()
    {
        yield return StartCoroutine(Working());

        if (isComplete) { fueledShip.isFuel = true; }
    }
}
