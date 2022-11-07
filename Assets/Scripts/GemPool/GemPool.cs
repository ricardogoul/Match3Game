using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GemPool : MonoBehaviour
{
    public int CurrentPoolSize => gemsPool.Count;
    [SerializeField]
    private Transform poolHolder;
    [SerializeField]
    private GemPoolScriptableObject gemPoolScriptableObject;

    private int row;
    private int column;
    private List<GameObject> gemsPool;

    private void Awake()
    {
        gemsPool = new List<GameObject>();
    }

    private void OnEnable()
    {
        ServiceLocator.Provide(this);
    }

    private void Start()
    {
        row = ServiceLocator.GetGridManager().GridRows;
        column = ServiceLocator.GetGridManager().GridColumns;
        
        CreateGemPool();
    }

    public GameObject GetPooledGem()
    {
        if (gemsPool.Count == 0)
            CreateSingleGemAndAddToPool();
        
        var randomNumber = Random.Range(0, gemsPool.Count);
        var selectedGem = gemsPool[randomNumber];
        gemsPool.Remove(selectedGem);
        return selectedGem;
    }

    public void ReturnGemToPool(GameObject gemToReturn)
    {
        gemsPool.Add(gemToReturn);
    }

    private void CreateGemPool()
    {
        var numberOfGems = Mathf.Round(row * column) / gemPoolScriptableObject.gemPrefabs.Count + 2;
        foreach (var gem in gemPoolScriptableObject.gemPrefabs)
        {
            for (var i = 0; i < numberOfGems; i++)
            {
                InstantiateGem(gem);
            }
        }
    }

    private void InstantiateGem(GameObject gem)
    {
        var gemObject = Instantiate(gem, poolHolder.position, Quaternion.identity, poolHolder);
        gemObject.SetActive(false);
        gemsPool.Add(gemObject);
    }

    private void CreateSingleGemAndAddToPool()
    {
        var randomNumber = Random.Range(0, gemPoolScriptableObject.gemPrefabs.Count);
        InstantiateGem(gemPoolScriptableObject.gemPrefabs[randomNumber]);
    }
}
