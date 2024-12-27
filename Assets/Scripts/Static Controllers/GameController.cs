using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public enum GAMESTATE
    {
        PAUSED,
        RUNNING
    }

    public GAMESTATE State { get; private set; } = GAMESTATE.RUNNING;
    public GAMESTATE PreviousState { get; private set; } = GAMESTATE.PAUSED;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public event Action<GAMESTATE, GAMESTATE> OnStateChanged;
    public void SetState(GAMESTATE newState)
    {
        PreviousState = State;
        State = newState;
        OnStateChanged?.Invoke(State, PreviousState);
    }

    public void QuitGame()
    {
        Debug.Log("I quit the game.");
        Application.Quit();
    }
}