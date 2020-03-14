using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class NukeScreen : KMonoBehaviour
    {
        /* This is far from perfect, but it works stable now.
         * Will be updated in the future. */

        private static GameObject nukeDialog;
        private static Canvas canvas;

        private static Transform tileToggle;
        private static Transform bitumenSelector;

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

            public bool GetValue()
            {
                return toggle.isOn;
            }
            public SettingsToggle(Transform togglePanel, bool state)
            {

                toggle = togglePanel.transform.Find("Toggle").gameObject.GetComponent<Toggle>();

                toggle.isOn = state;
            }
        }



        public void Initialize()
        {
            // Cancel
            void Deactivate()
            {
                nukeDialog.SetActive(false);
                nukeDialog = null;
            }

            void Nuke()
            {
                Log.Info("Nuking");
                NukePatches.NukeAsphalt = true;
                NukePatches.ChangeAsphaltToSandstoneTiles();
                nukeDialog.SetActive(false);
                nukeDialog = null;
            }

            CustomAssets.NukeModDialogPrefab.gameObject.GetComponent<Canvas>().GetComponent<CanvasScaler>().scaleFactor = FindObjectOfType<KCanvasScaler>().GetCanvasScale() * 1.1f;

            nukeDialog = Instantiate(CustomAssets.NukeModDialogPrefab);
            canvas = nukeDialog.gameObject.GetComponent<Canvas>();

            tileToggle = canvas.transform.Find("ModSettingsPanel/TogglePanel");
            bitumenSelector = canvas.transform.Find("ModSettingsPanel/CycleSelectorPanel");

            Button cancelButton = canvas.transform.Find("ModSettingsPanel/CancelButton").GetComponent<Button>();
            cancelButton.onClick.AddListener(Deactivate);
            Button okButton = canvas.transform.Find("ModSettingsPanel/OKButton").GetComponent<Button>();
            okButton.onClick.AddListener(Nuke);

            SettingsToggle glassSculptureFabulousToggle = new SettingsToggle(tileToggle, true);

            var bitumenOptions = new List<KeyValuePair<string, string>>();
            bitumenOptions.Add(new KeyValuePair<string, string>("Remove All", "Removes all bitumen from floor and storages."));
            bitumenOptions.Add(new KeyValuePair<string, string>("Refund", "Removes  all bitumen from floor and storages, refund some oil for it (1:4)."));
            bitumenOptions.Add(new KeyValuePair<string, string>("Leave", "Does not touch bitumen."));

            SettingsCycle bitumenCycler = new SettingsCycle(bitumenSelector, bitumenOptions);

        }
    }
}

