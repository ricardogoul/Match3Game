using System.Collections;
using UnityEngine;
using Match3.Grid;

namespace Match3.Piece
{
    public class Gem : MonoBehaviour
    {
        public enum GemType
        {
            APPLE,
            BREAD,
            BUSH,
            COCONUT,
            JELLY,
            MILK,
            ORANGE
        }
        public bool HasMatch { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int PreviousRow { get; set; }
        public int GemBaseValue => gemScriptableObject.gemBaseValue;
        public GemType MyGemType => gemScriptableObject.gemType;
        
        [SerializeField]
        private GemScriptableObject gemScriptableObject;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
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

        public void Constructor(int column, int row)
        {
            Column = column;
            Row = row;
        }
        
        public void ResetGem()
        {
            HasMatch = false;
            spriteRenderer.color = Color.white;
        }

        private void OnValidate()
        {
            spriteRenderer.sprite = gemScriptableObject.gemSprite;
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
            spriteRenderer.color = Color.grey;
        }

        private void OnMouseDown()
        {
            if (ServiceLocator.GetGridManager().CurrentState != GridManager.GameState.Move) return;
            GetMouseClick();
        }

        private void OnMouseUp()
        {
            if (ServiceLocator.GetGridManager().CurrentState != GridManager.GameState.Move) return;
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
                    case > 0 when Column < ServiceLocator.GetGridManager().GridColumns - 1:
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
                    case < 0.4f when Row < ServiceLocator.GetGridManager().GridRows - 1:
                        MoveDown();
                        break;
                }
            } 

            if (ServiceLocator.GetGridManager().CurrentState == GridManager.GameState.Move)
            {
                ServiceLocator.GetSoundManager().PlaySwapGemsSound();
            }
            ServiceLocator.GetGridManager().CurrentState = GridManager.GameState.CantMove;
            StartCoroutine(ProcessMovement());
        }
        
        private bool IsLateralMovement(Vector2 swipeDirection)
        {
            return Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y);
        }
        
        private void MoveRight()
        {
            swappedGem = ServiceLocator.GetGridManager().GemsGrid[Row, Column + 1];
            swappedGem.Column -= 1;
            Column += 1;
        }

        private void MoveLeft()
        {
            swappedGem = ServiceLocator.GetGridManager().GemsGrid[Row, Column - 1];
            swappedGem.Column += 1;
            Column -= 1;
        }

        private void MoveUp()
        {
            swappedGem = ServiceLocator.GetGridManager().GemsGrid[Row - 1, Column];
            swappedGem.Row += 1;
            Row -= 1;
        }

        private void MoveDown()
        {
            swappedGem = ServiceLocator.GetGridManager().GemsGrid[Row + 1, Column];
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

                ServiceLocator.GetGridManager().GemsGrid[Row, Column] = this;
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

                ServiceLocator.GetGridManager().GemsGrid[Row, Column] = this;
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
                    ServiceLocator.GetGridManager().DestroyFoundMatch();
                }
                else
                {
                    ReturnToPreviousPosition();
                    
                    if (ServiceLocator.GetGridManager().CurrentState == GridManager.GameState.Move)
                    {
                        ServiceLocator.GetSoundManager().PlaySwapGemsSound();
                    }
                    
                    yield return new WaitForSeconds(waitSecondsToMove);
                    ServiceLocator.GetGridManager().CurrentState = GridManager.GameState.Move;
                }

                swappedGem = null;
            }
            else
            {
                ServiceLocator.GetGridManager().CurrentState = GridManager.GameState.Move;
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