using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlienSkill : UI_Base
{
    public Alien Alien { get; set; }
    public Image[] imageCooldowns;
    public float[] cooldowns;
    private bool[] isCooldowns;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        isCooldowns = new bool[3];
        cooldowns = new float[3];

        for (int i = 0; i < 3; i++)
        { 
            imageCooldowns[i].fillAmount = 0f;
            cooldowns[i] = 5f;
            isCooldowns[i] = false;
        }

        Debug.Log("initSuccess");
        return true;
    }

    private void Update()
    {
        if (Alien == null) return;

        if (Alien.Object == null || !Alien.Object.IsValid) return;

        Debug.Log("hihi");

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log('Q');
            isCooldowns[0] = true;
        }   
        else if (Input.GetKeyDown(KeyCode.Mouse1))
            isCooldowns[1] = true;
        else if (Input.GetKeyDown(KeyCode.R))
            isCooldowns[2] = true;

        for (int i = 0; i < 3; i++)
        {
            if (isCooldowns[i])
            {
                imageCooldowns[i].fillAmount += 1 / cooldowns[i] * Time.deltaTime;

                if (imageCooldowns[i].fillAmount >= 1)
                {
                    isCooldowns[i] = false;
                    imageCooldowns[i].fillAmount = 0;
                }
            }
        }
    }
}
