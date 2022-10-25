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

        internal void BuildGrid()
        {
            GemsGrid = new GameObject[GridRows, GridColumns];
            
            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    Vector2 auxPos = new Vector2(column, (row * -1) + GridOffset);
                    int gemNumber = Random.Range(0, GemPrefabs.Count);

                    while (CheckForMatches(column, row, GemPrefabs[gemNumber]))
                    {
                        gemNumber = Random.Range(0, GemPrefabs.Count);
                    }

                    InstantiateGem(column, row, auxPos, GemPrefabs[gemNumber]);
                }
            }
        }

        internal void ShuffleGrid()
        {
            var gemsToShuffle = GetGemObjectsList();   

            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    var gemNumber = Random.Range(0, gemsToShuffle.Count);

                    while (CheckForMatches(column, row, gemsToShuffle[gemNumber]))
                    {
                        gemNumber = Random.Range(0, gemsToShuffle.Count);
                    }

                    ShuffleGems(column, row, gemNumber, gemsToShuffle);
                }
            }
        }
        
        private bool CheckForMatches(int column, int row, GameObject gemToCheck)
        {
            if (gemToCheck == null) return false;
            if (row <= 1 && column <= 1) return false;
            
            return (row > 1 && CheckForMatchesOnColumn(column, row, gemToCheck))
                   || (column > 1 && CheckForMatchesOnRow(column, row, gemToCheck));
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
        
        private List<GameObject> GetGemObjectsList()
        {
            var gemObjects = new List<GameObject>();

            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    if (GemsGrid[row, column] != null)
                    {
                        gemObjects.Add(GemsGrid[row, column]);
                    }
                }
            }

            return gemObjects;
        }

        private void ShuffleGems(int column, int row,int gemNumber, List<GameObject> gemsToShuffle)
        {
            var gem = gemsToShuffle[gemNumber].GetComponent<Gem>();
            gem.Column = column;
            gem.Row = row;
            GemsGrid[row, column] = gemsToShuffle[gemNumber];
            gemsToShuffle.Remove(gemsToShuffle[gemNumber]);
        }
        
        internal void SpawnGems()
        {
            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    if (GemsGrid[row, column] != null) continue;
                    
                    var auxPos = new Vector2(column, (row*-1) + GridOffset);
                    var gemNumber = Random.Range(0, GemPrefabs.Count);

                    InstantiateGem(column, row, auxPos, GemPrefabs[gemNumber]);
                }
            }
        }
    }
}
