using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /**
     * This isn't the greatest system since it allows
     * other scripts to modify the stage that's being loaded.
     * 
     * Instead, perhaps the scene transition should be passed in as a child object.
     * That way there's only one scene transition - they're coupled anyway so
     * it doesn't make sense to "decouple" them with events.
     * 
     * Will refactor this later if necessary.
     */
    public static SceneController instance;

    public enum SCENE
    {
        MAIN_MENU = 0,
        MAIN_GAME = 1,
        GAME_OVER = 2,
    }
    enum TRANSITION_STATE { START, LOADING, DONE }

    [SerializeField] TRANSITION_STATE state = TRANSITION_STATE.DONE;
    SCENE nextSceneIndex;

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

    /**
     * Sets the next scene index using the scene enum
     * emits an event to tell the Transition UI to start hiding the screen,
     * lock player input, and fade out / crossfade to the correct music.
     */
    public event Action<SCENE> onTransitionStart;
    public void TransitionToScene(SCENE _nextScene)
    {
        state = TRANSITION_STATE.START;
        nextSceneIndex = _nextScene;
        onTransitionStart?.Invoke(_nextScene);
    }


    /**
     * Begins loading the next scene. Used by the scene transition only.
     */
    public event Action onSceneLoading;
    public void LoadNextScene()
    {
        state = TRANSITION_STATE.LOADING;
        SceneManager.LoadScene((int)nextSceneIndex);
        onSceneLoading?.Invoke();
    }

    /**
     * Emits an event indicating that the next scene has loaded
     * to tell the transition UI to animate back out and re-enable
     * the player controller, and crossfade to the correct music
     */
    public event Action<SCENE> onTransitionEnd;
    public void EndTransition()
    {
        state = TRANSITION_STATE.DONE;
        onTransitionEnd?.Invoke(nextSceneIndex);
    }
}