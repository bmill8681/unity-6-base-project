using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettingsSO : ScriptableObject
{

    [Range(0f, 1f)] public float MasterVolume;
    [Range(0f, 1f)] public float MusicVolume;
    [Range(0f, 1f)] public float SfxVolume;
}