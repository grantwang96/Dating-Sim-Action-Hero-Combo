using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public static PlayerInput Instance;

    private PlayerMovement playerMove; // for moving the character around
    private PlayerAttack playerAttack; // for fighting and interacting, also stores what mode the player is in

    private SpriteRenderer srend;

    private Vector2 _moveVector;
    public Vector2 moveVector { get { return _moveVector; } }
    private Vector2 _lookVector;
    public Vector2 lookVector { get { return _lookVector; } }
    
    private bool _agentModeOn = false;
    public bool agentModeOn {
        get { return _agentModeOn; }
    }
    protected bool AgentModeOn {
        set {
            _agentModeOn = value;
            gameObject.layer = (_agentModeOn) ? 8 : 9;
            srend.color = (agentModeOn) ? Color.black : Color.white;
        }
    }
    [SerializeField] private float switchTime;
    [SerializeField] private bool _switchingOutfits = false;
    public bool switchingOutfits { get { return _switchingOutfits; } }
    
    // Use this for initialization
    void Start () {
        Instance = this;
        playerMove = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        srend = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        ProcessPCInputs();
	}

    private void OnDisable() {
        _moveVector = Vector2.zero;
    }

    private void ProcessPCInputs() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _moveVector = new Vector2(horizontal, vertical);

        _lookVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (Input.GetButton("Fire1")) {
            playerAttack.MainAction();
        }
        if (Input.GetButtonDown("Fire2") && !switchingOutfits) {
            StartCoroutine(SwitchOutfits());
        }
    }

    IEnumerator SwitchOutfits() {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Debug.Log("Switching outfits...");
        _switchingOutfits = true;
        float time = 0f;
        bool origVal = agentModeOn;
        while(time < switchTime) {
            if((time / switchTime) >= 0.5f && origVal == agentModeOn) { AgentModeOn = !agentModeOn; }
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        _switchingOutfits = false;
        Debug.Log("Finished switching outfits!");
    }
}
