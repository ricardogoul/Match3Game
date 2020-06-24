using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Piece;
using Match3.UI;
using Match3.Sounds;

namespace Match3.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        [Tooltip("Height of the Grid.")]
        [SerializeField]
        private int _height;
        [Tooltip("Width of the Grid.")]
        [SerializeField]
        private int _width;
        [SerializeField]
        private int offset;        
        [SerializeField]
        private int _baseGemValue;
        private int _streakValue = 1;

        [Tooltip("Amont of seconds to wait after a match drop gems down.")]
        [SerializeField]
        private float _waitSeconds;
        [Tooltip("Amont of seconds to be able to move.")]
        [SerializeField]
        private float _waitSecondsToMove;
        [Tooltip("Amont of seconds to wait before destroying particle system.")]
        [SerializeField]
        private float _explosionEffectTimer;
        
        [Tooltip("Put all gems prefab in here.")]
        [SerializeField]
        private GameObject[] _gems;
        private GameObject[,] _gemsGrid;
        [SerializeField]
        private GameObject _explosionEffect;

        [Tooltip("Grid Transform.")]
        [SerializeField]
        private Transform _grid;

        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private FindMatches _findMatches;

        private GameState _currentState;

        void Start()
        {
            _scoreManager = GetComponentInChildren<ScoreManager>();
            _soundManager = GetComponentInChildren<SoundManager>();
            _findMatches = GetComponent<FindMatches>();

            _soundManager.PlayBackgroundMusic();

            _gemsGrid = new GameObject[_height, _width];

            BuildGrid();
            _currentState = GameState.cantMove;
        }

        private void BuildGrid()
        {
            for (int i = 0; i < _height; i++)
            {
                for(int j = 0; j < _width; j++)
                {
                    Vector2 auxPos = new Vector2(j, (i*-1) + offset);
                    int gemNumber = Random.Range(0, _gems.Length);
                    int iterations = 0;
                        
                    while (CheckForMatches(j, i, _gems[gemNumber]) && iterations < 50)
                    {
                        gemNumber = Random.Range(0, _gems.Length);
                        iterations++;
                    }
                    iterations = 0;

                    GameObject gem = Instantiate(_gems[gemNumber], auxPos, Quaternion.identity, _grid);
                    gem.GetComponent<Gem>().Row = i;
                    gem.GetComponent<Gem>().Column = j;

                    _gemsGrid[i, j] = gem;
                }
            }
        }

        public bool CheckForMatches(int column, int row, GameObject gemToCheck)
        {
            if (gemToCheck != null)
            {
                if (row > 1 || column > 1)
                {
                    if (row > 1)
                    {
                        if (_gemsGrid[row - 1, column] != null && _gemsGrid[row - 2, column] != null)
                        {
                            if (_gemsGrid[row - 1, column].CompareTag(gemToCheck.tag) && _gemsGrid[row - 2, column].CompareTag(gemToCheck.tag))
                            {
                                return true;
                            }
                        }
                    }

                    if (column > 1)
                    {
                        if (_gemsGrid[row, column - 1] != null && _gemsGrid[row, column - 2] != null)
                        {
                            if (_gemsGrid[row, column - 1].CompareTag(gemToCheck.tag) && _gemsGrid[row, column - 2].CompareTag(gemToCheck.tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void DestroyMatches(int row, int column)
        {
            if (_gemsGrid[row, column].GetComponent<Gem>().HasMatch)
            {
                GameObject explosionEffect = Instantiate(_explosionEffect, _gemsGrid[row, column].transform.position + new Vector3(0,0,-1), Quaternion.identity);
                _soundManager.PlayExplodeGemSound();
                Destroy(explosionEffect, _explosionEffectTimer);
                Destroy(_gemsGrid[row, column]);
                _scoreManager.IncreaseScore(_baseGemValue * _streakValue);
                _gemsGrid[row, column] = null;
            }
        }

        public void FoundMatch()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_gemsGrid[i, j] != null)
                    {
                        DestroyMatches(i , j);
                    }
                }
            }

            StartCoroutine(DropRow());
        }

        private void SpawnGems()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if(_gemsGrid[i, j] == null)
                    {
                        Vector2 auxPos = new Vector2(j, (i*-1) + offset);
                        int gemNumber = Random.Range(0, _gems.Length);

                        GameObject gem = Instantiate(_gems[gemNumber], auxPos, Quaternion.identity, _grid);
                        _gemsGrid[i, j] = gem;

                        gem.GetComponent<Gem>().Row = i;
                        gem.GetComponent<Gem>().Column = j;
                    }
                }
            }
        }

        

        private void SwitchGemsForDeadlockCheck(int row, int column, Vector2 direction)
        {
            GameObject holder = _gemsGrid[row + (int)direction.y, column + (int)direction.x];
            _gemsGrid[row + (int)direction.y, column + (int)direction.x] = _gemsGrid[row, column];
            _gemsGrid[row, column] = holder;
        }        

        private bool SwitchAndCheckForDeadlock(int row, int column, Vector2 direction)
        {
            SwitchGemsForDeadlockCheck(row, column, direction);

            if (_findMatches.CheckForMatches())
            {
                SwitchGemsForDeadlockCheck(row, column, direction);
                return true;
            }

            SwitchGemsForDeadlockCheck(row, column, direction);
            return false;
        }

        private bool IsDeadLocked()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_gemsGrid[i, j] != null)
                    {
                        if (j < _width - 1)
                        {
                            if(SwitchAndCheckForDeadlock(i, j, Vector2.right))
                            {
                                return false;
                            }
                        }

                        if (i < _height - 1)
                        {
                            if (SwitchAndCheckForDeadlock(i, j, Vector2.up))
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

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_gemsGrid[i, j] != null)
                    {
                        gemsToShuffle.Add(_gemsGrid[i, j]);
                    }
                }
            }

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
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
                    _gemsGrid[i, j] = gemsToShuffle[gemNumber];
                    gemsToShuffle.Remove(gemsToShuffle[gemNumber]);
                }
            }

            if (_currentState == GridGenerator.GameState.move)
            {
                _soundManager.PlaySwapGemsSound();
            }

            if (IsDeadLocked())
            {
                Invoke("ShuffleGems", 0.7f);
            }
        }

        private IEnumerator DropRow()
        {
            int nullSpots = 0;

            for (int j = 0; j < _width; j++)
            {
                for (int i = _height - 1; i >= 0; i--)
                {
                    if (_gemsGrid[i, j] == null)
                    {
                        nullSpots++;
                    }
                    else if (nullSpots > 0)
                    {
                        _gemsGrid[i, j].GetComponent<Gem>().PreviousRow += nullSpots;
                        _gemsGrid[i, j].GetComponent<Gem>().Row += nullSpots;
                        _gemsGrid[i, j] = null;
                    }
                }
                nullSpots = 0;
            }

            yield return new WaitForSeconds(_waitSeconds);

            StartCoroutine(SpawnGemsCo());
        }

        private IEnumerator SpawnGemsCo()
        {
            SpawnGems();
            yield return new WaitForSeconds(_waitSeconds);

            while (_findMatches.GemsMatchedOnGrid())
            {
                _streakValue ++;
                FoundMatch();
                yield return new WaitForSeconds(_waitSecondsToMove * 4);
            }

            if (IsDeadLocked())
            {
                Invoke("ShuffleGems", 0.7f);
                Debug.Log("IS DEADLOCKED!!!");
            }

            _streakValue = 1;
            yield return new WaitForSeconds(_waitSecondsToMove * 2);
            _currentState = GameState.move;
        }

        public enum GameState
        {
            cantMove,
            move
        }

        public GameObject[,] GemsGrid
        {
            get { return _gemsGrid; }
            set { _gemsGrid = value; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public GameState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
    }
}
