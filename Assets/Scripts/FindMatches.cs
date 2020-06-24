using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Piece;

namespace Match3.Grid {
    public class FindMatches : MonoBehaviour
    {
        [SerializeField]
        private float _waitSeconds;

        private GridGenerator _gridGenerator;

        void Start()
        {
            _gridGenerator = GetComponentInParent<GridGenerator>();
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
            for (int i = 0; i < _gridGenerator.Height; i++)
            {
                for (int j = 0; j < _gridGenerator.Width; j++)
                {
                    if (_gridGenerator.GemsGrid[i, j] != null)
                    {
                        if (i < _gridGenerator.Height - 2)
                        {
                            if (_gridGenerator.GemsGrid[i + 1, j] != null && _gridGenerator.GemsGrid[i + 2, j] != null)
                            {
                                if (_gridGenerator.GemsGrid[i + 1, j].CompareTag(_gridGenerator.GemsGrid[i, j].tag) && _gridGenerator.GemsGrid[i + 2, j].CompareTag(_gridGenerator.GemsGrid[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }

                        if (j < _gridGenerator.Width - 2)
                        {
                            if (_gridGenerator.GemsGrid[i, j + 1] != null && _gridGenerator.GemsGrid[i, j + 2] != null)
                            {
                                if (_gridGenerator.GemsGrid[i, j + 1].CompareTag(_gridGenerator.GemsGrid[i, j].tag) && _gridGenerator.GemsGrid[i, j + 2].CompareTag(_gridGenerator.GemsGrid[i, j].tag))
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
            for (int i = 0; i < _gridGenerator.Height; i++)
            {
                for (int j = 0; j < _gridGenerator.Width; j++)
                {
                    if (_gridGenerator.GemsGrid[i, j] != null)
                    {
                        if (_gridGenerator.GemsGrid[i, j].GetComponent<Gem>().HasMatch)
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
            for (int i = 0; i < _gridGenerator.Height; i++)
            {
                for (int j = 0; j < _gridGenerator.Width; j++)
                {
                    GameObject currentGem = _gridGenerator.GemsGrid[i, j];

                    if (currentGem != null)
                    {
                        if (j > 0 && j < _gridGenerator.Width - 1)
                        {
                            GameObject leftGem = _gridGenerator.GemsGrid[i, j - 1];
                            GameObject rightGem = _gridGenerator.GemsGrid[i, j + 1];

                            CompareGems(currentGem, leftGem, rightGem);
                        }

                        if (i > 0 && i < _gridGenerator.Height - 1)
                        {
                            GameObject upGem = _gridGenerator.GemsGrid[i - 1, j];
                            GameObject downGem = _gridGenerator.GemsGrid[i + 1, j];

                            CompareGems(currentGem, upGem, downGem);
                        }
                    }
                }
            }
        }
    }
}
