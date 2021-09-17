using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<P> : ScriptableObject {
    private readonly List<GameEventListener<P>> _listeners = new List<GameEventListener<P>>();

    public void Emit(P payload) {
        // Iterate from back to front so that listeners can unsubscribe themselves
        // from event when the event is emitted:
        for (var i = _listeners.Count - 1; i >= 0; i--) {
            _listeners[i].OnEventEmitted(payload);
        }
    }

    public void Subscribe(GameEventListener<P> gameEventListener) {
        _listeners.Add(gameEventListener);
    }

    public void Unsubscribe(GameEventListener<P> gameEventListener) {
        _listeners.Remove(gameEventListener);
    }
}