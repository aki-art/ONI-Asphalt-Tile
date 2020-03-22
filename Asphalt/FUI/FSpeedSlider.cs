using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asphalt
{
    public class FSpeedSlider : KMonoBehaviour, IEventSystemHandler
    {
        private const string SPEED_MULTIPLIER_PREFIX = "Speed bonus: ";

        private Text speedMultiplerLabel;
        private Text speedRangeLabel;

        public List<Range> ranges;

        private FSlider fSlider;
        Sprite plaidSprite;
        Image backgroundImage;
        Color weirdPink;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            #region  set object references
            fSlider = gameObject.AddComponent<FSlider>();
            speedMultiplerLabel = transform.Find("SliderLabel").GetComponent<Text>();
            speedRangeLabel = transform.Find("SliderRangeLabel").GetComponent<Text>();
            backgroundImage = transform.Find("Fill Area/Fill").GetComponent<Image>();
            plaidSprite = backgroundImage.sprite;
            weirdPink = backgroundImage.color;
            #endregion

            fSlider.OnChange += UpdateLabels;
            backgroundImage.color = weirdPink;
            backgroundImage.sprite = null;
            UpdateLabels();
        }

        public void AssignRanges(List<Range> rangeList)
        {
            ranges = rangeList;
            UpdateLabels();
        }

        public void SetValue(float val)
        {
            fSlider.Value = val;
            UpdateLabels();
        }

        public void UpdateLabels()
        {
            float val = MapValue();
            speedMultiplerLabel.text = SPEED_MULTIPLIER_PREFIX + val.ToString() + "x";

            if (fSlider.slider.value < fSlider.slider.maxValue)
            {
                backgroundImage.color = weirdPink;
                backgroundImage.sprite = null;
            }
            else
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = plaidSprite;
            }

            UpdateRange(val);

        }

        private void UpdateRange(float val)
        {
            if (ranges != null && ranges.Count > 0)
            {
                Log.Info("Checking: " + val);
                var currentRange = ranges.FirstOrDefault(r => r.min <= val);
                if(currentRange.name != null)
                {
                    Log.Info(currentRange.name);
                    speedRangeLabel.text = currentRange.name;
                    speedRangeLabel.color = currentRange.color;
                }
            }
            else Log.Info("no ranges defined");
        }


        // Thanks for Asquaredπ and Peter Han for help with the math
        private float MapValue(float val)
        {
            float actualValue;
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

        private float MapValue()
        {
            float val = fSlider.Value;
            return MapValue(val);
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
