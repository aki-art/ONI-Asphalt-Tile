using UnityEngine.EventSystems;

namespace Asphalt
{
    public class FButton : KMonoBehaviour, IEventSystemHandler, IPointerDownHandler
    {
        public event System.Action OnClick;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                OnClick?.Invoke();
            }
        }
    }
}
