using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Grid;
using Match3.Sounds;

namespace Match3.Piece
{
    public class Gem : MonoBehaviour
    {
        public bool HasMatch { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int PreviousRow { get; set; }
        
        [Tooltip("Amount of seconds to wait before going back to last position when switched.")]
        [SerializeField]
        private float waitSeconds = 0.2f;
        [Tooltip("Amount of seconds to be able to move.")]
        [SerializeField]
        private float waitSecondsToMove = 0.2f;

        private const float TimeToLerp = 0.2f;
        
        private Vector2 mouseClicked;
        private Vector2 mouseReleased;

        private int previousCol;

        private Gem swappedGem;

        private GridManager gridManager;

        public void Constructor(int column, int row, GridManager gridManager)
        {
            Column = column;
            Row = row;
            this.gridManager = gridManager;
        }

        private void FixedUpdate()
        {
            MoveSpriteHorizontally();
            MoveSpriteVertically();
            ChangeColorWhenHasMatch();
        }

        private void ChangeColorWhenHasMatch()
        {
            if (!HasMatch) return;
            
            SpriteRenderer gemSprite = GetComponent<SpriteRenderer>();
            gemSprite.color = Color.grey;
        }

        private void OnMouseDown()
        {
            if (gridManager.CurrentState != GridManager.GameState.move) return;
            GetMouseClick();
        }

        private void OnMouseUp()
        {
            if (gridManager.CurrentState != GridManager.GameState.move) return;
            GetMouseRelease();
            CheckForMovement();
        }

        private void GetMouseClick()
        {
            mouseClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void GetMouseRelease()
        {
            mouseReleased = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void CheckForMovement()
        {
            var swipeDirection = CalculateSwipeDirection();
            
            if (swipeDirection.x == 0 && swipeDirection.y == 0) return;
            Move(swipeDirection);
        }
        
        private Vector2 CalculateSwipeDirection()
        {
            var swipeDirection = mouseReleased - mouseClicked;
            swipeDirection.Normalize();
            return swipeDirection;
        }

        private void Move(Vector2 swipeDirection)
        {
            PreviousRow = Row;
            previousCol = Column;

            if (IsLateralMovement(swipeDirection))
            {
                switch (swipeDirection.x)
                {
                    case > 0 when Column < gridManager.GridColumns - 1:
                        MoveRight();
                        break;
                    case < 0 when Column > 0:
                        MoveLeft();
                        break;
                }
            }
            else
            {
                switch (swipeDirection.y)
                {
                    case > 0.4f when Row > 0:
                        MoveUp();
                        break;
                    case < 0.4f when Row < gridManager.GridRows - 1:
                        MoveDown();
                        break;
                }
            } 

            if (gridManager.CurrentState == GridManager.GameState.move)
            {
                ServiceLocator.GetSoundManager().PlaySwapGemsSound();
            }
            gridManager.CurrentState = GridManager.GameState.cantMove;
            StartCoroutine(ProcessMovement());
        }
        
        private bool IsLateralMovement(Vector2 swipeDirection)
        {
            return Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y);
        }
        
        private void MoveRight()
        {
            swappedGem = gridManager.GemsGrid[Row, Column + 1].GetComponent<Gem>();
            swappedGem.Column -= 1;
            Column += 1;
        }

        private void MoveLeft()
        {
            swappedGem = gridManager.GemsGrid[Row, Column - 1].GetComponent<Gem>();
            swappedGem.Column += 1;
            Column -= 1;
        }

        private void MoveUp()
        {
            swappedGem = gridManager.GemsGrid[Row - 1, Column].GetComponent<Gem>();
            swappedGem.Row += 1;
            Row -= 1;
        }

        private void MoveDown()
        {
            swappedGem = gridManager.GemsGrid[Row + 1, Column].GetComponent<Gem>();
            swappedGem.Row -= 1;
            Row += 1;
        }
        
        private void MoveSpriteHorizontally()
        {
            var currentPosition = transform.position;
            var currentColumn = Column;
            
            if (Mathf.Abs(currentColumn - currentPosition.x) > 0.1f)
            {
                var auxPosition = new Vector2(currentColumn, currentPosition.y);
                currentPosition = Vector2.Lerp(currentPosition, auxPosition, TimeToLerp);
                transform.position = currentPosition;

                if (gridManager.GemsGrid[Row, Column] != gameObject)
                {
                    gridManager.GemsGrid[Row, Column] = gameObject;
                }

                ServiceLocator.GetFindMatches().FindMatchesOnGrid();
            }
            else
            {
                var auxTransform = transform;
                var auxPosition = new Vector2(currentColumn, auxTransform.position.y);
                auxTransform.position = auxPosition;                
            }
        }

        private void MoveSpriteVertically()
        {
            var currentPosition = transform.position;
            var currentRow = Row;
            
            if (Mathf.Abs(currentRow - currentPosition.y) > 0.1f)
            {
                var auxPosition = new Vector2(currentPosition.x, currentRow * -1);
                currentPosition = Vector2.Lerp(currentPosition, auxPosition, TimeToLerp);
                transform.position = currentPosition;

                if (gridManager.GemsGrid[Row, Column] != gameObject)
                {
                    gridManager.GemsGrid[Row, Column] = gameObject;
                }

                ServiceLocator.GetFindMatches().FindMatchesOnGrid();
            }
            else
            {
                var auxTransform = transform;
                var auxPosition = new Vector2(auxTransform.position.x, currentRow * -1);
                auxTransform.position = auxPosition;
            }
        }

        private IEnumerator ProcessMovement()
        {
            yield return new WaitForSeconds(waitSeconds);
            
            if(swappedGem != null)
            {
                if (FoundMatchAfterMovement())
                {
                    gridManager.FoundMatch();
                }
                else
                {
                    ReturnToPreviousPosition();
                    
                    if (gridManager.CurrentState == GridManager.GameState.move)
                    {
                        ServiceLocator.GetSoundManager().PlaySwapGemsSound();
                    }
                    
                    yield return new WaitForSeconds(waitSecondsToMove);
                    gridManager.CurrentState = GridManager.GameState.move;
                }

                swappedGem = null;
            }
            else
            {
                gridManager.CurrentState = GridManager.GameState.move;
            }
        }
        
        private bool FoundMatchAfterMovement()
        {
            return HasMatch || swappedGem.HasMatch;
        }

        private void ReturnToPreviousPosition()
        {
            if (Row != PreviousRow)
            {
                swappedGem.Row = Row;
                Row = PreviousRow;
            }
            else if (Column != previousCol)
            {
                swappedGem.Column = Column;
                Column = previousCol;
            }
        }
    }
}