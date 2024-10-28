using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    private int playerIndex;
    // [SerializeField] private GameObject readyGameObject;
    [SerializeField] private List<GameObject> playerVisualList;
    // [SerializeField] private Button kickButton;
    // [SerializeField] private TextMeshPro playerNameText;

    private int currentCharacterId = 0;


    // private void Awake()
    // {
    // kickButton.onClick.AddListener(() =>
    // {
    //     PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
    //     GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
    //     GameMultiplayer.Instance.KickPlayer(playerData.clientId);
    // });
    // }


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");

            transform.Rotate(Vector3.up, -mouseX * 100 * Time.deltaTime);
        }
    }
    private void Start()
    {
        playerIndex = GameMultiplayer.Instance.PlayerDataNetworkList.Count - 1;
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        // kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        OnUpdateNetwork();
    }

    private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        OnUpdateNetwork();
    }

    private void OnUpdateNetwork()
    {
        if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {

        }
    }

    public void UpdateCharacter(int characterId)
    {
        // if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        // {
        // readyGameObject.SetActive(true);

        // playerNameText.text = playerData.playerName.ToString();

        if (playerVisualList[currentCharacterId].activeSelf)
            playerVisualList[currentCharacterId].SetActive(false);

        currentCharacterId = characterId;
        playerVisualList[currentCharacterId].SetActive(true);
        // }
    }


    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
    }


}