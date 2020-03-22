﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class FHSVColorSelector : KMonoBehaviour
    {
        // TODO:
        // input fields should prompt change too

        public event System.Action OnChange;

        public FSlider hueSlider;
        public FSlider satSlider;
        public FSlider valSlider;

        private FNumberInputField hueField;
        private FNumberInputField satField;
        private FNumberInputField valField;

        private const float HUE_MAX = byte.MaxValue;
        private const float SAT_MAX = 100;
        private const float VAL_MAX = 100;

        private Image satColorImage;
        private Image valImage;

        private Image previewImage;

        private float h;
        private float s;    
        private float v;

        public Color color;
        private Color defaultColor;
        private Color loadedColor;

        private FButton resetButton;
        private FButton resetTempButton;
        private Image resetImg;
        private Image resetTempImg;

        static float MapHue(float x)
        {
            return x / HUE_MAX;
        }
        static float MapSat(float x)
        {
            return x / SAT_MAX;
        }
        static float MapVal(float x)
        {
            return x / VAL_MAX;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            #region setting object references
            hueSlider = transform.Find("HueSlider").gameObject.AddComponent<FSlider>();
            satSlider = transform.Find("SaturationSlider").gameObject.AddComponent<FSlider>();
            valSlider = transform.Find("ValueSlider").gameObject.AddComponent<FSlider>();

            hueField = hueSlider.transform.Find("InputField").gameObject.AddComponent<FNumberInputField>();
            satField = satSlider.transform.Find("InputField").gameObject.AddComponent<FNumberInputField>();
            valField = valSlider.transform.Find("InputField").gameObject.AddComponent<FNumberInputField>();

            satColorImage = satSlider.transform.Find("Background").GetComponent<Image>();
            valImage = valSlider.transform.Find("Background").GetComponent<Image>();
            previewImage = transform.Find("Image").GetComponent<Image>();

            resetImg = transform.Find("ResetOriginal").GetComponent<Image>();
            resetButton = resetImg.gameObject.AddComponent<FButton>();

            resetTempImg = transform.Find("ResetLast").GetComponent<Image>();
            resetTempButton = resetTempImg.gameObject.AddComponent<FButton>();
            #endregion

            resetButton.OnClick += ResetColor;
            resetTempButton.OnClick += ResetTempColor;

            hueSlider.OnChange += TriggerOnChange;
            satSlider.OnChange += TriggerOnChange;
            valSlider.OnChange += TriggerOnChange;

            hueSlider.AttachInputField(hueField, MapHue);
            satSlider.AttachInputField(satField, MapVal);
            valSlider.AttachInputField(valField, MapSat);

            hueField.maxValue = (int)HUE_MAX;
            satField.maxValue = (int)SAT_MAX;
            valField.maxValue = (int)VAL_MAX;

            SetColorFromHex(SettingsManager.Settings.BitumenColor); // set default values

            defaultColor = Util.ColorFromHex(SettingsManager.DefaultSettings.BitumenColor);
            loadedColor = Util.ColorFromHex(SettingsManager.LoadedSettings.BitumenColor);

            resetImg.gameObject.SetActive(IsChanged(defaultColor));

            resetImg.color = defaultColor;
            resetTempImg.color = loadedColor;
        }
        public void SetColor(Color RGBColor)
        {
            Color.RGBToHSV(RGBColor, out h, out s, out v);
            color = RGBColor;

            UpdateColorPreviews();
            UpdateInputFields();
            UpdateSliderValues();
        }
        public void SetColorFromHex(string hex)
        {
            Log.Info("trying to set color: " + hex);
            color = Util.ColorFromHex(hex);
            Color.RGBToHSV(color, out h, out s, out v);
            Log.Info("set color:  " + Util.ToHexString(color));

            UpdateColorPreviews();
            UpdateInputFields();
            UpdateSliderValues();
        }
        public string GetHexValue()
        {
            return Util.ToHexString(color);
        }

        public void UpdateColor()
        {
            h = hueSlider.Value;
            s = satSlider.Value;
            v = valSlider.Value;

            color = Color.HSVToRGB(h, s, v);

            UpdateColorPreviews();
            UpdateInputFields();
        }
        private void UpdateInputFields()
        {
            float m = 100f;
            hueField.SetDisplayValue(Mathf.Ceil(h * byte.MaxValue).ToString());
            satField.SetDisplayValue(Mathf.Ceil(s * m).ToString());
            valField.SetDisplayValue(Mathf.Ceil(v * m).ToString());
        }

        private void UpdateColorPreviews()
        {
            satColorImage.color = color;
            valImage.color = color;
            previewImage.color = color;
        }

        private void UpdateSliderValues()
        {
            hueSlider.Value = h;
            satSlider.Value = s;
            valSlider.Value = v;
        }

        private void ResetTempColor()
        {
            SetColorFromHex(SettingsManager.LoadedSettings.BitumenColor);
            TriggerOnChange();
        }

        private void ResetColor()
        {
            SetColorFromHex(SettingsManager.DefaultSettings.BitumenColor);
            TriggerOnChange();
        }
        public bool IsChanged(Color defaultValue)
        {
            return !CompareColors(color, defaultValue, 1);
        }

        private bool CompareColors(Color c1, Color c2, byte treshold)
        {
            var c1r = c1.r * 1000;
            var c1g = c1.g * 1000;
            var c1b = c1.b * 1000;
            var c2r = c2.r * 1000;
            var c2g = c2.g * 1000;
            var c2b = c2.b * 1000;

            float rDiff = Mathf.Abs(c1r - c2r);
            float gDiff = Mathf.Abs(c1g - c2g);
            float bDiff = Mathf.Abs(c1b - c2b);

            return (rDiff + gDiff + bDiff) <= treshold;
        }

        private void TriggerOnChange()
        {
            resetImg.gameObject.SetActive(IsChanged(defaultColor));
            resetTempImg.gameObject.SetActive(IsChanged(loadedColor));

            UpdateColor();

            OnChange?.Invoke();
        }
    }
}
