using UnityEngine.EventSystems;

namespace Asphalt
{
    // expects to be attached to a Button gameObject
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
