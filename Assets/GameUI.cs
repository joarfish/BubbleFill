using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {
    public GameState gameState;
    public GameObject winningScreen;
    public GameObject loosingScreen;

    private void OnEnable() {
        if (winningScreen != null) {
            winningScreen.SetActive(false);            
        }
        if (loosingScreen != null) {
            loosingScreen.SetActive(false);            
        }

        gameState.OnPlayerMovesChanged += CheckMaxMovesReached;
    }

    public void HandleBoardCovered() {
        if (gameState.moveLimit >= gameState.PlayerMoves && winningScreen != null) {
            winningScreen.SetActive(true);
        }
    }
    
    private void CheckMaxMovesReached(int moves) {
        if (gameState.moveLimit < moves && loosingScreen != null) {
            loosingScreen.SetActive(true);
        }
    }
}