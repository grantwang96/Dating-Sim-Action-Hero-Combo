using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Date Blueprint")]
public class Date_Blueprint : NPC_Blueprint {

    [SerializeField] private float _waitTime;
    public float waitTime { get { return _waitTime; } }

    public override void IdleEnter(Brain brain) {
        DateBrain.Instance.currentTarget = null;
        // DateBrain.Instance.idleRoutine = brain.StartCoroutine(Idling(brain)); // save this coroutine
    }
    public override void IdleExecute(Brain brain) {
        // check for player or any threats
        if (CheckForThreat()) {
            DateBrain.Instance.ChangeStates(new Threat_Detected());
            DateBrain.Instance.idleRoutine = null;
        }

        if(Time.time - DateBrain.Instance.lastConversationTime > waitTime) {
            DateBrain.Instance.ChangeStates(new CalculatePath_Wander());
        }
    }
    public override IEnumerator Idling(Brain brain) {
        float time = 0f;
        while(time < _waitTime) {

            // check for player or any threats
            if (CheckForThreat()) {
                DateBrain.Instance.ChangeStates(new Threat_Detected());
                DateBrain.Instance.idleRoutine = null;
                yield break;
            }
            
            time += Time.deltaTime;
            yield return null;
        }
    }
    public override void IdleExit(Brain brain) {
        base.IdleExit(brain);
        // show date's question emote
    }
    
    public override void CalculateWanderExit(Brain brain) {
        base.CalculateWanderExit(brain);
    }

    public override void TravelEnter(Brain brain) {
        base.TravelEnter(brain);
    }
    public override void TravelExecute(Brain brain) {
        base.TravelExecute(brain);
    }

    private bool CheckForThreat() {
        // check for threats
        Transform threat = DateBrain.Instance.CheckVision();
        if (threat != null) {
            Debug.Log("A");
            if (threat == PlayerInput.Instance.transform && PlayerInput.Instance.agentModeOn) {
                DateBrain.Instance.currentTarget = PlayerDamageable.Instance;
                GameManager.Instance.EndGame();
                return true;
            } else {
                DateBrain.Instance.currentTarget = threat.GetComponent<Damageable>();
                DateBrain.Instance.ChangeStates(new Threat_Detected());
                return true;
            }
        }
        return false;
    }
}
