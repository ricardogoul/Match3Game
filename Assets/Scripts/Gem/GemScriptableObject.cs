using Match3.Piece;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Gem")]
public class GemScriptableObject : ScriptableObject
{
    public Sprite gemSprite;
    public int gemBaseValue;
    public Gem.GemType gemType;
}
