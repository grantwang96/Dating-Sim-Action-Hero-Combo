using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public static PlayerUI Instance;

    public Image playerHealth;
    public Image playerArmor;

    [SerializeField] private string[] dialogue;
    [SerializeField] private int dialogueIndex;
    [SerializeField] private string currentSpeaker;

    [SerializeField] private GameObject textBox;
    [SerializeField] private Text speakerNameText;
    [SerializeField] private Text speakerTextBox;
    [Range(.01f, .3f)] [SerializeField] private float textSpeed;
    Coroutine textBoxDisplayAnim;

    private void Awake() {
        Instance = this;
    }

    /// <summary>
    /// Displays the dialog box with speaker name and speaker text
    /// </summary>
    /// <param name="speakerName"></param>
    /// <param name="speakerText"></param>
    public void DisplayTextBox(string speakerName, string[] speakerDialogue) {
        textBox.gameObject.SetActive(true);
        PlayerInput.Instance.enabled = false;

        currentSpeaker = speakerName;
        dialogue = speakerDialogue;
        dialogueIndex = 0;

        if(textBoxDisplayAnim != null) { StopCoroutine(textBoxDisplayAnim); }
        textBoxDisplayAnim = StartCoroutine(textBoxRoutine(currentSpeaker, dialogue[dialogueIndex]));
    }
    IEnumerator textBoxRoutine(string speakerName, string speakerText) {
        speakerNameText.text = speakerName;
        speakerTextBox.text = "";
        for(int i = 0; i < speakerText.Length; i++) {
            speakerTextBox.text += speakerText[i];
            yield return new WaitForSeconds(textSpeed);
        }
        textBoxDisplayAnim = null;
    }
    private void ExitTextBox() {
        if (textBoxDisplayAnim != null) { StopCoroutine(textBoxDisplayAnim); }
        textBox.gameObject.SetActive(false);
        PlayerInput.Instance.enabled = true;
    }
    public void NextDialogue() {
        dialogueIndex++;
        if(dialogueIndex == dialogue.Length) { ExitTextBox(); return; }
        
        if (textBoxDisplayAnim != null) { StopCoroutine(textBoxDisplayAnim); }
        textBoxDisplayAnim = StartCoroutine(textBoxRoutine(currentSpeaker, dialogue[dialogueIndex]));
    }
}
