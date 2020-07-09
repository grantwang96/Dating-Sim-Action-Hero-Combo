using System;
using UnityEngine;

public class UnitSoundListener : MonoBehaviour {

    public event Action<IntVector3, Unit> OnCombatNoiseHeard;

    public virtual void OnSoundHeard(IntVector3 position, Unit source) {
        OnCombatNoiseHeard?.Invoke(position, source);
    }
}