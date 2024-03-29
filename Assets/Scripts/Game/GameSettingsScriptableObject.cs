using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameSettings", fileName = "GameSettingsSO")]
public class GameSettingsScriptableObject : ScriptableObject
{
    public int gridRows;
    public int gridColumns;
    public int gridOffset;
    [Space]
    public List<GameObject> gemPrefabs;

    [Space]
    public float waitForSeconds;
    public float explosionEffectTimer;
    public float timeToShuffle;

}
