using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Piece;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Grid
{
    public class Grid : MonoBehaviour
    {
        public int GridRows => gameSettingsScriptableObject.gridRows;
        public int GridColumns => gameSettingsScriptableObject.gridColumns;
        private int GridOffset => gameSettingsScriptableObject.gridOffset;
        private List<GameObject> GemPrefabs => gameSettingsScriptableObject.gemPrefabs;

        public GameObject[,] GemsGrid { get; private set; }
        [SerializeField]
        private Transform gridTransform;
        [SerializeField]
        internal GameSettingsScriptableObject gameSettingsScriptableObject;

        public void LoopThruGrid(Action<int, int> callback)
        {
            for (var row = 0; row < GridRows; row++)
            {
                for (var column = 0; column < GridColumns; column++)
                {
                    callback.Invoke(column, row);
                }
            }
        }
        
        private void LoopThruGrid(Action<int, int, List<GameObject>> callback, List<GameObject> gameObjectList)
        {
            for (var row = 0; row < GridRows; row++)
            {
                for (var column = 0; column < GridColumns; column++)
                {
                    callback.Invoke(column, row, gameObjectList);
                }
            }
        }
        
        internal bool LoopThruGrid(Func<int, int, bool> callback)
        {
            for (var row = 0; row < GridRows; row++)
            {
                for (var column = 0; column < GridColumns; column++)
                {
                    var result = callback.Invoke(column, row);
                    if(result) return true;
                }
            }
            return false;
        }

        internal void BuildGrid()
        {
            GemsGrid = new GameObject[GridRows, GridColumns];
            LoopThruGrid(MakeGrid, GemPrefabs);
        }

        private void MakeGrid(int column, int row, List<GameObject> gemObjects)
        {
            var gem = GenerateNonMatchingGem(column, row, gemObjects);
            Vector2 auxPos = new Vector2(column, (row * -1) + GridOffset);
            InstantiateGem(column, row, auxPos, gem);
        }

        private GameObject GenerateNonMatchingGem(int column, int row, List<GameObject> gemList)
        {
            var gemNumber = Random.Range(0, gemList.Count);

            while (CheckForMatches(column, row, gemList[gemNumber]))
            {
                gemNumber = Random.Range(0, gemList.Count);
            }

            return gemList[gemNumber];
        }
        
        private bool CheckForMatchesOnColumn(int column, int row, GameObject gemToCheck)
        {
            if (GemsGrid[row - 1, column] == null || GemsGrid[row - 2, column] == null) return false;
            
            return GemsGrid[row - 1, column].CompareTag(gemToCheck.tag) 
                   && GemsGrid[row - 2, column].CompareTag(gemToCheck.tag);
        }

        private bool CheckForMatchesOnRow(int column, int row, GameObject gemToCheck)
        {
            if (GemsGrid[row, column - 1] == null || GemsGrid[row, column - 2] == null) return false;
            
            return GemsGrid[row, column - 1].CompareTag(gemToCheck.tag) 
                   && GemsGrid[row, column - 2].CompareTag(gemToCheck.tag);
        }

        private void InstantiateGem(int column, int row, Vector2 gemPos, GameObject gemPrefab)
        {
            var gemObject = Instantiate(gemPrefab, gemPos, Quaternion.identity, gridTransform);
            var gem = gemObject.GetComponent<Gem>();
            gem.Constructor(column, row);

            GemsGrid[row, column] = gemObject;
        }

        internal void ShuffleGrid()
        {
            var gemsToShuffle = GetGemObjectsList();
            LoopThruGrid(DoShuffle, gemsToShuffle);
        }

        private void DoShuffle(int column, int row, List<GameObject> gemList)
        {
            var gem = GenerateNonMatchingGem(column, row, gemList);
            ShuffleGems(column, row, gem, gemList);
        }
        
        private void ShuffleGems(int column, int row, GameObject gemObject, List<GameObject> gemsToShuffle)
        {
            var gem = gemObject.GetComponent<Gem>();
            gem.Column = column;
            gem.Row = row;
            GemsGrid[row, column] = gemObject;
            gemsToShuffle.Remove(gemObject);
        }
        
        private bool CheckForMatches(int column, int row, GameObject gemToCheck)
        {
            if (gemToCheck == null) return false;
            if (row <= 1 && column <= 1) return false;
            
            return (row > 1 && CheckForMatchesOnColumn(column, row, gemToCheck))
                   || (column > 1 && CheckForMatchesOnRow(column, row, gemToCheck));
        }

        private List<GameObject> GetGemObjectsList()
        {
            var gemObjects = new List<GameObject>();
            LoopThruGrid(MakeGemList, gemObjects);
            return gemObjects;
        }

        private void MakeGemList(int column, int row, List<GameObject> gemList)
        {
            if (GemsGrid[row, column] != null)
            {
                gemList.Add(GemsGrid[row, column]);
            }
        }

        internal void SpawnGems()
        {
            LoopThruGrid(DoSpawnGems);
        }

        private void DoSpawnGems(int column, int row)
        {
            if (GemsGrid[row, column] != null) return;
                    
            var auxPos = new Vector2(column, (row*-1) + GridOffset);
            var gemNumber = Random.Range(0, GemPrefabs.Count);

            InstantiateGem(column, row, auxPos, GemPrefabs[gemNumber]);
        }
    }
}
