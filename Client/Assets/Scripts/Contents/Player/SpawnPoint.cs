using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Define.SectorName _sectorName;

    public Define.SectorName SectorName => _sectorName;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
