﻿using System.Collections;
using System.Collections.Generic;
using System;

public interface IUnitsManager : IInitializableManager {
    
    void RegisterUnit(Unit unit);
    void DeregisterUnit(Unit unit);
    List<Unit> GetUnitListByTags(UnitTags tags);
    
    event Action<Unit> OnUnitRegistered;
    event Action<Unit> OnUnitDeregistered;
}
/// <summary>
/// Parent class that manages all units in the given level
/// </summary>
public class UnitsManager : IUnitsManager
{
    public static IUnitsManager Instance => GetOrSetInstance();
    private static IUnitsManager _instance;
    
    public event Action<Unit> OnUnitRegistered;
    public event Action<Unit> OnUnitDeregistered;

    private List<Unit> _allUnits = new List<Unit>();
    private Dictionary<UnitTags, List<Unit>> _unitsByTags = new Dictionary<UnitTags, List<Unit>>();

    private static IUnitsManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new UnitsManager();
        }
        return _instance;
    }
    
    public void Initialize(Action<bool> initializationCallback = null) {
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        for(int i = 0; i < _allUnits.Count; i++) {
            Unit unit = _allUnits[i];
            unit.Despawn();
        }
        _allUnits.Clear();
        _unitsByTags.Clear();
    }
    
    public void RegisterUnit(Unit unit) {
        _allUnits.Add(unit);
        AddUnitToUnitsByTags(unit, unit.UnitTags);
        OnUnitRegistered?.Invoke(unit);
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
        unit.Dispose();
        OnUnitDeregistered?.Invoke(unit);
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