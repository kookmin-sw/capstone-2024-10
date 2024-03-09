using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Sector : NetworkBehaviour
{
    [SerializeField]
    public Define.SectorType SectorName;

    [SerializeField]
    private Gate[] _gates;
    public WorkStation[] _workStations;
    private SectorLight _sectorLight;

    [Networked, OnChangedRender(nameof(OnElectricPowerChanged))]
    public NetworkBool ElectricPower { get; set; }

    public override void Spawned()
    {
        _workStations = GetComponentsInChildren<WorkStation>();
    }

    public void OnElectricPowerChanged()
    {

    }
}
