﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class ModSettingsScreen : KScreen
    {
        private bool shown = false;
        public bool pause = true;
        public const float SCREEN_SORT_KEY = 300f;

        private Toggle bitumenToggle;
        private Toggle nukeToggle;
        private Toggle safeLocationToggle;

        private FButton cancelButton;
        private FButton confirmButton;
        private FButton githubButton;
        private FButton steamButton;

        private Text versionLabel;
        private Text whatsNewLabel;
        private Text authorNote;

        private FSpeedSlider speedSlider;

        private FHSVColorSelector colorSelector;

        private FAccordion accordionControl;
        private GameObject advancedSettingsAccordion;

        private List<FSpeedSlider.Range> speedSliderRanges;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            #region set object references

            // This would be handled by Unity normally via Unity magic
            string path = "ScrollView/Viewport/Content/Panel";

            cancelButton = gameObject.transform.Find("CancelButton").gameObject.AddComponent<FButton>();
            confirmButton = gameObject.transform.Find("OKButton").gameObject.AddComponent<FButton>();
            githubButton = gameObject.transform.Find("GithubButton").gameObject.AddComponent<FButton>();
            steamButton = gameObject.transform.Find("SteamButton").gameObject.AddComponent<FButton>();

            versionLabel = gameObject.transform.Find("VersionLabel").gameObject.GetComponent<Text>();
            authorNote = gameObject.transform.Find("AuthorNote").gameObject.GetComponent<Text>();

            whatsNewLabel = gameObject.transform.Find("VersionLabel/WhatsNew").gameObject.GetComponent<Text>();
            bitumenToggle = gameObject.transform.Find(path + "/TileSettingsPanel/TogglePanel/Toggle").GetComponent<Toggle>();
            speedSlider = gameObject.transform.Find(path + "/TileSettingsPanel/SliderPanel/Slider").gameObject.AddComponent<FSpeedSlider>();
            nukeToggle = gameObject.transform.Find(path + "/NukeSettingsPanel/TogglePanel/Toggle").GetComponent<Toggle>();

            colorSelector = gameObject.transform.Find(path + "/AdvancedSettingsAccordion/AdvancedSettingsPanel/ColorPicker").gameObject.AddComponent<FHSVColorSelector>();

            accordionControl = gameObject.transform.Find(path + "/AccordionController").gameObject.AddComponent<FAccordion>();
            advancedSettingsAccordion = gameObject.transform.Find(path + "/AdvancedSettingsAccordion").gameObject;
            safeLocationToggle = gameObject.transform.Find(path + "/AdvancedSettingsAccordion/AdvancedSettingsPanel/TogglePanel/Toggle").GetComponent<Toggle>();

            bitumenToggle.gameObject.AddComponent<FToggle>();
            nukeToggle.gameObject.AddComponent<FToggle>();
            safeLocationToggle.gameObject.AddComponent<FToggle>();

            #endregion

            if (SettingsManager.Settings.UseLocalFolder)
            {
                authorNote.gameObject.SetActive(true);
                authorNote.GetComponent<Text>().text = "Settings will be reset with game updates.";
                UIHelper.AddSimpleToolTip(authorNote.gameObject,
                    "When the game updates, since 2020 February all user files will be wiped in the mod folder. It seems you turned off permissions for this mod to save outside in the hidden configuration options, where it could not be reset. ", false, 400f);
            }

            versionLabel.GetComponent<Text>().text = "v" + typeof(ModSettingsScreen).Assembly.GetName().Version.ToString();

            // TODO: figure out something better for this
            string whatsNewInfo = "<size=13><b>Version 1.1.0.0 Update</b>\n\n- " +
                "Auto Sweepers and Sweepy can now pick up Bitumen.\n " +
                "- Hatches can eat Bitumen (Base Hatch food, nothing fancy, it's just something to do with the excess bitumen)\n " +
                "- Bitumen can now be used to build Tempshift Plates and Wallpapers (dark slate grey)\n " +
                "- New Mod settings UI\n " +
                "- New option: Nuke mod.Replace asphalt tiles with regular (sandstone) tiles on the next World load. Useful if you plan to uninstall the mod permanently.\n  <color=#9D9D9D>- This option will only apply once and then disable itself. If you don't use this, the missing asphalt will just leave regular 200kg deposits of solid vanilla bitumen behind, which isn't the end of the world, but as vanilla bitumen cannot be stored it may cause unwanted unsweepable clutter.\n " +
                "   - Option to remove all bitumen from world, which can greatly reduce useless clutter once the mod is gone.\n " +
                "       - Option to refund some Oil if you feel you lost out on resources.\n " +
                "   - If you just want to reinstall the mod, do not use this option.</color>\n " +
                "- Changing the mod settings no longer requires restart\n " +
                "- <color=#9D9D9D>Updated framework to 4.0\n " +
                "- No longer using Plib\n " +
                "- Cleaned up logging\n " +
                "- Small performance improvement</color></size>";

            UIHelper.AddSimpleToolTip(whatsNewLabel.gameObject, whatsNewInfo, true, 400f);

            speedSliderRanges = new List<FSpeedSlider.Range> {
                new FSpeedSlider.Range(1f, 1.05f, "No bonus", Color.grey),
                new FSpeedSlider.Range(1.05f, 1.2f, "Small bonus", Color.grey),
                new FSpeedSlider.Range(1.25f, 1.25f, "Regular Tiles", Color.white),
                new FSpeedSlider.Range(1.3f, 1.5f, "Some bonus", Color.white),
                new FSpeedSlider.Range(1.5f, 1.5f, "Metal tiles", Color.white),
                new FSpeedSlider.Range(1.55f, 1.95f, "Fast", Color.white),
                new FSpeedSlider.Range(2f, 2f, "Default", Color.white),
                new FSpeedSlider.Range(2.05f, 3f, "GO FAST", new Color32(55, 168, 255, 255)),
                new FSpeedSlider.Range(3f, 50f, "Light Speed", new Color32(183, 226, 13, 255)),
                new FSpeedSlider.Range(50f, 999.95f, "Ridiculous", new Color32(226, 93, 13, 255)),
                new FSpeedSlider.Range(999.95f, 1000f, "Ludicrous", new Color32(226, 23, 13, 255))
            };

            SetSettings(SettingsManager.Settings);

            ConsumeMouseScroll = true;
            activateOnSpawn = true;
            gameObject.SetActive(true);
        }


        public void ShowDialog()
        {
            if (transform.parent.GetComponent<Canvas>() == null && transform.parent.parent != null)
                transform.SetParent(transform.parent.parent);
            transform.SetAsLastSibling();

            // Buttons
            cancelButton.OnClick += OnClickCancel;
            confirmButton.OnClick += OnClickApply;
            githubButton.OnClick += OnClickGithub;
            steamButton.OnClick += OnClickSteam;

            UIHelper.AddSimpleToolTip(githubButton.gameObject, "Open Asphalt Tiles on Github");
            UIHelper.AddSimpleToolTip(steamButton.gameObject, "Open Asphalt Tiles on Steam");

            // Tile Settings
            speedSlider.SetValue(SettingsManager.Settings.SpeedMultiplier);
            speedSlider.AssignRanges(speedSliderRanges);

            // Nuke Settings
            nukeToggle.isOn = SettingsManager.TempSettings.NukeAsphaltTiles;

            // Advanced Settings
            accordionControl.SetTarget(advancedSettingsAccordion, 168f, false);
            var colorSelectorLabel = colorSelector.transform.Find("SliderLabel").gameObject;
            UIHelper.AddSimpleToolTip(colorSelectorLabel, "Sets element color for bitumen.\nThis will show up for wallpapers or painted walls. \n\nRequires restart.");
        }


        #region Handling Input, Screen and Camera
        protected override void OnCmpEnable()
        {
            base.OnCmpEnable();
            if (CameraController.Instance != null)
            {
                CameraController.Instance.DisableUserCameraControl = true;
            }
        }

        protected override void OnCmpDisable()
        {
            base.OnCmpDisable();
            if (CameraController.Instance != null)
            {
                CameraController.Instance.DisableUserCameraControl = false;
            }
            Trigger((int)GameHashes.Close, null);
        }

        public override bool IsModal()
        {
            return true;
        }

        public override float GetSortKey()
        {
            return SCREEN_SORT_KEY;
        }

        protected override void OnActivate()
        {
            OnShow(true);
        }

        protected override void OnDeactivate()
        {
            OnShow(false);
        }

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            if (pause && SpeedControlScreen.Instance != null)
            {
                if (show && !shown)
                {
                    SpeedControlScreen.Instance.Pause(false);
                }
                else
                {
                    if (!show && shown)
                    {
                        SpeedControlScreen.Instance.Unpause(false);
                    }
                }
                shown = show;
            }
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape))
            {
                OnClickCancel();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        public override void OnKeyUp(KButtonEvent e)
        {
            if (!e.Consumed)
            {
                KScrollRect scroll_rect = GetComponentInChildren<KScrollRect>();
                if (scroll_rect != null)
                {
                    scroll_rect.OnKeyUp(e);
                }
            }
            e.Consumed = true;
        }
        #endregion

        private void SetSettings(UserSettings values)
        {
            bitumenToggle.isOn = values.DisableBitumenProduction;
            speedSlider.SetValue(values.SpeedMultiplier);
            safeLocationToggle.isOn = values.UseLocalFolder;
            colorSelector.SetColorFromHex(values.BitumenColor);
        }

        public void OnClickApply()
        {
            float speedSlideValue = speedSlider.GetComponent<Slider>().value;

            // Immediate effects

            ElementLoader.FindElementByHash(SimHashes.Bitumen).substance.uiColour = colorSelector.color;
            ElementLoader.FindElementByHash(SimHashes.Bitumen).substance.colour = colorSelector.color;
            ElementLoader.FindElementByHash(SimHashes.Bitumen).substance.conduitColour = colorSelector.color;

            SettingsManager.TempSettings.UpdateMovementMultiplierRunTime = (SettingsManager.Settings.SpeedMultiplier != speedSlideValue);
            SettingsManager.TempSettings.NukeAsphaltTiles = nukeToggle.isOn;

            // Saving configuration
            SettingsManager.Settings.DisableBitumenProduction = bitumenToggle.isOn;
            SettingsManager.Settings.SpeedMultiplier = speedSlideValue;
            SettingsManager.Settings.UseLocalFolder = safeLocationToggle.isOn;
            SettingsManager.Settings.BitumenColor = colorSelector.GetHexValue();

            SettingsManager.SaveSettings();

            Deactivate();
        }

        public void OnClickCancel()
        {
            Deactivate();
        }
        public void OnClickGithub()
        {
            Application.OpenURL("https://github.com/aki-art/ONI-Asphalt-Tile");
        }
        public void OnClickSteam()
        {
            Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=1979475408");
        }
    }
}
