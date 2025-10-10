using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VK.Events.Samples
{
    public class LoginPanel : MonoBehaviour
    {
        [SerializeField] private EventChannelManager _eventManager;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _loginButton;

        private void Awake()
        {
            _eventManager.Subscribe<string>(Events.ON_LOGIN_CLICKED, Login);
            _loginButton.onClick.AddListener(() =>
            {
                _eventManager.Publish(Events.ON_LOGIN_CLICKED, _inputField.text);
            });
        }

        private void OnDestroy()
        {
            _eventManager.Unsubscribe<string>(Events.ON_LOGIN_CLICKED, Login);
            _loginButton.onClick.RemoveAllListeners();
        }

        private void Login(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                _eventManager.Publish(Events.ON_SHOW_POPUP, "Username cannot be empty");
                return;
            }
            _eventManager.Publish(Events.ON_LOGGED_IN, userName);
        }
    }
}
