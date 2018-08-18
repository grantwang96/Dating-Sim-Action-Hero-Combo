using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that handles player's interaction and combat abilities
/// </summary>
public class PlayerAttack : MonoBehaviour {

    // total ammo player has on hand
    [SerializeField] private int _totalAmmo;
    public int totalAmmo { get { return _totalAmmo; } }

    // total ammo in the current clip
    [SerializeField] private int _currentClip;
    public int currentClip { get { return _currentClip; } }

    // allows the player to shoot
    private bool canFire = true;

    // the current weapon in the player's hand
    [SerializeField] private WeaponType heldWeapon;
    // where the sound is emitted from the shots
    [SerializeField] private BulletNoise gunBarrel;

	// Use this for initialization
	void Start () {
        PlayerDamageable.Instance.OnPlayerDeath += OnPlayerDeath; // subscribe the player death functionality
	}
	
	// Update is called once per frame
	void Update () {

    }
    
    public void MainAction() { // occurs when you press Fire1
        if (PlayerInput.Instance.switchingOutfits) { return; }
        if (PlayerInput.Instance.agentModeOn) { // fire your gun if you're in agent mode
            FireGun();
        } else { // talk with person
            Talk();
        }
    }

    private void Talk() { // interact with an NPC
        if (!canFire) { return; }
        StartCoroutine(WaitToFire(.2f));
    }

    private void FireGun() { // fires your currently held weapon
        if (!canFire || !heldWeapon) { return; }

        float coolDown = heldWeapon.Fire(transform.position + transform.up, transform.up, PlayerDamageable.Instance);
        gunBarrel.Noise(heldWeapon.noiseRadius);
        _currentClip--;
        
        if (_currentClip == 0 && _totalAmmo > 0) { StartCoroutine(Reload(heldWeapon.reloadTime)); }
        else { StartCoroutine(WaitToFire(coolDown)); }
    }

    private IEnumerator WaitToFire(float time) { // handles the cooldown between shots
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
    }

    private IEnumerator Reload(float time) {
        canFire = false;
        float t = 0f;
        while(t < time) {
            t += Time.deltaTime;

            // visual reload

            yield return new WaitForEndOfFrame();
        }
        
        if(_totalAmmo > heldWeapon.clipCapacity) {
            _currentClip = heldWeapon.clipCapacity;
            _totalAmmo -= heldWeapon.clipCapacity;
        } else {
            _currentClip = _totalAmmo;
            _totalAmmo = 0;
        }
        canFire = true;
    }

    public void OnPlayerDeath() {
        this.enabled = false;
    }
}
