using Match3.Grid;
using Match3.Piece;
using Match3.Sounds;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
        
    private static GameManager gameManager;
    private static SoundManager soundManager;
    private static FindMatches findMatches;
    private static GridManager gridManager;
    private static GemPool gemPool;
    private static GemExplosionPool gemExplosionPool;

    public static GameManager GetGameManager() => gameManager;
    public static SoundManager GetSoundManager() => soundManager;
    public static FindMatches GetFindMatches() => findMatches;
    public static GridManager GetGridManager() => gridManager;
    public static GemPool GetGemPool() => gemPool;
    public static GemExplosionPool GetGemExplosionPool() => gemExplosionPool;
    
    public static void Provide(GameManager service)
    { 
        if(gameManager != null) return;
        gameManager = service;
    }
    
    public static void Provide(SoundManager service)
    { 
        if(soundManager != null) return;
        soundManager = service;
    }

    public static void Provide(FindMatches service)
    {
        if(findMatches != null) return;
        findMatches = service;
    }

    public static void Provide(GridManager service)
    {
        if(gridManager != null) return;
        gridManager = service;
    }

    public static void Provide(GemPool service)
    {
        if(gemPool != null) return;
        gemPool = service;
    }

    public static void Provide(GemExplosionPool service)
    {
        if(gemExplosionPool != null) return;
        gemExplosionPool = service;
    }
}
