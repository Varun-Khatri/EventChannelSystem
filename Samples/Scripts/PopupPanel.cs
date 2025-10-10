using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VK.Events.Samples
{
    public class PopupPanel : MonoBehaviour
    {
        [SerializeField] private EventChannelManager _eventManager;
        [SerializeField] private TMP_Text _message;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _eventManager.Subscribe<string>(Events.ON_SHOW_POPUP, ShowPopup);
            _eventManager.Subscribe(Events.ON_CLOSE_POPUP, ClosePopup);
            _closeButton.onClick.AddListener(() =>
            {
                _eventManager.Publish(Events.ON_CLOSE_POPUP);
            });
        }

        private void OnDestroy()
        {
            _eventManager.Unsubscribe<string>(Events.ON_SHOW_POPUP, ShowPopup);
            _eventManager.Unsubscribe(Events.ON_CLOSE_POPUP, ClosePopup);
            _closeButton.onClick.RemoveAllListeners();
        }

        private void ShowPopup(string message)
        {
            _message.text = message;
        }

        private void ClosePopup()
        {
            _message.text = string.Empty;
        }

    }
}
