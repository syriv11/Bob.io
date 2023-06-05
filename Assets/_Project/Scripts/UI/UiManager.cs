using UnityEngine;

namespace BobIO
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField, InspectorName("HUD")] private GameObject Hud;

        [SerializeField] private MainMenuUI _mainMenuUI;

        public void ShowMainMenu()
        {
            _mainMenuUI.gameObject.SetActive(true);
            Hud.SetActive(false);
        }
        public void ShowHud()
        {
            _mainMenuUI.gameObject.SetActive(false);
            Hud.SetActive(true);
        }

        public string GetPlayerNickname()
        {
            return _mainMenuUI.GetPlayerNickname();
        }
    }
}