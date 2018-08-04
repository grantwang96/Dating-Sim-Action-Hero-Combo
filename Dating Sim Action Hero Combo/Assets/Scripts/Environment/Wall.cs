using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Damageable {
    
	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // de-register and disable this object
    protected override void Die() {
        base.Die();
        GameManager.Instance.grid[xPos, yPos] = null;
        gameObject.SetActive(false);
    }
}
