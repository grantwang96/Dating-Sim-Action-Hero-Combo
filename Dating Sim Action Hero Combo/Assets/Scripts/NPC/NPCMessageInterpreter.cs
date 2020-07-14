using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMessageInterpreter {

    private NPCUnit _unit;

    public NPCMessageInterpreter(NPCUnit unit) {
        _unit = unit;
    }

    public void InterpetMessage(NPCUnit ally, UnitMessage message) {
        switch (message) {
            case UnitMessage.HostileFound:
                OnHostileFound(ally);
                break;
            // add more cases as necessary
        }
    }

    private void OnHostileFound(NPCUnit ally) {
        if (_unit.TargetManager.CurrentTarget == null) {
            _unit.TargetManager.OverrideCurrentTarget(ally.TargetManager.CurrentTarget);
        }
    }
}