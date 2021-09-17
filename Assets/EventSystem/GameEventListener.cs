using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameEventListener<P> : MonoBehaviour {
    public GameEvent<P> Event;
    public UnityEvent<P> Response;

    private void OnEnable() {
        Event.Subscribe(this);
    }

    private void OnDisable() {
        Event.Unsubscribe(this);
    }

    public void OnEventEmitted(P payload) {
        Response.Invoke(payload);
    }
}