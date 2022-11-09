using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Piece;

namespace Match3.Grid {
    public class FindMatches : MonoBehaviour
    {
        [SerializeField]
        private float _waitSeconds;

        private GridManager gridManager;

        void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        private void OnEnable()
        {
            ServiceLocator.Provide(this);
        }

        public void FindMatchesOnGrid()
        {
            StartCoroutine(FindMatchesOnGridCo());
        }
        
        private IEnumerator FindMatchesOnGridCo()
        {
            yield return new WaitForSeconds(_waitSeconds);
            
            gridManager.LoopThruGrid(DoFindMatchesOnGrid);
        }

        private void DoFindMatchesOnGrid(int column, int row)
        {
            var currentGem = gridManager.GemsGrid[row, column];
            if (currentGem == null) return;
            
            if (column > 0 && column < gridManager.GridColumns - 1)
            {
                var leftGem = gridManager.GemsGrid[row, column - 1];
                var rightGem = gridManager.GemsGrid[row, column + 1];

                CompareGems(currentGem, leftGem, rightGem);
            }

            if (row > 0 && row < gridManager.GridRows - 1)
            {
                var upperGem = gridManager.GemsGrid[row - 1, column];
                var lowerGem = gridManager.GemsGrid[row + 1, column];

                CompareGems(currentGem, upperGem, lowerGem);
            }
        }

        private void CompareGems(Gem currentGem, Gem gem1, Gem gem2)
        {
            if (gem1 == null || gem2 == null) return;
            if (gem1.MyGemType != currentGem.MyGemType 
               || gem2.MyGemType != currentGem.MyGemType) return;
            
            gem1.HasMatch = true;
            gem2.HasMatch = true;
            currentGem.HasMatch = true;
        }

        public bool CheckForMatches()
        {
            var result = gridManager.LoopThruGrid(DoCheckForMatches);
            return result;
        }

        private bool DoCheckForMatches(int column, int row)
        {
            if (gridManager.GemsGrid[row, column] == null) return false;
                    
            if (row < gridManager.GridRows - 2)
            {
                if (gridManager.GemsGrid[row + 1, column] != null && gridManager.GemsGrid[row + 2, column] != null)
                {
                    if (gridManager.GemsGrid[row + 1, column].MyGemType == gridManager.GemsGrid[row, column].MyGemType 
                        && gridManager.GemsGrid[row + 2, column].MyGemType == gridManager.GemsGrid[row, column].MyGemType)
                    {
                        return true;
                    }
                }
            }

            if (column < gridManager.GridColumns - 2)
            {
                if (gridManager.GemsGrid[row, column + 1] != null && gridManager.GemsGrid[row, column + 2] != null)
                {
                    if (gridManager.GemsGrid[row, column + 1].MyGemType == gridManager.GemsGrid[row, column].MyGemType
                        && gridManager.GemsGrid[row, column + 2].MyGemType == gridManager.GemsGrid[row, column].MyGemType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GemsMatchedOnGrid()
        {
            var result = gridManager.LoopThruGrid(HasMatchesOnGrid);
            return result;
        }
        private bool HasMatchesOnGrid(int column, int row)
        {
            return gridManager.GemsGrid[row, column] != null 
                   && gridManager.GemsGrid[row, column].HasMatch;
        }
    }
}
