using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace BobIO
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private UnityTransport _unityTransport;
        [SerializeField] private TMP_InputField _nicknameInput;
        [SerializeField] private TMP_InputField _addressInput;
        [SerializeField] private TMP_Text _errorText;
        [Space]
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private GameObject _errorTextRoot;

        private string LOCALHOST_ADDRESS = "127.0.0.1";
        private int _nicknameMaxLength = 12;

        private void Awake()
        {
            _errorTextRoot.SetActive(false);

            _hostButton.onClick.AddListener(() =>
            {
                if (!IsInputNicknameValid())
                {
                    _errorTextRoot.SetActive(true);
                    return;
                }

                if (IsInputAddressEmpty())
                {
                    UpdateAddress(LOCALHOST_ADDRESS);
                }
                else
                {
                    UpdateAddress(_addressInput.text);
                }
                
                NetworkManager.Singleton.StartHost();
                ShowHud();
            });

            _clientButton.onClick.AddListener(() =>
            {
                if (!IsInputNicknameValid())
                {
                    _errorTextRoot.SetActive(true);
                    return;
                }

                if (IsInputAddressEmpty())
                {
                    _errorTextRoot.SetActive(true);
                    return;
                }
                else
                {
                    UpdateAddress(_addressInput.text);
                }

                NetworkManager.Singleton.StartClient();
                ShowHud();
            });
        }

        private void UpdateAddress(string newAdress)
        {
            _unityTransport.ConnectionData.Address = newAdress;
        }

        private void ShowHud()
        {
            ProjectContext.Instance.UiManager.ShowHud();
        }

        public string GetPlayerNickname()
        {
            return _nicknameInput.text;
        }

        private bool IsInputNicknameValid()
        {
            if (_nicknameInput.text != string.Empty && 
                _nicknameInput.text.Length <= _nicknameMaxLength)
            {
                return true;
            }
            else
            {
                _errorText.text = $"Invalid nickname. \nMust be less than {_nicknameMaxLength} characters";
                return false;
            }
        }

        private bool IsInputAddressEmpty()
        {
            if (_addressInput.text == string.Empty)
            {
                _errorText.text = $"Invalid address. \nTo start client enter the IP address of host";
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}