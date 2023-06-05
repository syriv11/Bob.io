using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobIO 
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float _playerSmoothDuration;

        private Player _targetPlayer;
        private Vector3 _cameraOffset;
        private Vector3 _currentVelocity = Vector3.zero;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            // Init variables
            _cameraOffset = transform.position;
            //_targetPlayer = ProjectContext.Instance.PlayerOwner;

            // Init events

        }

        void Update()
        {
            if (!_targetPlayer)
                return;

            if (_targetPlayer.IsAlive)
            {
                FollowTarget(GetCameraPositionNextToPlayer(), _playerSmoothDuration);
            }
        }

        public void SetPlayerTarget(Player player)
        {
            _targetPlayer = player;
        }

        private void FollowTarget(Vector3 targetPosition, float smoothDuration)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothDuration);
        }

        public Vector3 GetCameraPositionNextToPlayer() => new Vector3(_targetPlayer.transform.position.x, _targetPlayer.transform.position.y, _cameraOffset.z);
    }
}