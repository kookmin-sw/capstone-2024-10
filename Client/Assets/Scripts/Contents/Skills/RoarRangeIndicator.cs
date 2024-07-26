using UnityEngine;

public class RoarRangeIndicator : MonoBehaviour
{
    public Material rangeIndicatorMat;
    private float length = 3f;
    private float width = 2f;

    private void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0f);
    }
}
