using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

internal interface IPlayerInputState {
    public IPlayerInputState HandleInput(PlayerMouseInput playerMouseInput, CoveredFields coveredFields);
}

internal class DroppingInputState : IPlayerInputState {
    public IPlayerInputState HandleInput(PlayerMouseInput playerMouseInput, CoveredFields coveredFields) {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        coveredFields.CoverFieldIfPossible(mousePosition);
        playerMouseInput.OnDragEnd.Invoke();
        return new IdleInputState();
    }
}

internal class DraggingInputState : IPlayerInputState {
    private readonly Vector3 _startDragPosition;

    public DraggingInputState(Vector3 startDragPosition) {
        _startDragPosition = startDragPosition;
    }
    
    public IPlayerInputState HandleInput(PlayerMouseInput playerMouseInput, CoveredFields coveredFields) {
        if (!Input.GetMouseButton(0)) {
            return new DroppingInputState();
        }

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var draggingDistance = _startDragPosition - mousePosition;
        if (draggingDistance.magnitude <= 1.1) {
            playerMouseInput.OnDragging.Invoke(new Vector2(mousePosition.x, mousePosition.z));
        }
        return this;
    }
}

internal class IdleInputState : IPlayerInputState {
    public IPlayerInputState HandleInput(PlayerMouseInput playerMouseInput, CoveredFields coveredFields) {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!Input.GetMouseButton(0) || !coveredFields.IsWorldCoordinateInCoveredField(mousePosition)) {
            return this;
        }
        playerMouseInput.OnDragStart.Invoke();
        return new DraggingInputState(mousePosition);
    }
}

public class PlayerMouseInput : MonoBehaviour {

    public UnityEvent OnDragStart;
    public UnityEvent<Vector2> OnDragging;
    public UnityEvent OnDragEnd;
    
    private CoveredFields _coveredFields;
    private IPlayerInputState _inputState;

    private void OnEnable() {
        _coveredFields = GetComponent<CoveredFields>();
        _inputState = new IdleInputState();
    }

    private void Update() {
        _inputState = _inputState.HandleInput(this, _coveredFields);
    }
}
