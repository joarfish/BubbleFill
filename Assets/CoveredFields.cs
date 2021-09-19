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
    private int _coveredFieldsCount;

    public UnityEvent OnValidMove;
    public UnityEvent OnBoardCovered;

    private void Start() {
        if (boardState == null) {
            return;
        }

        _coveredFields = new bool[boardState.BoardSize, boardState.BoardSize];
        _coveredFieldsCount = 0;
        _currentColorIndex = boardState.GetFieldColorIndex(0, 0);
        CalculateCoveredFields();

        OnSetDominantColor.Invoke(_currentColorIndex);
    }

    private void OnDisable() {
        _coveredFields = null;
    }

    public bool IsWorldCoordinateInCoveredField(Vector3 position) {
        return WorldCoordinateToFieldIndex(position) switch {
            var (column, row) => _coveredFields[column, row],
            _ => false
        };
    }

    public void CoverFieldIfPossible(Vector3 position) {
        if (!(WorldCoordinateToFieldIndex(position) is var (column, row))) {
            return;
        }
        if (_coveredFields[column, row]) {
            return;
        }

        // Todo: Add check if allowed
        _coveredFields[column, row] = true;
        _coveredFieldsCount++;
        SwitchCoveredColor(boardState.GetFieldColorIndex(column, row));
        CalculateCoveredFields();
        OnValidMove.Invoke();
        if (_coveredFieldsCount == boardState.BoardSize * boardState.BoardSize) {
            Debug.Log("Board is fully covered! " + _coveredFieldsCount + "/" + (boardState.BoardSize * boardState.BoardSize));
            OnBoardCovered.Invoke();
        }
        else {
            Debug.Log("Covered Fields: " + _coveredFieldsCount);
        }
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

    private (int, int)? WorldCoordinateToFieldIndex(Vector3 position) {
        var column = (int)Math.Floor(position.x + boardState.BoardSize * 0.5);
        var row = (int)Math.Floor(position.z + boardState.BoardSize * 0.5);
        if (column < 0 || column >= boardState.BoardSize || row < 0 || row >= boardState.BoardSize) {
            return null;
        }

        return (column, row);
    }

    private void CalculateCoveredFields() {
        for (var row = 0; row < boardState.BoardSize; row++) {
            for (var column = 0; column < boardState.BoardSize; column++) {
                CoverField(column, row);
            }
        }

        // To catch all cases we need to iterate over the whole board once more.
        // This is a bit dumb, but it works:
        for (var row = boardState.BoardSize - 1; row >= 0; row--) {
            for (var column = boardState.BoardSize - 1; column >= 0; column--) {
                CoverField(column, row);
            }
        }
    }

    private void CoverField(int column, int row) {
        if (row == 0 && column == 0) {
            // This field is covered by default
            if (!_coveredFields[0, 0]) {
                _coveredFieldsCount++;                
            }
            _coveredFields[0, 0] = true;
            return;
        }

        var shouldBeCovered = ((column > 0 && _coveredFields[column - 1, row]) ||
                               (column < boardState.BoardSize - 1 && _coveredFields[column + 1, row]) ||
                               (row > 0 && _coveredFields[column, row - 1]) ||
                               (row < boardState.BoardSize - 1 && _coveredFields[column, row + 1])
                              ) &&
                              boardState.GetFieldColorIndex(column, row) == _currentColorIndex;

        if (_coveredFields[column, row] || !shouldBeCovered) {
            return;
        }

        _coveredFields[column, row] = true;
        _coveredFieldsCount++;
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