using System.Collections.Generic;
using Match3.Piece;
using UnityEngine;
using Random = UnityEngine.Random;

public class GemPool : MonoBehaviour
{
    [SerializeField]
    private Transform poolHolder;
    [SerializeField]
    private GemPoolScriptableObject gemPoolScriptableObject;

    private int row;
    private int column;
    private List<Gem> gemsPool;

    private void Awake()
    {
        gemsPool = new List<Gem>();
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

    public Gem GetPooledGem()
    {
        if (gemsPool.Count == 0)
            CreateSingleGemAndAddToPool();
        
        var randomNumber = Random.Range(0, gemsPool.Count);
        var selectedGem = gemsPool[randomNumber];
        gemsPool.Remove(selectedGem);
        return selectedGem;
    }
    
    private void CreateSingleGemAndAddToPool()
    {
        var randomNumber = Random.Range(0, gemPoolScriptableObject.gemPrefabs.Count);
        InstantiateGem(gemPoolScriptableObject.gemPrefabs[randomNumber]);
    }

    public void ReturnGemToPool(Gem gemToReturn)
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

    private void InstantiateGem(GameObject gemPrefab)
    {
        var gemObject = Instantiate(gemPrefab, poolHolder.position, Quaternion.identity, poolHolder);
        gemObject.SetActive(false);
        var gem = gemObject.GetComponent<Gem>();
        gemsPool.Add(gem);
    }
}
