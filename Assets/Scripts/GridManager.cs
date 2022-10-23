using System.Collections;
using UnityEngine;
using Match3.Piece;
using Match3.UI;

namespace Match3.Grid
{
    [RequireComponent(typeof(Grid))]
    public class GridManager : Grid
    {
        [SerializeField]
        private float waitSeconds;
        [SerializeField]
        private float waitSecondsToMove;
        [SerializeField]
        private float explosionEffectTimer;

        [SerializeField]
        private GameObject explosionEffect;

        private int streakValue = 1;

        private GameState currentState;

        private void Start()
        {
            ServiceLocator.Provide(this);

            BuildGrid();
            currentState = GameState.CantMove;
        }

        private void DestroyMatches(int row, int column)
        {
            var gem = GemsGrid[row, column].GetComponent<Gem>();
            if (!gem.HasMatch) return;
            
            DisplayExplosionEffect(row, column);
            Destroy(GemsGrid[row, column]);
            Score.HandleIncreaseScoreDelegate?.Invoke(gem.GemBaseValue * streakValue);
            GemsGrid[row, column] = null;
        }

        private void DisplayExplosionEffect(int row, int column)
        {
            GameObject explosionEffect = Instantiate(this.explosionEffect, GemsGrid[row, column].transform.position + new Vector3(0,0,-1), Quaternion.identity);
            Destroy(explosionEffect, explosionEffectTimer);
            ServiceLocator.GetSoundManager().PlayExplodeGemSound();
        }

        public void FoundMatch()
        {
            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    if (GemsGrid[row, column] != null)
                    {
                        DestroyMatches(row , column);
                    }
                }
            }

            StartCoroutine(DropRow());
        }

        

        private void SwitchGemsForDeadlockCheck(int row, int column, Vector2 direction)
        {
            GameObject holder = GemsGrid[row + (int)direction.y, column + (int)direction.x];
            GemsGrid[row + (int)direction.y, column + (int)direction.x] = GemsGrid[row, column];
            GemsGrid[row, column] = holder;
        }        

        private bool SwitchAndCheckForDeadlock(int row, int column, Vector2 direction)
        {
            SwitchGemsForDeadlockCheck(row, column, direction);

            if (ServiceLocator.GetFindMatches().CheckForMatches())
            {
                SwitchGemsForDeadlockCheck(row, column, direction);
                return true;
            }

            SwitchGemsForDeadlockCheck(row, column, direction);
            return false;
        }

        private bool IsDeadLocked()
        {
            for (int row = 0; row < GridRows; row++)
            {
                for (int column = 0; column < GridColumns; column++)
                {
                    if (GemsGrid[row, column] != null)
                    {
                        if (column < GridColumns - 1)
                        {
                            if(SwitchAndCheckForDeadlock(row, column, Vector2.right))
                            {
                                return false;
                            }
                        }

                        if (row < GridRows - 1)
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
            ShuffleGrid();

            if (currentState == GameState.Move)
                ServiceLocator.GetSoundManager().PlaySwapGemsSound();

            if (IsDeadLocked())
                Invoke(nameof(ShuffleGems), 0.7f);
        }

        private IEnumerator DropRow()
        {
            int nullSpots = 0;

            for (int j = 0; j < GridColumns; j++)
            {
                for (int i = GridRows - 1; i >= 0; i--)
                {
                    if (GemsGrid[i, j] == null)
                    {
                        nullSpots++;
                    }
                    else if (nullSpots > 0)
                    {
                        GemsGrid[i, j].GetComponent<Gem>().PreviousRow += nullSpots;
                        GemsGrid[i, j].GetComponent<Gem>().Row += nullSpots;
                        GemsGrid[i, j] = null;
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

            while (ServiceLocator.GetFindMatches().GemsMatchedOnGrid())
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
            currentState = GameState.Move;
        }

        public enum GameState
        {
            CantMove,
            Move
        }

        public GameState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }
    }
}
