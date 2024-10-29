using UnityEngine;
using Unity.Netcode;

public class LocalCameraController : NetworkBehaviour
{
    public GameObject playerCamera;
    public GameObject playerVirtualCamera;

    private void Start()
    {
        if (!IsOwner)
        {
            playerCamera.SetActive(false);
            playerVirtualCamera.SetActive(false);
        }
        else
        {
            playerCamera.SetActive(true);
            playerVirtualCamera.SetActive(true);
        }
    }
}
