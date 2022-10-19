using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Piece;
using Match3.UI;
using Match3.Sounds;
using TMPro;

namespace Match3.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField]
        private int gridRows;
        [SerializeField]
        private int gridColumns;
        [SerializeField]
        private int gridOffset;        
        [SerializeField]
        private int baseGemValue;

        [SerializeField]
        private float waitSeconds;
        [SerializeField]
        private float waitSecondsToMove;
        [SerializeField]
        private float explosionEffectTimer;
        
        [SerializeField]
        private GameObject[] gemPrefabs;
        [SerializeField]
        private GameObject explosionEffect;
        [SerializeField]
        private Transform gridTransform;

        private GameObject[,] gemsGrid;
        private int streakValue = 1;
        
        private FindMatches findMatches;

        private GameState currentState;

        private void Start()
        {
            findMatches = GetComponent<FindMatches>();

            gemsGrid = new GameObject[gridRows, gridColumns];

            BuildGrid();
            currentState = GameState.cantMove;
        }

        private void BuildGrid()
        {
            for (int row = 0; row < gridRows; row++)
            {
                for(int column = 0; column < gridColumns; column++)
                {
                    Vector2 auxPos = new Vector2(column, (row*-1) + gridOffset);
                    int gemNumber = Random.Range(0, gemPrefabs.Length);
                    
                    // int iterations = 0;
                    //
                    // while (CheckForMatches(column, row, gemPrefabs[gemNumber]) && iterations < 50)
                    // {
                    //     gemNumber = Random.Range(0, gemPrefabs.Length);
                    //     iterations++;
                    // }
                    // iterations = 0;

                    InstantiateGem(column, row, gemNumber, auxPos);
                }
            }
        }

        private void InstantiateGem(int column, int row, int gemNumber, Vector2 gemPos)
        {
            GameObject gem = Instantiate(gemPrefabs[gemNumber], gemPos, Quaternion.identity, gridTransform);
            gem.GetComponent<Gem>().Row = row;
            gem.GetComponent<Gem>().Column = column;

            gemsGrid[row, column] = gem;
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
            if (gemsGrid[row - 1, column] == null || gemsGrid[row - 2, column] == null) return false;
            
            return gemsGrid[row - 1, column].CompareTag(gemToCheck.tag) 
                   && gemsGrid[row - 2, column].CompareTag(gemToCheck.tag);
        }

        private bool CheckForMatchesOnRow(int column, int row, GameObject gemToCheck)
        {
            if (gemsGrid[row, column - 1] == null || gemsGrid[row, column - 2] == null) return false;
            
            return gemsGrid[row, column - 1].CompareTag(gemToCheck.tag) 
                   && gemsGrid[row, column - 2].CompareTag(gemToCheck.tag);
        }

        private void DestroyMatches(int row, int column)
        {
            if (!gemsGrid[row, column].GetComponent<Gem>().HasMatch) return;
            
            DisplayExplosionEffect(row, column);
            Destroy(gemsGrid[row, column]);
            Score.HandleIncreaseScoreDelegate?.Invoke(baseGemValue * streakValue);
            gemsGrid[row, column] = null;
        }

        private void DisplayExplosionEffect(int row, int column)
        {
            GameObject explosionEffect = Instantiate(this.explosionEffect, gemsGrid[row, column].transform.position + new Vector3(0,0,-1), Quaternion.identity);
            Destroy(explosionEffect, explosionEffectTimer);
            ServiceLocator.GetSoundManager().PlayExplodeGemSound();
        }

        public void FoundMatch()
        {
            for (int row = 0; row < gridRows; row++)
            {
                for (int column = 0; column < gridColumns; column++)
                {
                    if (gemsGrid[row, column] != null)
                    {
                        DestroyMatches(row , column);
                    }
                }
            }

            StartCoroutine(DropRow());
        }

        private void SpawnGems()
        {
            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    if(gemsGrid[i, j] == null)
                    {
                        Vector2 auxPos = new Vector2(j, (i*-1) + gridOffset);
                        int gemNumber = Random.Range(0, gemPrefabs.Length);

                        GameObject gem = Instantiate(gemPrefabs[gemNumber], auxPos, Quaternion.identity, gridTransform);
                        gemsGrid[i, j] = gem;

                        gem.GetComponent<Gem>().Row = i;
                        gem.GetComponent<Gem>().Column = j;
                    }
                }
            }
        }

        

        private void SwitchGemsForDeadlockCheck(int row, int column, Vector2 direction)
        {
            GameObject holder = gemsGrid[row + (int)direction.y, column + (int)direction.x];
            gemsGrid[row + (int)direction.y, column + (int)direction.x] = gemsGrid[row, column];
            gemsGrid[row, column] = holder;
        }        

        private bool SwitchAndCheckForDeadlock(int row, int column, Vector2 direction)
        {
            SwitchGemsForDeadlockCheck(row, column, direction);

            if (findMatches.CheckForMatches())
            {
                SwitchGemsForDeadlockCheck(row, column, direction);
                return true;
            }

            SwitchGemsForDeadlockCheck(row, column, direction);
            return false;
        }

        private bool IsDeadLocked()
        {
            for (int row = 0; row < gridRows; row++)
            {
                for (int column = 0; column < gridColumns; column++)
                {
                    if (gemsGrid[row, column] != null)
                    {
                        if (column < gridColumns - 1)
                        {
                            if(SwitchAndCheckForDeadlock(row, column, Vector2.right))
                            {
                                return false;
                            }
                        }

                        if (row < gridRows - 1)
                        {
                            if (SwitchAndCheckForDeadlock(row, column, Vector2.up))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void ShuffleGems()
        {
            List<GameObject> gemsToShuffle = new List<GameObject>();

            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    if (gemsGrid[i, j] != null)
                    {
                        gemsToShuffle.Add(gemsGrid[i, j]);
                    }
                }
            }

            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    int gemNumber = Random.Range(0, gemsToShuffle.Count);
                    int iterations = 0;

                    while (CheckForMatches(j, i, gemsToShuffle[gemNumber]) && iterations < 50)
                    {
                        gemNumber = Random.Range(0, gemsToShuffle.Count);
                        iterations++;
                    }
                    iterations = 0;

                    Gem gem = gemsToShuffle[gemNumber].GetComponent<Gem>();
                    gem.Column = j;
                    gem.Row = i;
                    gemsGrid[i, j] = gemsToShuffle[gemNumber];
                    gemsToShuffle.Remove(gemsToShuffle[gemNumber]);
                }
            }

            if (currentState == GridGenerator.GameState.move)
            {
                ServiceLocator.GetSoundManager().PlaySwapGemsSound();
            }

            if (IsDeadLocked())
            {
                Invoke("ShuffleGems", 0.7f);
            }
        }

        private IEnumerator DropRow()
        {
            int nullSpots = 0;

            for (int j = 0; j < gridColumns; j++)
            {
                for (int i = gridRows - 1; i >= 0; i--)
                {
                    if (gemsGrid[i, j] == null)
                    {
                        nullSpots++;
                    }
                    else if (nullSpots > 0)
                    {
                        gemsGrid[i, j].GetComponent<Gem>().PreviousRow += nullSpots;
                        gemsGrid[i, j].GetComponent<Gem>().Row += nullSpots;
                        gemsGrid[i, j] = null;
                    }
                }
                nullSpots = 0;
            }

            yield return new WaitForSeconds(waitSeconds);

            StartCoroutine(SpawnGemsCo());
        }

        private IEnumerator SpawnGemsCo()
        {
            SpawnGems();
            yield return new WaitForSeconds(waitSeconds);

            while (findMatches.GemsMatchedOnGrid())
            {
                streakValue ++;
                FoundMatch();
                yield return new WaitForSeconds(waitSecondsToMove * 4);
            }

            if (IsDeadLocked())
            {
                Invoke("ShuffleGems", 0.7f);
                Debug.Log("IS DEADLOCKED!!!");
            }

            streakValue = 1;
            yield return new WaitForSeconds(waitSecondsToMove * 2);
            currentState = GameState.move;
        }

        public enum GameState
        {
            cantMove,
            move
        }

        public GameObject[,] GemsGrid
        {
            get { return gemsGrid; }
            set { gemsGrid = value; }
        }

        public int GridColumns
        {
            get { return gridColumns; }
        }

        public int GridRows
        {
            get { return gridRows; }
        }

        public GameState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
    }
}
