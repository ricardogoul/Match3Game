using System;
using System.Collections;
using UnityEngine;
using Match3.Piece;
using Match3.UI;

namespace Match3.Grid
{
    public class GridManager : Grid
    {
        private float WaitForSeconds => gameSettingsScriptableObject.waitForSeconds;
        private float ExplosionEffectTimer => gameSettingsScriptableObject.explosionEffectTimer;
        private float TimeToShuffle => gameSettingsScriptableObject.timeToShuffle;

        private int streakValue = 1;
        private GameState currentState;

        private void OnEnable()
        {
            ServiceLocator.Provide(this);
        }

        private void Start()
        {
            BuildGrid();
            currentState = GameState.CantMove;
        }

        public void DestroyFoundMatch()
        {
            LoopThruGrid(DoDestroyFoundMatch);
            StartCoroutine(DropRow());
        }

        private void DoDestroyFoundMatch(int column, int row)
        {
            if (GemsGrid[row, column] != null)
            {
                DestroyMatches(row , column);
            }
        }
        
        private void DestroyMatches(int row, int column)
        {
            var gem = GemsGrid[row, column].GetComponent<Gem>();
            if (!gem.HasMatch) return;
            
            DisplayExplosionEffect(row, column);
            gem.ResetGem();
            GemsGrid[row, column].SetActive(false);
            ServiceLocator.GetGemPool().ReturnGemToPool(GemsGrid[row, column]);
            GemsGrid[row, column] = null;
            Score.HandleIncreaseScoreDelegate?.Invoke(gem.GemBaseValue * streakValue);
        }
        
        private void DisplayExplosionEffect(int row, int column)
        {
            StartCoroutine(DisplayEffectCo(row, column));
            ServiceLocator.GetSoundManager().PlayExplodeGemSound();
        }

        private IEnumerator DisplayEffectCo(int row, int column)
        {
            var explosionEffect = ServiceLocator.GetGemExplosionPool().GetExplosion();
            explosionEffect.gameObject.transform.position = new Vector3(column, row * -1, 0);
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.Play();
            yield return new WaitForSeconds(ExplosionEffectTimer);
            explosionEffect.gameObject.SetActive(false);
        }

        private void SwitchGems(int row, int column, Vector2 direction)
        {
            GameObject holder = GemsGrid[row + (int)direction.y, column + (int)direction.x];
            GemsGrid[row + (int)direction.y, column + (int)direction.x] = GemsGrid[row, column];
            GemsGrid[row, column] = holder;
        }        

        private bool SwitchAndCheckForMatch(int row, int column, Vector2 direction)
        {
            SwitchGems(row, column, direction);
            var foundMatchOnGrid = ServiceLocator.GetFindMatches().CheckForMatches();
            SwitchGems(row, column, direction);
            return foundMatchOnGrid;
        }

        private bool IsDeadLocked()
        {
            var result = LoopThruGrid(CheckIfNotDeadlocked);
            return !result;
        }
        
        private bool CheckIfNotDeadlocked(int column, int row)
        {
            if (GemsGrid[row, column] == null) return false;
                    
            if (column < GridColumns - 1)
            {
                if(SwitchAndCheckForMatch(row, column, Vector2.right))
                {
                    return true;
                }
            }
        
            if (row < GridRows - 1)
            {
                if (SwitchAndCheckForMatch(row, column, Vector2.up))
                {
                    return true;
                }
            }

            return false;
        }

        public void ShuffleGems()
        {
            ShuffleGrid();

            if (currentState == GameState.Move)
                ServiceLocator.GetSoundManager().PlaySwapGemsSound();

            if (IsDeadLocked())
                Invoke(nameof(ShuffleGems), TimeToShuffle);
        }

        private IEnumerator DropRow()
        {
            int nullSpots = 0;

            for (int column = 0; column < GridColumns; column++)
            {
                for (int row = GridRows - 1; row >= 0; row--)
                {
                    if (GemsGrid[row, column] == null)
                    {
                        nullSpots++;
                    }
                    else if (nullSpots > 0)
                    {
                        GemsGrid[row, column].GetComponent<Gem>().PreviousRow += nullSpots;
                        GemsGrid[row, column].GetComponent<Gem>().Row += nullSpots;
                        GemsGrid[row, column] = null;
                    }
                }
                nullSpots = 0;
            }

            yield return new WaitForSeconds(WaitForSeconds);

            StartCoroutine(SpawnGemsCo());
        }

        private IEnumerator SpawnGemsCo()
        {
            SpawnGems();
            yield return new WaitForSeconds(WaitForSeconds);

            while (ServiceLocator.GetFindMatches().GemsMatchedOnGrid())
            {
                streakValue ++;
                DestroyFoundMatch();
                yield return new WaitForSeconds(WaitForSeconds * 4);
            }

            if (IsDeadLocked())
            {
                Invoke(nameof(ShuffleGems), 0.7f);
                Debug.Log("IS DEADLOCKED!!!");
            }

            streakValue = 1;
            yield return new WaitForSeconds(WaitForSeconds * 2);
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
