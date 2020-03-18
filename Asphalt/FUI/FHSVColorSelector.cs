
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class FHSVColorSelector : KMonoBehaviour
    {
        //Color.HSVToRGB();
        public FSlider hueSlider;
        public FSlider satSlider;
        public FSlider valSlider;

        private Image satColorImage;
        private Image valImage;

        private Image previewImage;

        private float h;
        private float s;
        private float v;

        public Color color;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            hueSlider = transform.Find("HueSlider").gameObject.AddComponent<FSlider>();
            satSlider = transform.Find("SaturationSlider").gameObject.AddComponent<FSlider>();
            valSlider = transform.Find("ValueSlider").gameObject.AddComponent<FSlider>();

            satColorImage = satSlider.transform.Find("Background").GetComponent<Image>();
            valImage = valSlider.transform.Find("Background").GetComponent<Image>();
            previewImage = transform.Find("Image").GetComponent<Image>();

            hueSlider.OnChange += UpdateColor;
            satSlider.OnChange += UpdateColor;
            valSlider.OnChange += UpdateColor;
        }
        public void UpdateColor()
        {
            h = hueSlider.Value;
            s = satSlider.Value;
            v = valSlider.Value;

            color = Color.HSVToRGB(h, s, v);
            UpdateSliders();
        }

        private void UpdateSliders()
        {
            satColorImage.color = color;
            valImage.color = color;
            previewImage.color = color;
        }


    }
}
