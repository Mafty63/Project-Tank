using System;
using System.Collections;
using System.Collections.Generic;
using ProjectTank;
using ProjectTank.Utilities;
using Unity.Netcode;
using UnityEngine;

public class GameManager : SingletonNetworkBehaviour<GameManager>
{
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [Serializable]
    public struct PlayerTeamData
    {
        public TeamId teamId;
    }

    public enum TeamId
    {
        TeamA,
        TeamB
    }

    [SerializeField] private List<Transform> teamASpawnPositions;
    [SerializeField] private List<Transform> teamBSpawnPositions;
    [SerializeField] private List<Transform> playerPrefabs;

    private List<Transform> availableTeamASpawnPositions;
    private List<Transform> availableTeamBSpawnPositions;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private Dictionary<ulong, PlayerTeamData> playerTeamDataDictionary;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 90f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool autoTestGamePausedState;

    protected override void Awake()
    {
        base.Awake();
        playerTeamDataDictionary = new Dictionary<ulong, PlayerTeamData>();
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
        availableTeamASpawnPositions = new List<Transform>(teamASpawnPositions);
        availableTeamBSpawnPositions = new List<Transform>(teamBSpawnPositions);
    }

    private void Start()
    {
        // GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        // GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayerForClient(clientId);
        }
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        if (!playerTeamDataDictionary.TryGetValue(clientId, out PlayerTeamData playerTeamData))
        {
            playerTeamData = new PlayerTeamData { teamId = (UnityEngine.Random.value > 0.5f) ? TeamId.TeamA : TeamId.TeamB };
            playerTeamDataDictionary[clientId] = playerTeamData;
        }

        Transform spawnPosition = GetRandomSpawnPosition(playerTeamData.teamId);

        if (spawnPosition != null)
        {
            Transform playerTransform = Instantiate(playerPrefabs[(int)playerTeamData.teamId], spawnPosition.position, spawnPosition.rotation);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    public void RespawnPlayer(ulong clientId)
    {
        if (playerTeamDataDictionary.TryGetValue(clientId, out PlayerTeamData playerTeamData))
        {
            Transform spawnPosition = GetRandomSpawnPosition(playerTeamData.teamId);

            if (spawnPosition != null)
            {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
                {
                    NetworkObject playerNetworkObject = client.PlayerObject;

                    playerNetworkObject.transform.position = spawnPosition.position;
                    playerNetworkObject.transform.rotation = spawnPosition.rotation;

                    playerNetworkObject.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"No available spawn position for team {playerTeamData.teamId} for client {clientId}");
            }
        }
    }

    private Transform GetRandomSpawnPosition(TeamId teamId)
    {
        List<Transform> availablePositions = (teamId == TeamId.TeamA) ? availableTeamASpawnPositions : availableTeamBSpawnPositions;

        if (availablePositions.Count == 0)
        {
            Debug.LogWarning($"No available spawn positions for {teamId}");
            return null;
        }

        // Pilih posisi acak dan hapus dari daftar posisi yang tersedia
        int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
        Transform chosenPosition = availablePositions[randomIndex];
        availablePositions.RemoveAt(randomIndex);

        return chosenPosition;
    }


    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();

            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();

            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                // This player is paused
                isGamePaused.Value = true;
                return;
            }
        }

        // All players are unpaused
        isGamePaused.Value = false;
    }

}