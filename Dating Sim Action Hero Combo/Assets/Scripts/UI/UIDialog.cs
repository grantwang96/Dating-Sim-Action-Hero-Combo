using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIDialog : UIObject
{
    [SerializeField] private Text _textField;
    [SerializeField] private Button _continueButton;

    public event Action OnContinue;

    public override bool Initialize() {

        _continueButton.onClick.AddListener(OnContinuePressed);

        return base.Initialize();
    }

    public override void CleanUp() {
        base.CleanUp();

        _continueButton.onClick.RemoveAllListeners();
    }

    public override void Display() {
        gameObject.SetActive(true);
        SetDialogInfo();
        base.Display();
    }

    public override void Hide() {
        gameObject.SetActive(false);
        base.Hide();
    }

    private void SetDialogInfo() {
        DialogData initData = DialogManager.Instance.CurrentDialogData;
        _textField.text = initData.Text;
    }

    private void OnContinuePressed() {
        OnContinue?.Invoke();
    }
}
