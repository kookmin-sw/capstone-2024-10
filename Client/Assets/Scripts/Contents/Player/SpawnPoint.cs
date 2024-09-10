using Fusion;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public struct SpawnPointData : INetworkStruct
    {
        public Vector3 Position;
        public Define.SectorName SectorName;
    }

    [SerializeField]
    private Define.SectorName _sectorName;

    public SpawnPointData Data => new()
    {
        Position = gameObject.transform.position,
        SectorName = _sectorName
    };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
