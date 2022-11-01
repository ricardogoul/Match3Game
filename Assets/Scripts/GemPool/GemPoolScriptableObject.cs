using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GemPool", fileName = "GemPoolSettingsSO")]
public class GemPoolScriptableObject : ScriptableObject
{
    public List<GameObject> gemPrefabs;
}
