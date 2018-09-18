﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : Brain {

    public WeaponType heldWeapon;
    [SerializeField] protected bool canFire = true;
    [SerializeField] protected int _currentClip;
    [SerializeField] protected BulletNoise gunBarrel;

    // Use this for initialization
    protected override void Start () {
        if (!enemies.Contains(PlayerDamageable.Instance)) { enemies.Add(PlayerDamageable.Instance); }

        base.Start();
        _currentClip = heldWeapon.clipCapacity;

        // ChangeStates(new Civilian_Idle());
        ChangeStates(new Idle());
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override Transform CheckVision() {
        foreach (Damageable enemy in GameManager.Instance.goodGuys) {
            if (Vector2.Angle(enemy.transform.position - transform.position, transform.up) < myBluePrint.maxVisionAngle &&
               Vector2.Distance(enemy.transform.position, transform.position) < myBluePrint.rangeOfVision) {

                RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, myBluePrint.rangeOfVision, visionMask);
                if (rayhit.transform == enemy.transform) {
                    return enemy.transform;
                }
            }
        }
        return null;
    }
    
    public override void ReactToThreat(Damageable target) {
        if(currentState.GetType() == typeof(Threat_Detected)) { return; }
        currentTarget = target;
        ChangeStates(new Threat_Detected());
    }

    public override void MainAction() {
        FireGun();
    }

    protected void FireGun() {
        if (!canFire) { return; }
        if (!heldWeapon) { return; }

        float coolDown = heldWeapon.Fire(transform.position + transform.up, transform.up, myDamageable);
        gunBarrel.Noise(heldWeapon.noiseRadius);
        _currentClip--;

        if(_currentClip == 0) {
            _currentClip = heldWeapon.clipCapacity;
            coolDown = heldWeapon.reloadTime;
        }
        StartCoroutine(WaitToFire(coolDown));
    }

    protected IEnumerator WaitToFire(float time) {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
