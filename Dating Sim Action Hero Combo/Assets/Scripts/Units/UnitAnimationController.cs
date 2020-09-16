using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AnimationStatus {
    Started,
    InProgress,
    Completed
}

public interface IAnimationController {

    void UpdateState(AnimationStateData data);
    void OverrideAnimationController(AnimatorOverrideController overrideController);

    event Action<AnimationStatus> OnAnimationStatusUpdated;
}

[System.Serializable]
public class AnimationStateData {
    [SerializeField] private string _animationName;
    [SerializeField] private List<string> _triggers = new List<string>();
    [SerializeField] private List<string> _resetTriggers = new List<string>();
    [SerializeField] private List<StringBoolPair> _bools = new List<StringBoolPair>();
    [SerializeField] private List<StringIntPair> _ints = new List<StringIntPair>();
    [SerializeField] private List<StringFloatPair> _floats = new List<StringFloatPair>();

    public string AnimationName => _animationName;
    public IReadOnlyList<string> Triggers => _triggers;
    public IReadOnlyList<string> ResetTriggers => _resetTriggers;
    public IReadOnlyList<StringBoolPair> Bools => _bools;
    public IReadOnlyList<StringIntPair> Ints => _ints;
    public IReadOnlyList<StringFloatPair> Floats => _floats;
}

public class UnitAnimationController : MonoBehaviour, IAnimationController, IUnitComponent
{
    public event Action<AnimationStatus> OnAnimationStatusUpdated;

    [SerializeField] private Animator _animator;

    private bool _active = true;

    public void Initialize() {
        GameEventsManager.Pause.Subscribe(OnGamePaused);
    }

    public void Dispose() {
        GameEventsManager.Pause.Unsubscribe(OnGamePaused);
    }

    private void OnGamePaused(bool paused) {
        _active = !paused;
    }

    public void UpdateState(AnimationStateData data) {
        if (!string.IsNullOrEmpty(data.AnimationName)) {
            _animator.Play(data.AnimationName);
            return;
        }
        for(int i = 0; i < data.Triggers.Count; i++) {
            _animator.SetTrigger(data.Triggers[i]);
        }
        for(int i = 0; i < data.ResetTriggers.Count; i++) {
            _animator.ResetTrigger(data.ResetTriggers[i]);
        }
        for(int i = 0; i < data.Bools.Count; i++) {
            _animator.SetBool(data.Bools[i].Key, data.Bools[i].Value);
        }
        for(int i = 0; i < data.Ints.Count; i++) {
            _animator.SetInteger(data.Ints[i].Key, data.Ints[i].Value);
        }
        for(int i = 0; i < data.Floats.Count; i++) {
            _animator.SetFloat(data.Floats[i].Key, data.Floats[i].Value);
        }
    }

    public void OverrideAnimationController(AnimatorOverrideController overrideController) {

    }

    private void UpdateAnimationStatus(AnimationStatus status) {
        OnAnimationStatusUpdated?.Invoke(status);
    }
}
