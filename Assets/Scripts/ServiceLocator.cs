using Match3.UI;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
        
    private static ScoreManager scoreManagerService;

    public static ScoreManager GetScoreManager() => scoreManagerService;
    public static void Provide(ScoreManager service)
    { 
        if(scoreManagerService != null) return;
        scoreManagerService = service;
    }
}
