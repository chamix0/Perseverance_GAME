using System;
using System.Collections.Generic;
using Arcade.Mechanics.Doors;
using UnityEngine;

public class MapUpgrades : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<Transform> points;
    [SerializeField] private Transform circle;

    public void SetCurrentUpgradePoint(ZonesArcade zones)
    {
        circle.position = zones switch
        {
            ZonesArcade.Lobby => points[0].position,
            ZonesArcade.Library => points[1].position,
            ZonesArcade.Salon => points[2].position,
            ZonesArcade.Freezer => points[3].position,
            ZonesArcade.Tubes => points[4].position,
            ZonesArcade.Storage => points[5].position,
            _ => throw new ArgumentOutOfRangeException(nameof(zones), zones, null)
        };
    }
}