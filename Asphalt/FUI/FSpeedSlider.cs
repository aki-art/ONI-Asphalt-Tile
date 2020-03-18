using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asphalt
{
    public class FSpeedSlider : KMonoBehaviour, IEventSystemHandler, IDragHandler
    {
        public event System.Action OnChange;
        private Text speedMultiplerLabel;
        private const string SPEED_MULTIPLIER_PREFIX = "Speed bonus: ";
        private Text speedRangeLabel;
        public List<Range> ranges;
        private Slider slider;
        private bool maxed = false;
        Sprite plaidSprite;
        Color weirdPink;

        // Thanks for Asquaredπ and Peter Han for help with the math
        private float MapValue()
        {
            float actualValue;
            float val = slider.value;
            if (val < .66f) // on the first 2 thirds of the slider, we get a linear scale from 1-2.95
                actualValue = 3f * val + 1;
            else if (val == 0.66f) // it just skips between 2.95 and 3.05 (rounded) without hardcoding this one value
                actualValue = 3f;
            else
                actualValue = (float)Math.Pow(Math.E, 20.718 * (val - 2f / 3f)) + 2; // on the last half, we get an exponential range from 3.05 - 1000.285 (rounded)

            if (actualValue > 1000f) actualValue = 1000f; // clamping max value
            actualValue = (float)Math.Round(actualValue * 20) / 20; // rounding to .05

            return actualValue;
        }
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            slider = gameObject.GetComponent<Slider>();
            speedMultiplerLabel = transform.Find("SliderLabel").GetComponent<Text>();
            speedRangeLabel = transform.Find("SliderRangeLabel").GetComponent<Text>();
            Image image = transform.Find("Fill Area/Fill").GetComponent<Image>();
            plaidSprite = image.sprite;
            weirdPink = image.color;
            SetPlaid(false);

        }

        // this is a little lazy but it works™️
        public void OnDrag(PointerEventData eventData)
        {
            if (KInputManager.isFocused)
            {
                KInputManager.SetUserActive();
                UpdateLabels();
                OnChange?.Invoke();
            }
        }

        public void AssignRanges(List<Range> rangeList)
        {
            ranges = rangeList;
            UpdateLabels();
        }

        public void UpdateLabels()
        {
            float val = MapValue();
            speedMultiplerLabel.text = SPEED_MULTIPLIER_PREFIX + val.ToString() + "x";

            var currentRange = GetCurrentRange(val);
            speedRangeLabel.text = currentRange.name;
            speedRangeLabel.color = currentRange.color;

            // sets a plaid texture if the slider is maxed
            if (slider.value == slider.maxValue && !maxed)
                SetPlaid(true);
            else if (slider.value != slider.maxValue && maxed)
                SetPlaid(false);

        }

        private void SetPlaid(bool plaid)
        {
            maxed = plaid;
            Image image = transform.Find("Fill Area/Fill").GetComponent<Image>();

            if (plaid)
            {
                image.color = Color.white;
                image.sprite = plaidSprite;
            }
            else
            {
                image.color = weirdPink;
                image.sprite = null;
            }
        }

        private Range GetCurrentRange(float val)
        {
            if (ranges.Count > 0 && slider != null)
            {
                foreach (Range range in ranges)
                {
                    if (val >= range.min && val <= range.max)
                    {
                        return range;
                    }
                }
            }
            return new Range(-1, -1, "n/a", Color.white);
        }
        public struct Range
        {
            public float min;
            public float max;
            public string name;
            public Color color;


            public Range(float minimum, float maximum, string rangeName, Color rangeColor)
            {
                min = minimum;
                max = maximum;
                name = rangeName;
                color = rangeColor;
            }
        }

    }

}
