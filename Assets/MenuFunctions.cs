using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
   public static void QuitGame()
    {
        Application.Quit();
    }

    public static void AdjustMusicVolume(float val)
    {
        GameSettingsSO musicSettings = MusicController.instance.gameSettings;
        musicSettings.MusicVolume = val;
        MusicController.instance.SetMusicVolume(musicSettings.MasterVolume * musicSettings.MusicVolume);
    }

    /**
     *  TODO: SfxController doesn't exist yet. Need to implement this once it does.
     */
    public static void AdjustSfxVolume(float val)
    {
        GameSettingsSO musicSettings = MusicController.instance.gameSettings;
        musicSettings.SfxVolume = val;
        Debug.LogWarning("Attempting to adjust sound effect volume but SfxController hasn't been created yet.");
        //SfxController.instance.SetSfxVolume(musicSettings.MasterVolume * musicSettings.SfxVolume);
    }
    public static void AdjustMasterVolume(float val)
    {
        GameSettingsSO musicSettings = MusicController.instance.gameSettings;
        musicSettings.MasterVolume = val;
        AdjustSfxVolume(musicSettings.SfxVolume);
        AdjustMusicVolume(musicSettings.MusicVolume);
    }
}

