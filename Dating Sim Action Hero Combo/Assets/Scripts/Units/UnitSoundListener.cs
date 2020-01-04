using UnityEngine;
using System;

public interface ISoundListener
{
    event Action<IntVector3, Unit> OnCombatSoundHeard;

    void OnSoundHeard(IntVector3 origin, Unit source = null);
}
