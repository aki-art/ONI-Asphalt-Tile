using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class ModSettingsScreen : KScreen
    {
        private bool shown = false;
        public bool pause = true;
        public const float SCREEN_SORT_KEY = 300f;
        public System.Action onDeactivateCB;

        Toggle bitumenToggle;
        Toggle nukeToggle;
        [SerializeField]
        private GameObject cancelButton;
        [SerializeField]
        private GameObject confirmButton;
        [SerializeField]
        private GameObject githubButton;
        [SerializeField]
        private GameObject steamButton;
        [SerializeField]
        private GameObject versionLabel;
        [SerializeField]
        private GameObject whatsNewLabel;
        [SerializeField]
        private GameObject saveLocationToggle;
        [SerializeField]
        private GameObject authorNote;
        [SerializeField]
        private GameObject speedSlider;
        [SerializeField]
        private GameObject colorSelector;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            #region set object references

            // This would be handled by Unity normally via Unity magic
            string path = "ScrollView/Viewport/Content/Panel";
            cancelButton = gameObject.transform.Find("CancelButton").gameObject;
            confirmButton = gameObject.transform.Find("OKButton").gameObject;
            githubButton = gameObject.transform.Find("GithubButton").gameObject;
            steamButton = gameObject.transform.Find("SteamButton").gameObject;
            versionLabel = gameObject.transform.Find("VersionLabel").gameObject;
            authorNote = gameObject.transform.Find("AuthorNote").gameObject;
            saveLocationToggle = gameObject.transform.Find("SaveLocationToggle").gameObject;
            whatsNewLabel = gameObject.transform.Find("VersionLabel/WhatsNew").gameObject;
            bitumenToggle = gameObject.transform.Find(path + "/TileSettingsPanel/TogglePanel/Toggle").GetComponent<Toggle>();
            speedSlider = gameObject.transform.Find(path + "/TileSettingsPanel/SliderPanel/Slider").gameObject;
            nukeToggle = gameObject.transform.Find(path + "/NukeSettingsPanel/TogglePanel/Toggle").GetComponent<Toggle>();
            colorSelector = gameObject.transform.Find(path + "/AdvancedSettingsPanel/ColorPicker").gameObject;

            #endregion

            // set default values
            bitumenToggle.isOn = SettingsManager.Settings.DisableBitumenProduction;
            nukeToggle.isOn = SettingsManager.Settings.NukeAsphaltTiles;
            speedSlider.GetComponent<Slider>().value = SettingsManager.Settings.SpeedMultiplier;

            if (SettingsManager.Settings.UseLocalFolder)
            {
                authorNote.SetActive(true);
                authorNote.GetComponent<Text>().text = "Settings will be reset with game updates.";
                UIHelper.AddSimpleToolTip(authorNote,
                    "When the game updates, since 2020 February all user files will be wiped in \n" +
                    "the mod folder. It seems you turned off permissions for this mod to save out-\n" +
                    "side in the hidden configuration options, where it could not be reset. ");
            }
            else
            {
                if(!SettingsManager.IsThereAnOutsideConfig)
                {
                    saveLocationToggle.SetActive(true);
                    string safeLocationInfo = "If <b>enabled</b>, this mod will save settings to\n " +
                        SettingsManager.exteriorPath.Replace('\\', '/') + "/config.json\n" +
                        "This will leave user settings to persist after mod uninstall, but won't ever reset.\n\n" +
                        "If <b>disabled</b>, the settings will be saved at\n" +
                        SettingsManager.localPath.Replace('\\', '/') + "/config.json\n" +
                        "This will cause user settings to reset at <u>every</u> mod update (including this checkbox.)";

                    UIHelper.AddSimpleToolTip(saveLocationToggle, safeLocationInfo, true);
                }
            }

            versionLabel.GetComponent<Text>().text = "v" + typeof(ModSettingsScreen).Assembly.GetName().Version.ToString();

            string whatsNewInfo = "<size=13><b>Version 1.1.0.0 Update</b>\n\n- " +
                "Auto Sweepers and Sweepy can now pick up Bitumen.\n " +
                "- Hatches can eat Bitumen (Base Hatch food, nothing fancy, it's just something to do\n " +
                "  with the excess bitumen)\n " +
                "- Bitumen can now be used to build Tempshift Plates and Wallpapers (dark slate grey)\n " +
                "- New Mod settings UI\n " +
                "- New option: Nuke mod.Replace asphalt tiles with regular (sandstone) tiles on the next\n " +
                "  World load. Useful if you plan to uninstall the mod permanently.\n " +
                "   <color=#9D9D9D>- This option will only apply once and then disable itself. If you\n " +
                "     don't use this, the missing asphalt will just leave regular 200kg deposits of solid\n " +
                "     vanilla bitumen behind, which isn't the end of the world, but as vanilla bitumen\n " +
                "     cannot be stored it may cause unwanted unsweepable clutter.\n " +
                "   - Option to remove all bitumen from world, which can greatly reduce useless clutter\n " +
                "     once the mod is gone.\n " +
                "       - Option to refund some Oil if you feel you lost out on resources.\n " +
                "   - If you just want to reinstall the mod, do not use this option.</color>\n " +
                "- Changing the mod settings no longer requires restart\n " +
                "- <color=#9D9D9D>Updated framework to 4.0\n " +
                "- No longer using Plib\n " +
                "- Cleaned up logging\n " +
                "- Small performance improvement</color></size>";

            UIHelper.AddSimpleToolTip(whatsNewLabel.gameObject, whatsNewInfo, true);

            ConsumeMouseScroll = true;
            activateOnSpawn = true;
            gameObject.SetActive(true);
        }

        public void ShowDialog()
        {
            Log.Debuglog("Showing Dialog");
            if (transform.parent.GetComponent<Canvas>() == null && transform.parent.parent != null)
            {
                transform.SetParent(transform.parent.parent);
            }
            transform.SetAsLastSibling();

            var cancelFButton = cancelButton.AddComponent<FButton>();
            cancelFButton.OnClick += OnClickCancel;
            UIHelper.AddSimpleToolTip(cancelButton, "Test tooltip\nmore tests ");

            var confirmFButton = confirmButton.AddComponent<FButton>();
            confirmFButton.OnClick += OnClickApply;

            var githubFButton = githubButton.AddComponent<FButton>();
            githubFButton.OnClick += OnClickGithub;

            var steamFButton = steamButton.AddComponent<FButton>();
            steamFButton.OnClick += OnClickSteam;

            List<FSpeedSlider.Range> ranges = new List<FSpeedSlider.Range> {
                new FSpeedSlider.Range(1f, 1f, "No bonus", Color.grey),
                new FSpeedSlider.Range(1f, 1.25f, "Small bonus", Color.grey),
                new FSpeedSlider.Range(1.25f, 1.25f, "Regular Tiles", Color.grey),
                new FSpeedSlider.Range(1.25f, 1.5f, "Some bonus", Color.white),
                new FSpeedSlider.Range(1.5f, 1.5f, "Metal tiles", Color.white),
                new FSpeedSlider.Range(1f, 2f, "Fast", Color.white),
                new FSpeedSlider.Range(2f, 2f, "Default", Color.white),
                new FSpeedSlider.Range(2f, 3f, "GO FAST", new Color32(44, 44, 242, 255)),
                new FSpeedSlider.Range(3f, 50f, "Light Speed", new Color32(183, 226, 13, 255)),
                new FSpeedSlider.Range(50f, 1000f, "Ridiculous", new Color32(226, 93, 13, 255)),
                new FSpeedSlider.Range(1000f, 1000f, "Ludicrous", new Color32(226, 23, 13, 255))
            };

            var speedFSpeedSlider = speedSlider.AddComponent<FSpeedSlider>();
            speedFSpeedSlider.ranges = ranges;

            colorSelector.AddComponent<FHSVColorSelector>();
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
            onDeactivateCB?.Invoke();
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

        public void OnClickApply()
        {
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

