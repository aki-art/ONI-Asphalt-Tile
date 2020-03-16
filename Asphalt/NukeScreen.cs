using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Asphalt
{
    // WIP, Broken atm
    // Huge issues with trying to pass values between elements of UI.
    class NukeScreen : KScreen
    {
        public const float SCREEN_SORT_KEY = 500f;
        /* This is far from perfect, but it works stable now.
         * Will be updated in the future. */

        private static GameObject nukeDialog;
        private static Canvas canvas;

        private static Transform tileToggle;
        private static Transform bitumenSelector;

        private static bool NukeAsphalt = false;
        private static string bitumenOption;

        private UnityAction<NukeScreen> onCancelClick;

        SettingsToggle tilesToggle;
        SettingsCycle bitumenCycler;
        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape))
                Deactivate();
            else
                base.OnKeyDown(e);
        }

        public NukeScreen()
        {
            Initialize();
        }

        public class SettingsCycle
        {
            private readonly GameObject btnNext;
            private readonly GameObject btnPrev;
            private int index = 0;
            private readonly GameObject selectedLabel;
            private readonly GameObject note;
            public List<KeyValuePair<string, string>> Options { get; set; }

            public string GetValue()
            {
                return selectedLabel.GetComponent<Text>().text;
            }
            public SettingsCycle(Transform cycle, List<KeyValuePair<string, string>> options)
            {
                void Next()
                {
                    index++;
                    index = index == -1 ? Options.Count - 1 : index % Options.Count;
                    SetSelected(Options[index]);
                }

                void Previous()
                {
                    index--;
                    index = index == -1 ? Options.Count - 1 : index % Options.Count;
                    SetSelected(Options[index]);
                }

                Options = options;

                btnNext = cycle.transform.Find("CycleSelector/LightShapeRightButton").gameObject;
                btnNext.GetComponent<Button>().onClick.AddListener(Next);

                btnPrev = cycle.transform.Find("CycleSelector/LightShapeLeftButton").gameObject;
                btnPrev.GetComponent<Button>().onClick.AddListener(Previous);

                selectedLabel = cycle.transform.Find("CycleSelector").gameObject;
                note = cycle.transform.Find("Note").gameObject;
                SetSelected(Options[0]);

            }

            private void SetSelected(KeyValuePair<string, string> selectedText)
            {
                selectedLabel.GetComponent<Text>().text = selectedText.Key;
                note.GetComponent<Text>().text = selectedText.Value;
            }

            public string GetSelected()
            {
                return Options[index].Key;
            }
        }

        public class SettingsToggle
        {
            private Toggle toggle;

            public bool IsOn()
            {
                return toggle.isOn;
            }
            public SettingsToggle(Transform togglePanel, bool state)
            {
                void OnValueChanged(bool arg0)
                {
                    NukeAsphalt = arg0;
                }

                toggle = togglePanel.transform.Find("Toggle").gameObject.GetComponent<Toggle>();

                toggle.isOn = state;
                toggle.onValueChanged.AddListener(OnValueChanged);
            }

        }

        public void Initialize()
        {
            KScreenManager.Instance.DisableInput(true);
            void onCancelClicked()
            {
                nukeDialog.gameObject.SetActive(false);
                Nuker.ChangeAllAsphaltToSandstoneTiles();
            }

            // Scale this UI along the regular UI
            ModAssets.Prefabs.nukeScreenPrefab.gameObject.GetComponent<Canvas>().GetComponent<CanvasScaler>().scaleFactor = FindObjectOfType<KCanvasScaler>().GetCanvasScale() * 1.1f;

            nukeDialog = Instantiate(ModAssets.Prefabs.nukeScreenPrefab);
            canvas = nukeDialog.gameObject.GetComponent<Canvas>();

            tileToggle = canvas.transform.Find("ModSettingsPanel/TogglePanel");
            bitumenSelector = canvas.transform.Find("ModSettingsPanel/CycleSelectorPanel");

            tilesToggle = new SettingsToggle(tileToggle, true);

            var bitumenOptions = new List<KeyValuePair<string, string>>();
            bitumenOptions.Add(new KeyValuePair<string, string>("Remove All", "Removes all bitumen from floor and storages."));
            bitumenOptions.Add(new KeyValuePair<string, string>("Refund", "Removes  all bitumen from floor and storages, refund some oil for it (1:4)."));
            bitumenOptions.Add(new KeyValuePair<string, string>("Leave", "Does not touch bitumen."));

            bitumenCycler = new SettingsCycle(bitumenSelector, bitumenOptions);

         
            Button cancelButton = canvas.transform.Find("ModSettingsPanel/CancelButton").GetComponent<Button>();
            //cancelButton.onClick.AddListener(null);
            Button okButton = canvas.transform.Find("ModSettingsPanel/OKButton").GetComponent<Button>();
            okButton.onClick.AddListener(onCancelClicked);


        }


    }
}

