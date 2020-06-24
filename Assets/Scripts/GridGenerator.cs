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

        //private bool _checkRoundFinished;

        //[SerializeField]
        //private GameObject _gemPrefab;        
        //private GameObject[,] _tileGrid;        
        [Tooltip("Put all gems prefab in here.")]
        [SerializeField]
        private GameObject[] _gems;
        [SerializeField]
        private GameObject[,] _gemsGrid;
        [SerializeField]
        private GameObject _explosionEffect;

        [Tooltip("Grid Transform.")]
        [SerializeField]
        private Transform _grid;

        private SoundManager _soundManager;
        private ScoreManager _scoreManager;

        [SerializeField]
        private GameState _currentState;

        void Start()
        {
            _scoreManager = GetComponentInChildren<ScoreManager>();
            _soundManager = GetComponentInChildren<SoundManager>();

            _soundManager.PlayBackgroundMusic();

            //_tileGrid = new GameObject[_height, _width];
            _gemsGrid = new GameObject[_height, _width];

            BuildGrid();
            _currentState = GameState.cantMove;
    }

        private void FixedUpdate()
        {
            //if (_scoreManager.RemainingTime <= 0 && _checkRoundFinished)
            //{
            //    _checkRoundFinished = true;
            //    RoundFinished();
            //}

            //if (_scoreManager.ScoreGoalReached && !_checkRoundFinished)
            //{
            //    _checkRoundFinished = true;
            //    RoundFinished();
            //    RoundCleared();
            //}
        }

        private void BuildGrid()
        {
            for (int i = 0; i > (_height*-1); i--)
            {
                for(int j = 0; j < _width; j++)
                {
                    Vector2 auxPos = new Vector2(j, i + offset);
                    int gemNumber = Random.Range(0, _gems.Length);
                    int iterations = 0;
                        
                    while (CheckForMatches(j, i*-1, _gems[gemNumber]) && iterations < 50)
                    {
                        gemNumber = Random.Range(0, _gems.Length);
                        iterations++;
                    }
                    iterations = 0;

                    GameObject gem = Instantiate(_gems[gemNumber], auxPos, Quaternion.identity, _grid);
                    gem.GetComponent<Gem>().Row = i * -1;
                    gem.GetComponent<Gem>().Column = j;

                    //GameObject gem = Instantiate(_gems[gemNumber], new Vector3(j, i, 0), Quaternion.identity, _grid);
                    gem.name = "[" + i*-1 + "]" + "" + "[" + j + "]";

                    _gemsGrid[i*-1, j] = gem;
                }
            }
        }

        private bool CheckForMatches(int column, int row, GameObject gemToCheck)
        {
            //if (row > 1 && column > 1)
            //{
            //    if (_gemsGrid[row - 1, column].CompareTag(gemToCheck.tag) && _gemsGrid[row - 2, column].CompareTag(gemToCheck.tag))
            //    {
            //        return true;
            //    }

            //    if (_gemsGrid[row, column - 1].CompareTag(gemToCheck.tag) && _gemsGrid[row, column - 2].CompareTag(gemToCheck.tag))
            //    {
            //        return true;
            //    }
            //}
            //else if (row <=1 || column <= 1)
            //{
            //    if(row > 1)
            //    {
            //        if (_gemsGrid[row - 1, column].CompareTag(gemToCheck.tag) && _gemsGrid[row - 2, column].CompareTag(gemToCheck.tag))
            //        {
            //            return true;
            //        }                    
            //    }

            //    if(column > 1)
            //    {
            //        if (_gemsGrid[row, column - 1].CompareTag(gemToCheck.tag) && _gemsGrid[row, column - 2].CompareTag(gemToCheck.tag))
            //        {
            //            return true;
            //        }
            //    }
            //}

            if (row >= 1 || column >= 1)
            {
                if (row > 1)
                {
                    if (_gemsGrid[row - 1, column].CompareTag(gemToCheck.tag) && _gemsGrid[row - 2, column].CompareTag(gemToCheck.tag))
                    {
                        return true;
                    }
                }

                if (column > 1)
                {
                    if (_gemsGrid[row, column - 1].CompareTag(gemToCheck.tag) && _gemsGrid[row, column - 2].CompareTag(gemToCheck.tag))
                    {
                        return true;
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

        public bool GemsMatchedOnGrid()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_gemsGrid[i, j] != null)
                    {
                        if (_gemsGrid[i, j].GetComponent<Gem>().HasMatch)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void SwitchGemsForDeadlockCheck(int row, int column, Vector2 direction)
        {
            GameObject holder = _gemsGrid[row + (int)direction.y, column + (int)direction.x];
            _gemsGrid[row + (int)direction.y, column + (int)direction.x] = _gemsGrid[row, column];
            _gemsGrid[row, column] = holder;
        }

        private bool CheckForMatches()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (_gemsGrid[i, j] != null)
                    {
                        if (i < _height - 2)
                        {
                            if (_gemsGrid[i + 1, j] != null && _gemsGrid[i + 2, j] != null)
                            {
                                if (_gemsGrid[i + 1, j].CompareTag(_gemsGrid[i, j].tag) && _gemsGrid[i + 2, j].CompareTag(_gemsGrid[i, j].tag))
                                {
                                    return true;
                                }
                            }
                        }

                        if (j < _width - 2)
                        {
                            if (_gemsGrid[i, j + 1] != null && _gemsGrid[i, j + 2] != null)
                            {
                                if (_gemsGrid[i, j + 1].CompareTag(_gemsGrid[i, j].tag) && _gemsGrid[i, j + 2].CompareTag(_gemsGrid[i, j].tag))
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

        private bool SwitchAndCheckForDeadlock(int row, int column, Vector2 direction)
        {
            SwitchGemsForDeadlockCheck(row, column, direction);

            if (CheckForMatches())
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
        
        //public void RebuildGrid()
        //{
        //    for (int i = 0; i < _height; i++)
        //    {
        //        for (int j = 0; j < _width; j++)
        //        {
        //            Destroy(_gemsGrid[i, j].gameObject);
        //        }
        //    }

        //    _gemsGrid = null;
        //    BuildGrid();
        //}

        //private IEnumerator DropRow()
        //{
        //    int nullSpots = 0;

        //    for (int i = 0; i < _height; i++)
        //    {
        //        for (int j = _width-1; j >= 0; j--)
        //        {
        //            if (_gemsGrid[j, i] == null)
        //            {
        //                nullSpots++;
        //            }
        //            else if (nullSpots > 0)
        //            {
        //                _gemsGrid[j, i].GetComponent<Gem>().Row += nullSpots;
        //                _gemsGrid[j, i].GetComponent<Gem>().PreviousRow = _gemsGrid[j, i].GetComponent<Gem>().Row;
        //                _gemsGrid[j, i] = null;
        //            }
        //        }
        //        nullSpots = 0;
        //    }

        //    yield return new WaitForSeconds(_waitSeconds);

        //    StartCoroutine(SpawnGemsCo());
        //}

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
                        //_gemsGrid[i, j].GetComponent<Gem>().PreviousRow = _gemsGrid[j, i].GetComponent<Gem>().Row;
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

            while (GemsMatchedOnGrid())
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
