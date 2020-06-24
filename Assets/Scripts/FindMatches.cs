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

        //private List<GameObject> _matches = new List<GameObject>();

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
