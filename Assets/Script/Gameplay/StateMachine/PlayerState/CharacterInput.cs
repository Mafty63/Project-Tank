using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Cinemachine;

public class CharacterInput : MonoBehaviour, Controls.IRoamingActions
{
    public static CharacterInput instance;

    [Header("Camera")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoomIncrement = 20f;
    [SerializeField] private float maxZoomIncrement = 40f;
    [SerializeField] private float zoomDelayDuration = 0.1f;
    [SerializeField] private float zoomIncrement = 5f;

    [SerializeField] private float zoomStepDuration = 0.1f;


    public float minFOV = 20f;
    public float maxFOV = 40f;
    private Vector2 currentZoomIncrement = Vector2.zero;

    private bool isZooming = false;
    private float initialZoomValue;
    private float targetZoomValue;
    public bool IsAttacking { get; private set; }
    public bool IsSkill1 { get; private set; }
    public bool IsBlocking { get; private set; }
    public Vector2 MovementValue { get; private set; }
    // public Vector2 lookValue;
    public bool actionTrigger { get; private set; }
    public bool globalEnterTrigger { get; private set; }

    public event Action JumpEvent;
    public event Action DodgeEvent;
    public event Action TargetEvent;

    private Controls controls;
    private PlayerState _playerState = PlayerState.FreeLook;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }
    private void Start()
    {
        controls = new Controls();
        controls.Roaming.SetCallbacks(this);

        controls.Roaming.Enable();

        // SwitchCurrentActionMap("Roaming");
    }

    private void OnDestroy()
    {
        controls.Roaming.Disable();
    }

    private void Update()
    {
        cursorMode();
    }

    /// <summary>
    /// force to use cursor mode in specific condition
    /// </summary>
    private void cursorMode()
    {
        // if all condition below is true than force cursor to visible
        if (_playerState == PlayerState.Inventory || _playerState == PlayerState.Reward || _playerState == PlayerState.Minimap
         || _playerState == PlayerState.Crafting || _playerState == PlayerState.Dialogue || _playerState == PlayerState.Quest)
        {
            SetCursorState(false);
        }
        else
        {
            // if all condition below is true than give user change to use cursor with hold alt
            if (_playerState == PlayerState.Tutorial || _playerState == PlayerState.CutScene)
            {
                if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
                {
                    SetCursorState(false);
                }
                else
                {
                    SetCursorState(true);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
                {
                    //if user force using cursor in free look mode then user cannot move the character
                    changePlayerState(PlayerState.CursorMode);
                    HUDHandler.instance.roamingUIHandler.showNonEssentialUI();
                    SetCursorState(false);
                }
                else
                {
                    instance.changePlayerState(PlayerState.FreeLook);
                    HUDHandler.instance.roamingUIHandler.hideNonEssentialUI();
                    SetCursorState(true);
                }
            }
        }
    }

    public void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void changePlayerState(PlayerState playerState) // noted : rubah semua yang pakai fungsi ini. player state hanya bisa di rubah di user input saja
    {
        _playerState = playerState;
    }

    private void OnApplicationFocus()
    {
        SetCursorState(true);
    }






    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        if (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode)
            JumpEvent?.Invoke();
        else
            return;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        if (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode)
            DodgeEvent?.Invoke();
        else
            return;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode)
            MovementValue = context.ReadValue<Vector2>();
        else
            MovementValue = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // if (UserInput._playerState == PlayerState.FreeLook)
        // lookValue = context.ReadValue<Vector2>();

        // else
        // lookValue = Vector2.zero;
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        // Get the zoom input value from the context
        Vector2 zoomInput = context.ReadValue<Vector2>();

        targetZoomValue = Mathf.Clamp(targetZoomValue + zoomInput.y * zoomIncrement, minFOV, maxFOV);

        // Update the currentZoomIncrement based on input
        currentZoomIncrement += zoomInput;

        initialZoomValue = freeLookCamera.m_Lens.FieldOfView;
        StartCoroutine(ZoomDelay());

    }
    private IEnumerator ZoomDelay()
    {
        isZooming = true;
        yield return new WaitForSeconds(zoomDelayDuration);
        while (Mathf.Abs(freeLookCamera.m_Lens.FieldOfView - targetZoomValue) > 0.1f)
        {
            // Tentukan arah zoom (naik atau turun)
            float zoomDirection = Mathf.Sign(targetZoomValue - freeLookCamera.m_Lens.FieldOfView);

            // Tentukan nilai zoom yang akan dicapai pada langkah ini
            float newZoomValue = freeLookCamera.m_Lens.FieldOfView + zoomDirection * zoomIncrement;

            // Clamp nilai zoom agar tidak melebihi target
            newZoomValue = Mathf.Clamp(newZoomValue, minFOV, maxFOV);

            // Set nilai zoom kamera
            freeLookCamera.m_Lens.FieldOfView = newZoomValue;

            // Tunggu sebelum melanjutkan ke langkah zoom berikutnya
            yield return new WaitForSeconds(zoomStepDuration);
        }

        freeLookCamera.m_Lens.FieldOfView = targetZoomValue;

        isZooming = false;
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        if (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode)
            TargetEvent?.Invoke();
        else
            return;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _playerState == PlayerState.FreeLook)
        {
            IsAttacking = true;
        }
        else if (context.canceled && _playerState == PlayerState.FreeLook)
        {
            IsAttacking = false;
        }
    }

    public void OnSkill1(InputAction.CallbackContext context)
    {
        if (context.performed && _playerState == PlayerState.FreeLook)
        {
            IsSkill1 = true;
        }
        else if (context.canceled && _playerState == PlayerState.FreeLook)
        {
            IsSkill1 = false;
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed && (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode))
        {
            IsBlocking = true;
        }
        else if (context.canceled && (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode))
        {
            IsBlocking = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode))
        {
            IsBlocking = true;
        }
        else if (context.canceled && (_playerState == PlayerState.FreeLook || _playerState == PlayerState.CursorMode))
        {
            IsBlocking = false;
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook && context.performed)
        {
            HUDHandler.instance.openInventory();
        }
    }

    public void OnOpenMiniMap(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook && context.performed)
        {
            HUDHandler.instance.openMiniMap();
        }
    }

    public void OnOpenCharacterInfo(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook && context.performed)
        {
            HUDHandler.instance.openCharacterInfo();
        }
    }

    public void OnOpenQuest(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook && context.performed)
        {
            HUDHandler.instance.openQuest();
        }
    }

    public void OnPlayerAction(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.FreeLook && context.performed)
        {
            actionTrigger = true;
        }
        else
        {
            actionTrigger = false;
        }
    }

    public void OnGlobalEscAction(InputAction.CallbackContext context)
    {
        // noted : tambahkan state lagi jika ingin menggunakan esc
        if (_playerState == PlayerState.Inventory && context.performed)
            HUDHandler.instance.closeInventory();

        if (_playerState == PlayerState.Minimap && context.performed)
            HUDHandler.instance.closeMiniMap();

        if (_playerState == PlayerState.Quest && context.performed)
            HUDHandler.instance.closeQuest();

    }

    public void OnGlobalEnterAction(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.Dialogue && context.performed)
        {
            globalEnterTrigger = true;
        }
        else
        {
            globalEnterTrigger = false;
        }

    }

    public void OnCloseTutorial(InputAction.CallbackContext context)
    {
        if (_playerState == PlayerState.Tutorial && TutorialHandler.instance.getIsStartTutorial() && context.performed)
        {
            TutorialHandler.instance.closeCurrentTutorial();
        }
    }

    public void changeMoveAndLookToZero() // harus di rubah untuk tutorial
    {
        MovementValue = Vector2.zero;
    }
}