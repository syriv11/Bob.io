using UnityEngine;

namespace BobIO
{
    public class BotTest : MonoBehaviour
    {
        private Player _player;

        // Start is called before the first frame update
        void Start()
        {
            _player = GetComponent<Player>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_player.IsAlive)
                _player.Shoot();
        }
    }
}