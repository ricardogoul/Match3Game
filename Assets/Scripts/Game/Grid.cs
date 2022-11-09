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

        public Gem[,] GemsGrid { get; private set; }
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
        
        private void LoopThruGrid(Action<int, int, List<Gem>> callback, List<Gem> gameObjectList)
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
            GemsGrid = new Gem[GridRows, GridColumns];
            LoopThruGrid(MakeGrid);
        }

        private void MakeGrid(int column, int row)
        {
            var gem = GetNonMatchingGemFromPool(column, row);
            Vector2 auxPos = new Vector2(column, (row * -1) + GridOffset);
            SetGem(column, row, auxPos, gem);
        }

        private Gem GetNonMatchingGemFromPool(int column, int row)
        {
            var gem = ServiceLocator.GetGemPool().GetPooledGem();

            while (CheckForMatches(column, row, gem))
            {
                ServiceLocator.GetGemPool().ReturnGemToPool(gem);
                gem = ServiceLocator.GetGemPool().GetPooledGem();
            }

            return gem;
        }
        
        private bool CheckForMatches(int column, int row, Gem gemToCheck)
        {
            if (gemToCheck == null) return false;
            if (row <= 1 && column <= 1) return false;
            
            return (row > 1 && CheckForMatchesOnColumn(column, row, gemToCheck))
                   || (column > 1 && CheckForMatchesOnRow(column, row, gemToCheck));
        }

        private bool CheckForMatchesOnColumn(int column, int row, Gem gemToCheck)
        {
            if (GemsGrid[row - 1, column] == null || GemsGrid[row - 2, column] == null) return false;

            return GemsGrid[row - 1, column].MyGemType == gemToCheck.MyGemType
                   && GemsGrid[row - 2, column].MyGemType == gemToCheck.MyGemType;
        }

        private bool CheckForMatchesOnRow(int column, int row, Gem gemToCheck)
        {
            if (GemsGrid[row, column - 1] == null || GemsGrid[row, column - 2] == null) return false;

            return GemsGrid[row, column - 1].MyGemType == gemToCheck.MyGemType
                   && GemsGrid[row, column - 2].MyGemType == gemToCheck.MyGemType;
        }

        private void SetGem(int column, int row, Vector2 gemPos, Gem gem)
        {
            gem.gameObject.transform.position = gemPos;
            gem.Constructor(column, row);
            GemsGrid[row, column] = gem;
            gem.gameObject.SetActive(true);
        }

        internal void ShuffleGrid()
        {
            var gemsToShuffle = GetGemsList();
            LoopThruGrid(DoShuffle, gemsToShuffle);
        }
        
        private List<Gem> GetGemsList()
        {
            var gems = new List<Gem>();
            LoopThruGrid(MakeGemsList, gems);
            return gems;
        }
        
        private void MakeGemsList(int column, int row, List<Gem> gemsList)
        {
            if (GemsGrid[row, column] != null)
            {
                gemsList.Add(GemsGrid[row, column]);
            }
        }

        private void DoShuffle(int column, int row, List<Gem> gemsList)
        {
            var gem = GetNonMatchingGemFromList(column, row, gemsList);
            ShuffleGems(column, row, gem, gemsList);
        }
        
        private Gem GetNonMatchingGemFromList(int column, int row, List<Gem> gems)
        {
            var gemNumber = Random.Range(0, gems.Count);

            while (CheckForMatches(column, row, gems[gemNumber]))
            {
                gemNumber = Random.Range(0, gems.Count);
            }

            return gems[gemNumber];
        }
        
        private void ShuffleGems(int column, int row, Gem gem, List<Gem> gemsToShuffle)
        {
            gem.Column = column;
            gem.Row = row;
            GemsGrid[row, column] = gem;
            gemsToShuffle.Remove(gem);
        }

        internal void SpawnGems()
        {
            LoopThruGrid(DoSpawnGems);
        }

        private void DoSpawnGems(int column, int row)
        {
            if (GemsGrid[row, column] != null) return;
                    
            var auxPos = new Vector2(column, (row*-1) + GridOffset);
            var gem = ServiceLocator.GetGemPool().GetPooledGem();
            SetGem(column, row, auxPos, gem);
        }
    }
}
