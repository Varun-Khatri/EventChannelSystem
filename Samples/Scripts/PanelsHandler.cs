using UnityEngine;

namespace VK.Events.Samples
{
    public class PanelsHandler : MonoBehaviour
    {
        [SerializeField] private EventChannelManager _eventManager;
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _loggedInPanel;
        [SerializeField] private GameObject _popupPanel;

        private void OnEnable()
        {
            _eventManager.Subscribe<string>(Events.ON_LOGGED_IN, OnLogin);
            _eventManager.Subscribe(Events.ON_LOGGED_OUT, OnLogout);
            _eventManager.Subscribe(Events.ON_SHOW_POPUP, ShowPopup);
            _eventManager.Subscribe(Events.ON_CLOSE_POPUP, HidePopup);
        }

        private void OnDisable()
        {
            _eventManager.Unsubscribe<string>(Events.ON_LOGGED_IN, OnLogin);
            _eventManager.Unsubscribe(Events.ON_LOGGED_OUT, OnLogout);
            _eventManager.Unsubscribe(Events.ON_SHOW_POPUP, ShowPopup);
            _eventManager.Unsubscribe(Events.ON_CLOSE_POPUP, HidePopup);
        }

        private void Start()
        {
            _loginPanel.SetActive(true);
            _popupPanel.SetActive(false);
            _loggedInPanel.SetActive(false);
        }

        private void OnLogin(string _)
        {
            _loggedInPanel.SetActive(true);
            _loginPanel.SetActive(false);
        }

        private void OnLogout()
        {
            _loggedInPanel.SetActive(false);
            _loginPanel.SetActive(true);
        }

        private void ShowPopup()
        {
            _popupPanel.SetActive(true);
        }

        private void HidePopup()
        {
            _popupPanel.SetActive(false);
        }

    }
}
