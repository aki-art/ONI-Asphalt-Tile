using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asphalt
{
    public class FSlider : KMonoBehaviour, IEventSystemHandler, IDragHandler
    {
        public event System.Action OnChange;
        private Slider slider;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            slider = gameObject.GetComponent<Slider>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                OnChange?.Invoke();
            }
        }

        public void OnBeginDrag()
        {
            Log.Info("Start drag");
        }

        public float Value
        {
            get
            {
                return slider.value;
            }
            set
            {
                slider.value = value;
            }
        }
    }
}
