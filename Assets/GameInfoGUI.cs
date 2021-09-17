using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoGUI : MonoBehaviour {
    public GameState gameState;

    public void Start() {
        if (gameState == null) {
            Debug.Log("Somehow there is no game state.");
            return;
        }
        gameState.OnPlayerMovesChanged += OnPlayerMadeMove;
    }

    public void OnPlayerMadeMove(int moves) {
        Debug.Log("Moves: " + moves);
    }
}
