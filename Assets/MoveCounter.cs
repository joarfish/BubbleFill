using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCounter : MonoBehaviour {
    public GameState gameState;

    private void OnEnable() {
        gameState.OnPlayerMovesChanged += HandlePlayerMovesChanged;
    }

    private void HandlePlayerMovesChanged(int moves) {
        var textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.SetText(moves.ToString());
    }
}
