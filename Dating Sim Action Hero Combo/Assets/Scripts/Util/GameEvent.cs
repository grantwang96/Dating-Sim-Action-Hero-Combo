using System;

// the mega class that will store all the important game events that fire
public static partial class GameEventsManager {
    
}

public class GameEvent {
    public event Action GameEventAction;

    // actually fire the event
    public void Broadcast() {
        GameEventAction?.Invoke();
    }

    // register a listener
    public void Subscribe(Action action) {
        GameEventAction += action;
    }

    // unregister a listener
    public void Unsubscribe(Action action) {
        GameEventAction -= action;
    }

    // unregister all listeners
    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}

public class GameEvent<T> {
    public event Action<T> GameEventAction;

    public void Broadcast(T arg0) {
        GameEventAction?.Invoke(arg0);
    }

    public void Subscribe(Action<T> action) {
        GameEventAction += action;
    }

    public void Unsubscribe(Action<T> action) {
        GameEventAction -= action;
    }

    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}
