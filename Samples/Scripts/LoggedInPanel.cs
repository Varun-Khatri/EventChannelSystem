using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VK.Events.Samples
{
    public class LoggedInPanel : MonoBehaviour
    {
        [SerializeField] private EventChannelManager _eventManager;
        [SerializeField] private TMP_Text _userName;
        [SerializeField] private Button _logoutButton;

        private void Awake()
        {
            _eventManager.Subscribe<string>(Events.ON_LOGGED_IN, OnLogin);
            _eventManager.Subscribe(Events.ON_LOGOUT_CLICKED, OnLogout);
            _logoutButton.onClick.AddListener(() => _eventManager.Publish(Events.ON_LOGOUT_CLICKED));
        }

        private void OnDestroy()
        {
            _eventManager.Unsubscribe<string>(Events.ON_LOGGED_IN, OnLogin);
            _eventManager.Unsubscribe(Events.ON_LOGOUT_CLICKED, OnLogout);
            _logoutButton.onClick.RemoveAllListeners();
        }

        private void OnLogin(string userName)
        {
            _userName.text = $"Welcome, {userName}!";
        }

        private void OnLogout()
        {
            _eventManager.Publish(Events.ON_LOGGED_OUT);
        }

    }
}