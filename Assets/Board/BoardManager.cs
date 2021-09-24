using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public BoardState boardState;
    public GameObject boardColorPrefab;
    private readonly List<BoardMesh> _boardMeshes = new List<BoardMesh>();
    private int _currentColorIndex = 0;
    private int? _draggingColorIndex = null;

    void OnEnable() {
        for (var i = 0; i < boardState.Colors.Count; i++) {
            var boardColor = Instantiate(boardColorPrefab);
            var boardMesh = boardColor.GetComponentInChildren<BoardMesh>();
            boardMesh.BoardState = boardState;
            boardMesh.ColorIndex = i;
            _boardMeshes.Add(boardMesh);
        }
    }

    public void SetDominantColor(int colorIndex) {
        foreach (var boardMesh in _boardMeshes) {
            boardMesh.SetDraggableCircle(false);
        }

        _currentColorIndex = colorIndex;
    }

    public void OnEnableDragCircle(Vector2 position) {
        var colorIndex = boardState.GetColorIndexForPosition(position);
        var boardMesh = _boardMeshes[colorIndex];
        boardMesh.SetDraggableCircle(true);
        _draggingColorIndex = colorIndex;
    }

    public void OnDisableDragCircle() {
        if (_draggingColorIndex is null) {
            return;
        }
        var boardMesh = _boardMeshes[_draggingColorIndex.Value];
        boardMesh.SetDraggableCircle(false);
    }

    public void OnSetDragCirclePosition(Vector2 position) {
        if (_draggingColorIndex is null) {
            return;
        }
        var boardMesh = _boardMeshes[_draggingColorIndex.Value];
        boardMesh.SetDraggableCircleTargetColor(boardState.GetColorForPosition(position));
        position.x += boardState.BoardSize * 0.5f;
        position.y += boardState.BoardSize * 0.5f;
        boardMesh.SetDraggableCirclePosition(position);
    }
}