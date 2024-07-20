using UnityEngine;

public class TutorialClearPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialCrew creature = other.GetComponent<TutorialCrew>();
            if (creature != null)
            {
                creature.OnWin();
            }
        }
    }
}
