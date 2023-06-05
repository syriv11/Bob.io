//#pragma warning disable

using UnityEngine;

namespace BobIO
{
    public class ProjectContext : Singleton<ProjectContext>
    {
        [Header("References")]
        public Camera PlayerCamera;

        [Space]
        public InputManager InputManager;
        //public GameManager GameManager;   // useless
        //public ScoreManager ScoreManager; // useless
        public SpawnManager SpawnManager;
        public UiManager UiManager;

#pragma warning disable CS8632
        public Player? PlayerOwner { get; private set; }
#pragma warning restore CS8632

        protected override void Awake()
        {
            base.Awake();

            InputManager.gameObject.SetActive(false);
        }

        public void SetPlayerOwner(Player playerOwner)
        {
            PlayerOwner = playerOwner;

            playerOwner.SetNicknameServerRpc(UiManager.GetPlayerNickname());

            InputManager.gameObject.SetActive(true);
            PlayerCamera.GetComponent<PlayerCamera>().SetPlayerTarget(playerOwner);
        }
    }
}