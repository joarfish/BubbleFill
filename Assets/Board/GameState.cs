using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameState", menuName = "GameLogic/GameState", order = 2)]
public class GameState : ScriptableObject {
    public int moveLimit = 20;

    public int PlayerMoves { get; private set; }

    public delegate void PlayerMovesChanged(int moves);
    public PlayerMovesChanged OnPlayerMovesChanged;

    private void OnEnable() {
        PlayerMoves = 0;
    }

    public void PlayerMakesMove() {
        OnPlayerMovesChanged(++PlayerMoves);
        Debug.Log("Player moves: "+ PlayerMoves);
    }

    public void Reset() {
        PlayerMoves = 0;
    }
}
