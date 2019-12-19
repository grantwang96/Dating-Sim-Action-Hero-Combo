using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Dead")]
public class AIState_Dead : AIStateDataObject {

    protected override ActiveAIState GenerateActiveAIState(IUnitController controller) {
        ActiveAIState deadState = new ActiveAIState();
        return deadState;
    }
}
