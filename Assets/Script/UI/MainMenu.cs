using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Lobby);
        });
    }
}
