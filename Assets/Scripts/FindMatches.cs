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

        void Start()
        {
            gridManager = GetComponentInParent<GridManager>();
        }

        public void FindMatchesOnGrid()
        {
            StartCoroutine(FindMatchesOnGridCo());
        }

        private void CompareGems(GameObject currentGem, GameObject gem1, GameObject gem2)
        {
            if (gem1 != null && gem2 != null)
            {
                if (gem1.CompareTag(currentGem.tag) && gem2.CompareTag(currentGem.tag))
                {
                    gem1.GetComponent<Gem>().HasMatch = true;
                    gem2.GetComponent<Gem>().HasMatch = true;
                    currentGem.GetComponent<Gem>().HasMatch = true;
                }
            }
        }

        public bool CheckForMatches()
        {
            for (int i = 0; i < gridManager.GridRows; i++)
            {
                for (int j = 0; j < gridManager.GridColumns; j++)
                {
                    if (gridManager.GemsGrid[i, j] != null)
                    {
                        if (i < gridManager.GridRows - 2)
                        {
                            if (gridManager.GemsGrid[i + 1, j] != null && gridManager.GemsGrid[i + 2, j] != null)
                            {
                                if (gridManager.GemsGrid[i + 1, j].CompareTag(gridManager.GemsGrid[i, j].tag) && gridManager.GemsGrid[i + 2, j].CompareTag(gridManager.GemsGrid[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }

                        if (j < gridManager.GridColumns - 2)
                        {
                            if (gridManager.GemsGrid[i, j + 1] != null && gridManager.GemsGrid[i, j + 2] != null)
                            {
                                if (gridManager.GemsGrid[i, j + 1].CompareTag(gridManager.GemsGrid[i, j].tag) && gridManager.GemsGrid[i, j + 2].CompareTag(gridManager.GemsGrid[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        

        public bool GemsMatchedOnGrid()
        {
            for (int i = 0; i < gridManager.GridRows; i++)
            {
                for (int j = 0; j < gridManager.GridColumns; j++)
                {
                    if (gridManager.GemsGrid[i, j] != null)
                    {
                        if (gridManager.GemsGrid[i, j].GetComponent<Gem>().HasMatch)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private IEnumerator FindMatchesOnGridCo()
        {
            yield return new WaitForSeconds(_waitSeconds);
            for (int i = 0; i < gridManager.GridRows; i++)
            {
                for (int j = 0; j < gridManager.GridColumns; j++)
                {
                    GameObject currentGem = gridManager.GemsGrid[i, j];

                    if (currentGem != null)
                    {
                        if (j > 0 && j < gridManager.GridColumns - 1)
                        {
                            GameObject leftGem = gridManager.GemsGrid[i, j - 1];
                            GameObject rightGem = gridManager.GemsGrid[i, j + 1];

                            CompareGems(currentGem, leftGem, rightGem);
                        }

                        if (i > 0 && i < gridManager.GridRows - 1)
                        {
                            GameObject upGem = gridManager.GemsGrid[i - 1, j];
                            GameObject downGem = gridManager.GemsGrid[i + 1, j];

                            CompareGems(currentGem, upGem, downGem);
                        }
                    }
                }
            }
        }
    }
}
