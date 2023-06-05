using System;
using UnityEngine;

namespace BobIO
{
    public class InputManager : MonoBehaviour
    {
        public Action SpacePressed;

        [HideInInspector] public Vector2 InputMovement;
        [HideInInspector] public Vector2 InputMouse;

        [SerializeField] private PlayerCursor _playerCursor;

        private Player _player 
        { 
            get => ProjectContext.Instance.PlayerOwner;
        }

        public bool IsAttackPressed { get; private set; }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            
        }

        private void Update()
        {
            if (!_player.IsAlive)
                return;

            GetPlayerInput();

            HandleShoot();
        }

        private void FixedUpdate()
        {
            if (!_player.IsAlive)
                return;

            HandlePlayerMovement();
        }

        private void HandleShoot()
        {
            if (IsAttackPressed)
            {
                _player.Shoot();
            }
        }

        private void HandlePlayerMovement()
        {
            _player.Move(InputMovement);
            _player.RotatePlayerBodyToCursor(_playerCursor.GetCursorDirectionVector(_player.transform.position));
        }

        private void GetPlayerInput()
        {
            InputMovement = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            InputMouse = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

            if (Input.GetMouseButton(0))
            {
                IsAttackPressed = true;
            }
            else
            {
                IsAttackPressed = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (!_player.IsAlive)
                return;


            Gizmos.color = Color.red;
            float lineLength = 2f;

            Vector3 lineDirection = _playerCursor.GetCursorDirectionVector(_player.transform.position) * lineLength;

            Gizmos.DrawLine(_player.transform.position,
                            _player.transform.position + lineDirection);
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }
    }
}
