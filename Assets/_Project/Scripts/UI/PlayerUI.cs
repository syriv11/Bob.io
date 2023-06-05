using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BobIO {
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private TMP_Text _nickname;
        [SerializeField] private TMP_Text _scores;
        [SerializeField] private Image _healthBarFill;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            //NetworkManager.Singleton.OnServerStarted += SetNickname;
        }

        private void UpdateNickname()
        {
            _nickname.SetText(_player.Nickname);
        }

        private void Update()
        {
            UpdateHealthBar();
            UpdateNickname();
            UpdateScores();
        }

        private void UpdateScores()
        {
            _scores.text = _player.Score.ToString();
        }

        private void UpdateHealthBar()
        {
            _healthBarFill.fillAmount = _player.CurrentHpNormalized;
        }
    }
}