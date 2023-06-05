using UnityEngine;

namespace BobIO {
    public class PlayerCursor : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _camera = ProjectContext.Instance.PlayerCamera;
        }

        public Vector2 GetCursorDirectionVector(Vector3 playerPosition)
        {
            var playerScreenPosition = _camera.WorldToScreenPoint(playerPosition);

            return (Input.mousePosition - playerScreenPosition).normalized;
        }
    }
}