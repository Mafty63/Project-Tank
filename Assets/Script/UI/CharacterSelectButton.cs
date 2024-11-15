using System.Collections;
using System.Collections.Generic;
using ProjectTank;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private int characterId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    public int CharacterId => characterId;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            CharacterSelectionHandler.Instance.ChangeCharacter(this);
        });
    }

    private void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        OnUpdateNetwork();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        OnUpdateNetwork();
    }

    private void OnUpdateNetwork()
    {

    }

    public void Selected(bool cond)
    {
        selectedGameObject.SetActive(cond);
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
    public void AssignTeam(int selection)
{
    if (selection % 2 == 1)
    {
        // Assign to Team 1
        Debug.Log("Assigned to Team 1");
    }
    else
    {
        // Assign as an Enemy
        Debug.Log("Assigned as an Enemy");
    }
}

}