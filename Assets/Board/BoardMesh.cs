using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMesh : MonoBehaviour {
    public BoardState BoardState;
    public int ColorIndex;

    private Texture2D fieldTexture;
    private Material material;

    private void OnEnable() {
        BoardState.OnFieldChanged += OnFieldChanged;
    }

    void Start() {
        if (BoardState is null) {
            Debug.LogError("No board state set!");
            return;
        }

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var mesh = new Mesh();
        var boardSize = BoardState.BoardSize;

        var vertices = new Vector3[4] {
            new Vector3(boardSize * -0.5f, 0, boardSize * -0.5f),
            new Vector3(boardSize * 0.5f, 0, boardSize * -0.5f),
            new Vector3(boardSize * -0.5f, 0, boardSize * 0.5f),
            new Vector3(boardSize * 0.5f, 0, boardSize * 0.5f)
        };
        mesh.vertices = vertices;

        var tris = new int[6] {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = tris;
        meshFilter.mesh = mesh;

        fieldTexture = new Texture2D(boardSize, boardSize, TextureFormat.RGBA32, false) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        for (var i = 0; i < boardSize; i++) {
            for (var j = 0; j < boardSize; j++) {
                fieldTexture.SetPixel(i, j,
                    BoardState.IsFieldOccupiedWithColor(i, j, ColorIndex) ? Color.white : Color.black);
            }
        }

        fieldTexture.Apply();
        material = GetComponent<Renderer>().material;

        material.SetTexture("_FieldTexture", fieldTexture);
        material.SetInt("_BoardSize", boardSize);
        material.SetColor("_Color", BoardState.GetColorForIndex(ColorIndex));
    }

    public void OnFieldChanged(int column, int row) {
        fieldTexture.SetPixel(column, row,
            BoardState.IsFieldOccupiedWithColor(column, row, ColorIndex) ? Color.white : Color.black);
        fieldTexture.Apply();
    }

    public void SetDraggableCircle(bool draggable) {
        var position = transform.position;
        transform.position = new Vector3(
            position.x,
            draggable ? 1.0f : 0.0f,
            position.z
        );
        material.SetInt("_AddingCircle", draggable ? 1 : 0);
    }

    public void SetDraggableCirclePosition(Vector2 position) {
        material.SetVector("_DraggingCirclePosition", new Vector4(position.x, 0, position.y, 0));
    }

    public void SetDraggableCircleTargetColor(Color color) {
        material.SetColor("_AddingCircleColor", color);
    }
}