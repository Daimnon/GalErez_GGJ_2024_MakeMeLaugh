using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetupManager : MonoBehaviour
{
    private static PlayerSetupManager _instance;
    public static PlayerSetupManager Instance => _instance;

    [SerializeField] private int _maxPlayers = 4;
    public int MaxPlayers => _maxPlayers;

    [SerializeField] private Transform[] _spawnsTr;
    [SerializeField] private List<PlayerInput> _players = new();
    [SerializeField] private MultipleTargetCamera _mTC;

    [SerializeField] private PlayerInputManager _playerInputManager;
    public PlayerInputManager PlayerInputManager => _playerInputManager;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += OnPlayerJoin;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }
    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= OnPlayerJoin;
        _playerInputManager.onPlayerLeft -= OnPlayerLeft;
    }
    #region Unity Events
    private void OnPlayerJoin(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} has joined!");
        _players.Add(playerInput);

        Transform playerTr = playerInput.transform;
        playerTr.position = _spawnsTr[_players.Count - 1].position;

        _mTC.Targets.Add(playerTr.GetComponent<PlayerController>().Rb2D.transform);
    }
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} has left!");
        _mTC.Targets.Remove(playerInput.GetComponent<PlayerController>().Rb2D.transform);
        _players.Remove(playerInput);

    }
    #endregion
}
