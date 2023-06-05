using UnityEngine;

namespace BobIO
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Rigidbody2D _rigidbody2D;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 direction)
        {
            Vector3 currentMovementDirection = new Vector3(direction.x, direction.y, 0.0f).normalized;

            _rigidbody2D.AddForce(currentMovementDirection * _speed);
        }
    }
}