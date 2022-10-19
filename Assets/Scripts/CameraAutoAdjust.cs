using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.Grid;

namespace Match3.Settings
{
    public class CameraAutoAdjust : MonoBehaviour
    {
        [SerializeField]
        private GridManager gridManager;

        [SerializeField]
        private float _cameraOffset;
        [SerializeField]
        private float _padding;
        private float _aspectRatio;
        [SerializeField]
        private float _yOffset;

        void Start()
        {
            _aspectRatio = Camera.main.aspect;
        }

        void Update()
        {
            if (gridManager != null)
            {
                AdjustCameraPos(gridManager.GridColumns - 1, gridManager.GridRows - 1);
            }
        }

        private void AdjustCameraPos(float xAxis, float yAxis)
        {
            Vector3 posToSet = new Vector3(xAxis/2, ((yAxis/2) - _yOffset)*-1, _cameraOffset);
            transform.position = posToSet;
            
            Camera.main.orthographicSize = (gridManager.GridRows/2 + _padding);
        }
    }
}
