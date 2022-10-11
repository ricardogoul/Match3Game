using Match3.Grid;
using Match3.UI;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
        
    private static GameManager gameManager;

    public static GameManager GetGameManager() => gameManager;
    public static void Provide(GameManager service)
    { 
        if(gameManager != null) return;
        gameManager = service;
    }
}
