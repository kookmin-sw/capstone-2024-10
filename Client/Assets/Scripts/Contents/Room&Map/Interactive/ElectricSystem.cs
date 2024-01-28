using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricSystem : BaseWorking, IInteractable
{
    public GameObject electricUI;
    private List<ElectricsystemButton> electricsystemButtons;

    public BaseSystem baseSystem;

    private void Start()
    {
        for (int i = 0; i < electricUI.transform.childCount; i++)
        {
            if(electricUI.transform.GetChild(i).TryGetComponent(out ElectricsystemButton button))
            {
                button.GetComponent<Button>().onClick.AddListener(() => SelectSegment("a", button.buttonType));
            }
        }
    }

    public IEnumerator Interact()
    {
        yield return null;

        //전력시스템 설정 코드
        electricUI.SetActive(true);

    }

    public void SelectSegment(string segmentName, ElectricsystemButton.ButtonType type)
    {

    }
}
