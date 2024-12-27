using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameSettings : MonoBehaviour
{
    [SerializeField] GameSettingsSO settings;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;


    private void Awake()
    {
        if (!settings) throw new Exception("GameSettings requires a GameSettingsSO object");
    }

    private void Start()
    {
        InputController.instance.OnToggleMenuActions += ToggleInputSubscription;
        masterVolumeSlider.value = settings.MasterVolume;
        musicVolumeSlider.value = settings.MusicVolume;
        sfxVolumeSlider.value = settings.SfxVolume;
    }

    private void OnDestroy()
    {
        InputController.instance.OnToggleMenuActions -= ToggleInputSubscription;
    }

    void ToggleInputSubscription(bool enabled)
    {

        // These may not be necessary.. need to test controller input.
        //if (enabled) InputController.instance.PlayerInput.MenuActions.Selection.performed += HandleSelectionInput;
        //else InputController.instance.PlayerInput.MenuActions.Selection.performed -= HandleSelectionInput;
    }

    void HandleSelectionInput(InputAction.CallbackContext ctx)
    {
        // If you need any specific controller function you can add it here, but it's likely not necessary.
    }

    public void SetMasterVolume()
    {
        settings.MasterVolume = masterVolumeSlider.value;
        MusicController.instance.SetMusicVolume(settings.MasterVolume * settings.MusicVolume);
    }
    public void SetMusicVolume()
    {
        settings.MusicVolume = musicVolumeSlider.value;
        MusicController.instance.SetMusicVolume(settings.MasterVolume * settings.MusicVolume);
    }

    // TODO: When a strategy is set for handling sound effects, set the sound effect volume here.
    public void SetSfxVolume() { settings.SfxVolume = sfxVolumeSlider.value; }

    public void UnpauseGame() { GameController.instance.SetState(GameController.GAMESTATE.RUNNING); }

    public void QuitGame()
    {
        GameController.instance.QuitGame();
    }

    public void GoToMainMenu()
    {
        GameController.instance.SetState(GameController.GAMESTATE.RUNNING);
        SceneController.instance.TransitionToScene(SceneController.SCENE.MAIN_MENU);
        MusicController.instance.StopMusic();
    }
}