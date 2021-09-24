using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

internal struct FieldState {
    public bool Occupied;
    public int ColorIndex;
}

public struct FieldIndex {
    public FieldIndex(int column, int row) {
        Column = column;
        Row = row;
    }
    public int Column;
    public int Row;
}

[CreateAssetMenu(fileName = "BoardState", menuName = "GameLogic/BoardState", order = 1)]
public class BoardState : ScriptableObject {
    [Range(2, 20)]
    public int BoardSize;
    public List<Color> Colors;

    private FieldState[,] _fieldStates;

    public delegate void FieldChanged(int column, int row);
    public FieldChanged OnFieldChanged;

    private void OnEnable() {
        _fieldStates = new FieldState[BoardSize, BoardSize];
        var rand = new Random();
        var colorsCount = Colors.Count;
        for (var i = 0; i < BoardSize; i++) {
            for (var j = 0; j < BoardSize; j++) {
                _fieldStates[i, j] = new FieldState() {
                    Occupied = true,
                    ColorIndex = rand.Next(0, colorsCount)
                };
            }
        }

        var msg = "";
        for (var i = BoardSize - 1; i >= 0; i--) {
            for (var j = 0; j < BoardSize; j++) {
                var color = _fieldStates[j, i].ColorIndex switch {
                    0 => "red",
                    1 => "green",
                    2 => "lila",
                    3 => "blue",
                    4 => "yellow",
                    _ => "black"
                };
                msg += " <color="+color+">" + _fieldStates[j, i].ColorIndex + "</color> ";
            }

            msg += "\n";
        }

        Debug.Log(msg);
    }

    private void OnDisable() {
        _fieldStates = null;
    }

    public void SetFieldOccupied(int column, int row, int colorIndex) {
        _fieldStates[column, row] = new FieldState() {
            Occupied = true,
            ColorIndex = colorIndex
        };
        OnFieldChanged(column, row);
    }

    public void SetFieldUnoccupied(int column, int row) {
        _fieldStates[column, row] = new FieldState() {
            Occupied = false,
            ColorIndex = -1
        };
        OnFieldChanged(column, row);
    }

    public void SetFieldColor(int column, int row, int colorIndex) {
        _fieldStates[column, row].ColorIndex = colorIndex;
        OnFieldChanged(column, row);
    }

    public bool IsFieldOccupied(int column, int row) {
        return _fieldStates[column, row].Occupied;
    }

    public bool IsFieldOccupiedWithColor(int column, int row, int colorIndex) {
        return _fieldStates[column, row].Occupied && _fieldStates[column, row].ColorIndex == colorIndex;
    }

    public int GetFieldColorIndex(int column, int row) {
        return _fieldStates[column, row].ColorIndex;
    }

    public Color GetColorForIndex(int colorIndex) {
        return Colors[colorIndex];
    }

    public Color GetColorForPosition(Vector2 position) {
        return Colors[GetColorIndexForPosition(position)];
    }

    public int GetColorIndexForPosition(Vector2 position) {
        var column = (int)Math.Floor(position.x + 0.5f * BoardSize);
        var row = (int)Math.Floor(position.y + 0.5f * BoardSize);
        return _fieldStates[column, row].ColorIndex;
    }
}
