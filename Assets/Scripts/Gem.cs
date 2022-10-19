using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Grid;
using Match3.Sounds;

namespace Match3.Piece
{
    public class Gem : MonoBehaviour
    {
        private Vector2 _mouseClicked;
        private Vector2 _mouseReleased;
        private Vector2 _swipeDirection;
        private Vector2 _auxPos;

        private int _row;
        private int _column;
        private int _currentRow; // Y
        private int _currentCol; // X        
        private int _previousRow;
        private int _previousCol;

        [Tooltip("Amont of seconds to wait before going back to last position when switched.")]
        [SerializeField]
        private float _waitSeconds;
        [Tooltip("Amont of seconds to be able to move.")]
        [SerializeField]
        private float _waitSecondsToMove;

        private bool _hasMatch;

        private GameObject _swapedGem;

        private GridGenerator _gridGenerator;
        private FindMatches _findMatches;
        private SoundManager _soundManager;

        void Start()
        {
            _gridGenerator = GetComponentInParent<GridGenerator>();
            _findMatches = GetComponentInParent<FindMatches>();
            _soundManager = FindObjectOfType<SoundManager>();
        }

        void Update()
        {
            _currentRow = _row;
            _currentCol = _column;                       
        }

        private void FixedUpdate()
        {
            MoveSpriteHorizontally();
            MoveSpriteVertically();

            if (_hasMatch)
            {
                SpriteRenderer gemSprite = GetComponent<SpriteRenderer>();
                gemSprite.color = Color.grey;
            }
        }

        private void OnMouseDown()
        {
            if (_gridGenerator.CurrentState == GridGenerator.GameState.move)
            {
                _mouseClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void OnMouseUp()
        {
            if (_gridGenerator.CurrentState == GridGenerator.GameState.move)
            {
                _mouseReleased = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateDirection();
            }
        }
        
        private void CalculateDirection()
        {
            _swipeDirection = _mouseReleased - _mouseClicked;
            _swipeDirection.Normalize();

            if (_swipeDirection.x != 0 || _swipeDirection.y != 0)
            {
                _previousRow = _row;
                _previousCol = _column;

                if (Mathf.Abs(_swipeDirection.x) > Mathf.Abs(_swipeDirection.y))
                {
                    if (_swipeDirection.x > 0 && _column < _gridGenerator.GridColumns - 1)
                    {
                        MoveRight();
                    }
                    else if (_swipeDirection.x < 0 && _column > 0)
                    {
                        MoveLeft();
                    }                    
                }
                else
                {
                    if (_swipeDirection.y > 0.4 && _row > 0)
                    {
                        MoveUp();
                    }
                    if (_swipeDirection.y < 0.4 && _row < _gridGenerator.GridRows - 1)
                    {
                        MoveDown();
                    }                    
                }

                if (_gridGenerator.CurrentState == GridGenerator.GameState.move)
                {
                    _soundManager.PlaySwapGemsSound();
                }
                _gridGenerator.CurrentState = GridGenerator.GameState.cantMove;
                StartCoroutine(ReturnPos());
            }
        }

        private void MoveRight()
        {
            _swapedGem = _gridGenerator.GemsGrid[_row, _column + 1];
            _swapedGem.GetComponent<Gem>()._column -= 1;
            _column += 1;
        }

        private void MoveLeft()
        {
            _swapedGem = _gridGenerator.GemsGrid[_row, _column - 1];
            _swapedGem.GetComponent<Gem>()._column += 1;
            _column -= 1;
        }

        private void MoveUp()
        {
            _swapedGem = _gridGenerator.GemsGrid[_row - 1, _column];
            _swapedGem.GetComponent<Gem>()._row += 1;
            _row -= 1;
        }

        private void MoveDown()
        {
            _swapedGem = _gridGenerator.GemsGrid[_row + 1, _column];
            _swapedGem.GetComponent<Gem>()._row -= 1;
            _row += 1;
        }

        private void MoveSpriteHorizontally()
        {
            if (Mathf.Abs(_currentCol - transform.position.x) > 0.1f)
            {
                _auxPos = new Vector2(_currentCol, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, _auxPos, 0.4f);

                if (_gridGenerator.GemsGrid[_row, _column] != gameObject)
                {
                    _gridGenerator.GemsGrid[_row, _column] = gameObject;
                }

                _findMatches.FindMatchesOnGrid();
            }
            else
            {
                _auxPos = new Vector2(_currentCol, transform.position.y);
                transform.position = _auxPos;                
            }
        }

        private void MoveSpriteVertically()
        {
            if (Mathf.Abs(_currentRow - transform.position.y) > 0.1f)
            {
                _auxPos = new Vector2(transform.position.x, _currentRow * -1);
                transform.position = Vector2.Lerp(transform.position, _auxPos, 0.4f);

                if (_gridGenerator.GemsGrid[_row, _column] != gameObject)
                {
                    _gridGenerator.GemsGrid[_row, _column] = gameObject;
                }

                _findMatches.FindMatchesOnGrid();
            }
            else
            {
                _auxPos = new Vector2(transform.position.x, _currentRow * -1);
                transform.position = _auxPos;
            }
        }

        private IEnumerator ReturnPos()
        {
            yield return new WaitForSeconds(_waitSeconds);
            
            if(_swapedGem != null)
            {
                if (!_hasMatch && !_swapedGem.GetComponent<Gem>()._hasMatch)
                {
                    if (_row != _previousRow)
                    {
                        _swapedGem.GetComponent<Gem>()._row = _row;
                        _row = _previousRow;
                    }
                    else if (_column != _previousCol)
                    {
                        _swapedGem.GetComponent<Gem>()._column = _column;
                        _column = _previousCol;
                    }

                    if (_gridGenerator.CurrentState == GridGenerator.GameState.move)
                    {
                        _soundManager.PlaySwapGemsSound();
                    }
                    yield return new WaitForSeconds(_waitSecondsToMove);
                    _gridGenerator.CurrentState = GridGenerator.GameState.move;
                }
                else
                {
                    _gridGenerator.FoundMatch();
                }

                _swapedGem = null;
            }
            else
            {
                _gridGenerator.CurrentState = GridGenerator.GameState.move;
            }
        }        

        public bool HasMatch
        {
            get { return _hasMatch; }
            set { _hasMatch = value; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int PreviousRow
        {
            get { return _previousRow; }
            set { _previousRow = value; }
        }
    }
}
