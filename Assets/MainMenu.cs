using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] VisualElement ui;
    [SerializeField] Button playButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button backButton;
    [SerializeField] GroupBox mainMenuBox;
    [SerializeField] GroupBox settingsMenuBox;
    [SerializeField] VisualElement backgroundImage;
    bool isOnMainMenu = true;
    bool isFading = false;
    int backgroundShift = 0;
    //int totalShift = 576;
    int totalShift = 40;
    float coroutineStep = 0.025f;

    private void Awake()
    {
        ui = GetComponentInChildren<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton      = ui.Q<Button>("PlayButton");
        settingsButton  = ui.Q<Button>("SettingsButton");
        quitButton      = ui.Q<Button>("QuitButton");
        backButton      = ui.Q<Button>("BackButton");
        mainMenuBox     = ui.Q<GroupBox>("MainMenuGroup");
        settingsMenuBox = ui.Q<GroupBox>("SettingsMenuGroup");
        backgroundImage = ui.Q<VisualElement>("Background");

        playButton.clicked      += OnPressPlay;
        settingsButton.clicked  += OnPressSettings;
        backButton.clicked      += OnPressSettings;
        quitButton.clicked      += OnPressQuit;

        InitializeMenus();
    }

    private void OnDisable()
    {
        playButton.clicked      -= OnPressPlay;
        settingsButton.clicked  -= OnPressSettings;
        backButton.clicked      -= OnPressSettings;
        quitButton.clicked      -= OnPressQuit;
    }

    void InitializeMenus()
    {
        mainMenuBox.style.display = DisplayStyle.Flex;
        mainMenuBox.style.opacity = 1f;
        settingsMenuBox.style.display = DisplayStyle.None;
        settingsMenuBox.style.opacity = 0f;
    }

    void OnPressPlay()
    {
        if (isFading) return;
        SceneController.instance.TransitionToScene(SceneController.SCENE.MAIN_GAME);
    }

    void OnPressSettings()
    {
        if (isFading) return;
        StartCoroutine("CrossfadeMenu");
        StartCoroutine("ShiftBackground");
    }

    void OnPressQuit()
    {
        if (isFading) return;
        MenuFunctions.QuitGame();
    }

    IEnumerator CrossfadeMenu()
    {
        isFading = true;
        mainMenuBox.style.display = DisplayStyle.Flex;
        settingsMenuBox.style.display = DisplayStyle.Flex;
        while (isFading)
        {
            HandleFade();
            yield return new WaitForSeconds(coroutineStep);
        }
    }

    void HandleFade()
    {
        if (isOnMainMenu) 
        {
            mainMenuBox.style.opacity = mainMenuBox.style.opacity.value - 0.1f;
            settingsMenuBox.style.opacity = settingsMenuBox.style.opacity.value + 0.1f;
            ClampValues();
            if (mainMenuBox.style.opacity.value <= 0f)
            {
                mainMenuBox.style.display = DisplayStyle.None;
                isOnMainMenu = false;
                isFading = false;
            }
            return;
        }
        
        mainMenuBox.style.opacity = mainMenuBox.style.opacity.value + 0.1f;
        settingsMenuBox.style.opacity = settingsMenuBox.style.opacity.value - 0.1f;
        ClampValues();
        if (settingsMenuBox.style.opacity.value <= 0f)
        {
            settingsMenuBox.style.display = DisplayStyle.None;
            isOnMainMenu = true;
            isFading = false;
        }
        return;
    }


    /*
     *  TODO: These shifts should really be lerped.
     */
    IEnumerator ShiftBackground()
    {
        float seconds = 0.25f;
        while(seconds > 0f)
        {
            int mult = isOnMainMenu ? -1 : 1;
            int amt = Mathf.FloorToInt(totalShift * 0.05f) * mult;
            backgroundShift += amt;
            backgroundShift =  Mathf.Clamp(backgroundShift, -totalShift, 0);
            BackgroundPosition newAmt = new BackgroundPosition(BackgroundPositionKeyword.Left, backgroundShift);
            backgroundImage.style.backgroundPositionX = newAmt;
            yield return new WaitForSeconds(coroutineStep / 2);
            seconds -= coroutineStep;
        }
    }

    void ClampValues()
    {
        float mainMenuVal = Mathf.Clamp(mainMenuBox.style.opacity.value, 0f, 1f);
        float settingsMenuVal = Mathf.Clamp(settingsMenuBox.style.opacity.value, 0f, 1f);

        mainMenuBox.style.opacity = mainMenuVal;        
        settingsMenuBox.style.opacity = settingsMenuVal;
    }

}
