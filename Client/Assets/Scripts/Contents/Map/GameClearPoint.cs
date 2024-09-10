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
                creature.OnWin();
            }

        }
    }
}
