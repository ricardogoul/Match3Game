using Match3.Grid;
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

    public static GameManager GetGameManager() => gameManager;
    public static SoundManager GetSoundManager() => soundManager;
    
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
}
