using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public static InputController instance;
    public InputSystem_Actions PlayerInput { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        PlayerInput = new InputSystem_Actions();
    }


    public event Action<bool> OnToggleMenuActions;
    private void Start()
    {
        PlayerInput.Enable();
        PlayerInput.UI.Pause.performed += ctx => ToggleMenu();
    }

    private void OnDisable()
    {
        if (PlayerInput == null) return;
        PlayerInput.Disable();
        PlayerInput.UI.Pause.performed -= ctx => ToggleMenu();
    }

    void ToggleMenu()
    {
        if (!GameController.instance.State.Equals(GameController.GAMESTATE.PAUSED))
        {
            GameController.instance.SetState(GameController.GAMESTATE.PAUSED);
            PlayerInput.Player.Disable();
            OnToggleMenuActions?.Invoke(true);
            return;
        }

        GameController.instance.SetState(GameController.instance.PreviousState);
        PlayerInput.Player.Enable();
        OnToggleMenuActions?.Invoke(false);
    }
}