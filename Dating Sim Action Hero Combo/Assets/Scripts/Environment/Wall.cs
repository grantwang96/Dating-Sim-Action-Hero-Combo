using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Damageable {

    private Vector3 roundedPosition;

	// Use this for initialization
	void Start () {
        try {
            // register blockage at this location
            roundedPosition = transform.position;
            GameManager.Instance.obstructions.Add(
                new Vector3(Mathf.RoundToInt(roundedPosition.x),
                Mathf.RoundToInt(roundedPosition.y),
                Mathf.RoundToInt(roundedPosition.z)));
            Debug.Log("Obstruction at: " + roundedPosition);
        } catch {
            Debug.LogError("PROBLEM REGISTERING WALL IN MAP");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void Die() {
        base.Die();
        GameManager.Instance.obstructions.Remove(roundedPosition);
        Destroy(gameObject);
    }
}
