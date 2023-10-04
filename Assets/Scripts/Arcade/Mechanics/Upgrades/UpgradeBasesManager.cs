using System.Collections.Generic;
using Arcade.Mechanics.Doors;
using Player.Observer_pattern;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(4)]
public class UpgradeBasesManager : MonoBehaviour, IObserver
{
    private List<UpgradeBase> _upgradeBases;
    private List<MapUpgrades> _maps;
    [SerializeField] private UpgradeBase currentBase;
    private ArcadePlayerData _playerData;

    void Start()
    {
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _playerData.AddObserver(this);
        _upgradeBases = new List<UpgradeBase>(FindObjectsOfType<UpgradeBase>());
        _maps = new List<MapUpgrades>(FindObjectsOfType<MapUpgrades>());
        SelectBase(currentBase.GetZone);
        UpdateMaps(currentBase.GetZone);
    }

    private void SelectNewLocation()
    {
        List<ZonesArcade> possibleLocations = new List<ZonesArcade>(_playerData.GetUnlockedZonesArrayForMap());
        if (possibleLocations.Count > 1)
        {
            ZonesArcade selectedZone;
            do
            {
                selectedZone = possibleLocations[Random.Range(0, possibleLocations.Count)];
            } while (selectedZone == currentBase.GetZone  );

            currentBase = SelectBase(selectedZone);
            UpdateMaps(selectedZone);
        }
        else
        {
            currentBase = SelectBase(possibleLocations[0]);
            UpdateMaps(possibleLocations[0]);
        }
    }

    private UpgradeBase SelectBase(ZonesArcade zone)
    {
        UpgradeBase aux = null;
        foreach (var auxBase in _upgradeBases)
        {
            if (auxBase.GetZone == zone)
            {
                aux = auxBase;
                auxBase.ActivateBase();
            }
            else
            {
                auxBase.DeactivateBase();
            }
        }

        return aux;
    }

    private void UpdateMaps(ZonesArcade zonesArcade)
    {
        foreach (var map in _maps)
            map.SetCurrentUpgradePoint(zonesArcade);
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeUpgradeLocation)
            SelectNewLocation();
    }
}