using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitsManager {

    void RegisterUnit(Unit unit);
    void DeregisterUnit(Unit unit);
    List<Unit> GetUnitListByTags(UnitTags tags);
}

public class UnitsManager : MonoBehaviour, IUnitsManager
{
    public static IUnitsManager Instance { get; private set; }

    [SerializeField] private List<Unit> _allUnits = new List<Unit>();
    private Dictionary<UnitTags, List<Unit>> _unitsByTags = new Dictionary<UnitTags, List<Unit>>();

    private void Awake() {
        Instance = this;
    }

    public void RegisterUnit(Unit unit) {
        _allUnits.Add(unit);
        AddUnitToUnitsByTags(unit, unit.UnitTags);

        unit.OnUnitTagsSet += OnUnitTagsUpdated;
    }

    private void AddUnitToUnitsByTags(Unit unit, UnitTags tags) {
        if (!_unitsByTags.ContainsKey(tags)) {
            _unitsByTags.Add(tags, new List<Unit>());
        }
        _unitsByTags[tags].Add(unit);
    }

    public void DeregisterUnit(Unit unit) {
        _allUnits.Remove(unit);
        RemoveUnitFromUnitsByTags(unit, unit.UnitTags);
        unit.OnUnitTagsSet -= OnUnitTagsUpdated;
    }

    private void RemoveUnitFromUnitsByTags(Unit unit, UnitTags tags) {
        if (_unitsByTags.TryGetValue(tags, out List<Unit> units)) {
            _unitsByTags[unit.UnitTags].Remove(unit);
        }
    }

    public List<Unit> GetUnitListByTags(UnitTags tags) {
        List<Unit> units = new List<Unit>();
        foreach(KeyValuePair<UnitTags, List<Unit>> pair in _unitsByTags) {
            if((tags & pair.Key) != 0) {
                units.AddRange(pair.Value);
            }
        }
        return units;
    }

    private void OnUnitTagsUpdated(Unit unit, UnitTags newTags) {
        RemoveUnitFromUnitsByTags(unit, unit.UnitTags);
        AddUnitToUnitsByTags(unit, newTags);
    }
}
