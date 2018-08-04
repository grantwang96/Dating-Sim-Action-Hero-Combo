using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : Brain{

    [SerializeField] protected WeaponType heldWeapon;
    [SerializeField] private bool canFire = true;
    [SerializeField] private int _currentClip;
    [SerializeField] private BulletNoise gunBarrel;

    [SerializeField] private Vector2 targetLastSpotted;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        _currentClip = heldWeapon.clipCapacity;

        ChangeStates(new NPC_Idle());
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    protected override void React(Vector2 dir) {
        myCharMove.SetRotation(dir);
        FireGun();
    }
    
    private void FireGun() {
        if (!canFire) { return; }
        if (!heldWeapon) { return; }

        float coolDown = heldWeapon.Fire(transform.position + transform.up, transform.up);
        gunBarrel.Noise(heldWeapon.noiseRadius);
        _currentClip--;

        if(_currentClip == 0) {
            _currentClip = heldWeapon.clipCapacity;
            coolDown = heldWeapon.reloadTime;
        }
        StartCoroutine(WaitToFire(coolDown));
    }

    private IEnumerator WaitToFire(float time) {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
