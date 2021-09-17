using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;

public class CoveredFields : MonoBehaviour {
    public BoardState boardState;
    public UnityEvent<int> OnSetDominantColor;

    private bool[,] _coveredFields;
    private int _currentColorIndex;

    public UnityEvent OnValidMove;

    private void Start() {
        if (boardState == null) {
            return;
        }

        _coveredFields = new bool[boardState.BoardSize, boardState.BoardSize];
        _currentColorIndex = boardState.GetFieldColorIndex(0, 0);
        CalculateCoveredFields();

        Debug.Log("Dominant Color Index: " + _currentColorIndex);
        OnSetDominantColor.Invoke(_currentColorIndex);
    }

    private void OnDisable() {
        _coveredFields = null;
    }

    public bool IsWorldCoordinateInCoveredField(Vector3 position) {
        var (column, row) = WorldCoordinateToFieldIndex(position);

        if (column < 0 || column >= boardState.BoardSize || row < 0 || row >= boardState.BoardSize) {
            return false;
        }

        return _coveredFields[column, row];
    }

    public void CoverFieldIfPossible(Vector3 position) {
        var (column, row) = WorldCoordinateToFieldIndex(position);

        if (_coveredFields[column, row]) {
            return;
        }

        // Todo: Add check if allowed
        _coveredFields[column, row] = true;
        SwitchCoveredColor(boardState.GetFieldColorIndex(column, row));
        CalculateCoveredFields();
        OnValidMove.Invoke();
    }

    private void SwitchCoveredColor(int colorIndex) {
        for (var row = 0; row < boardState.BoardSize; row++) {
            for (var column = 0; column < boardState.BoardSize; column++) {
                if (_coveredFields[column, row]) {
                    boardState.SetFieldColor(column, row, colorIndex);
                }
            }
        }

        _currentColorIndex = colorIndex;
        OnSetDominantColor.Invoke(_currentColorIndex);
    }

    private (int, int) WorldCoordinateToFieldIndex(Vector3 position) {
        var column = (int)Math.Floor(position.x + boardState.BoardSize * 0.5);
        var row = (int)Math.Floor(position.z + boardState.BoardSize * 0.5);
        return (column, row);
    }

    private void CalculateCoveredFields() {
        // Todo: There are still some cases not covered!
        for (var row = 0; row < boardState.BoardSize; row++) {
            for (var column = 0; column < boardState.BoardSize; column++) {
                if (row == 0 && column == 0) {
                    // This field is covered by default
                    _coveredFields[column, row] = true;
                    continue;
                }

                _coveredFields[column, row] = ((column > 0 && _coveredFields[column - 1, row]) ||
                                               (column < boardState.BoardSize - 1 && _coveredFields[column + 1, row]) ||
                                               (row > 0 && _coveredFields[column, row - 1]) ||
                                               (row < boardState.BoardSize - 1 && _coveredFields[column, row + 1])
                                               ) &&
                                              boardState.GetFieldColorIndex(column, row) == _currentColorIndex;
            }
        }
    }

    private void OnDrawGizmos() {
        if (_coveredFields == null) {
            return;
        }

        for (var row = 0; row < boardState.BoardSize; row++) {
            for (var column = 0; column < boardState.BoardSize; column++) {
                if (_coveredFields[column, row]) {
                    Gizmos.DrawCube(
                        new Vector3(column - 0.5f * boardState.BoardSize, 2.0f, row - 0.5f * boardState.BoardSize),
                        new Vector3(1, 0.1f, 1));
                }
            }
        }
    }
}