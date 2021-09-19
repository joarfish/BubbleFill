using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public BoardState boardState;
    public GameObject boardColorPrefab;
    private readonly List<BoardMesh> _boardMeshes = new List<BoardMesh>();
    private int _currentColorIndex = 0;

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
            var position = boardMesh.transform.position;
            boardMesh.transform.position = new Vector3(
                position.x,
                boardMesh.ColorIndex == colorIndex ? 1.0f : 0.0f,
                position.z
            );
            boardMesh.SetDraggableCircle(false);
        }

        _currentColorIndex = colorIndex;
    }

    public void OnEnableDragCircle() {
        var boardMesh = _boardMeshes[_currentColorIndex];
        boardMesh.SetDraggableCircle(true);
    }

    public void OnDisableDragCircle() {
        var boardMesh = _boardMeshes[_currentColorIndex];
        boardMesh.SetDraggableCircle(false);
    }

    public void OnSetDragCirclePosition(Vector2 position) {
        var boardMesh = _boardMeshes[_currentColorIndex];
        boardMesh.SetDraggableCircleTargetColor(boardState.GetColorForPosition(position));
        position.x += boardState.BoardSize * 0.5f;
        position.y += boardState.BoardSize * 0.5f;
        boardMesh.SetDraggableCirclePosition(position);
    }
}