using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionController : MonoBehaviour
{
    /*
     *  This pops up a transition screen while a new scene is being loaded
     *  Made as a singleton.. but probably shouldn't be. 
     *  
     *  Should extend to enable multiple backgrounds, etc.
     */
    public static SceneTransitionController instance;

    [SerializeField] Image blackoutPanel;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float transitionDuration = 0.5f;
    [SerializeField] float waitTime = 2f;
    [SerializeField] GameObject canvas;

    enum STATE
    {
        FADE_IN,
        WAIT,
        FADE_OUT,
        DONE
    }

    [SerializeField] STATE state = STATE.DONE;
    float timer = 0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneController.instance.onTransitionStart += HandleTransition;
        SceneController.instance.onTransitionEnd += EndTransition;
    }
    private void OnDestroy()
    {
        SceneController.instance.onTransitionStart -= HandleTransition;
        SceneController.instance.onTransitionEnd -= EndTransition;
    }

    private void Update()
    {
        // These are debug controls for testing. Delete em.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneController.instance.TransitionToScene(SceneController.SCENE.MAIN_GAME);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneController.instance.TransitionToScene(SceneController.SCENE.MAIN_MENU);
        }
        HandleState();
    }

    void HandleState()
    {
        switch (state)
        {
            case STATE.FADE_IN:
                FadeIn();
                break;
            case STATE.WAIT:
                Wait();
                break;
            case STATE.FADE_OUT:
                FadeOut();
                break;
            case STATE.DONE:
            default: break;
        }
    }

    void SetAlphas(float newAlpha)
    {
        blackoutPanel.color = new Vector4(blackoutPanel.color.r, blackoutPanel.color.g, blackoutPanel.color.b, newAlpha);
        loadingText.color = new Vector4(loadingText.color.r, loadingText.color.g, loadingText.color.b, newAlpha);
    }

    void FadeIn()
    {
        float newAlpha = Mathf.Lerp(0f, 1f, timer / transitionDuration);
        timer += Time.deltaTime;
        if ((timer / transitionDuration) >= 0.99)
        {
            newAlpha = 1.0f;
            state = STATE.WAIT;
            timer = 0f;
            SceneController.instance.LoadNextScene();
        }
        SetAlphas(newAlpha);
    }
    void Wait()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            state = STATE.FADE_OUT;
            timer = 0f;
        }
    }

    void FadeOut()
    {
        float newAlpha = Mathf.Lerp(1f, 0f, timer / transitionDuration);
        timer += Time.deltaTime;
        if ((timer / transitionDuration) >= 0.99)
        {
            newAlpha = 0f;
            timer = 0f;
            SceneController.instance.EndTransition();
        }
        SetAlphas(newAlpha);
    }

    void HandleTransition(SceneController.SCENE nextScene)
    {
        if (!state.Equals(STATE.DONE)) return;
        StartCoroutine(BeginTransition());
    }
    IEnumerator BeginTransition()
    {
        canvas.SetActive(true);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        state = STATE.FADE_IN;
        timer = 0f;
    }


    void EndTransition(SceneController.SCENE nextScene)
    {
        state = STATE.DONE;
        timer = 0f;
        canvas.SetActive(false);
    }
}