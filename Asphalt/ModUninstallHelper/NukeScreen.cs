using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asphalt
{
    class NukeScreen : KScreen
    {
        private bool shown = false;
        public bool pause = true;
        public const float SCREEN_SORT_KEY = 100f;
        public System.Action onDeactivateCB;

        [SerializeField]
        private Toggle tileToggle;
        [SerializeField]
        private GameObject cancelButton;
        [SerializeField]
        private GameObject confirmButton;
        [SerializeField]
        private GameObject bitumenCycleSelector;
        private FCycle bitumenFCycler;

        protected override void OnPrefabInit()
        {
            #region setting object references

            // This would be handled by Unity normally via magic
            cancelButton = gameObject.transform.Find("CancelButton").gameObject;
            confirmButton = gameObject.transform.Find("OKButton").gameObject;
            tileToggle = gameObject.transform.Find("TogglePanel/Toggle").GetComponent<Toggle>();
            bitumenCycleSelector = gameObject.transform.Find("CycleSelectorPanel").gameObject;

            #endregion

            base.OnPrefabInit();
            ConsumeMouseScroll = true;
            activateOnSpawn = true;
            gameObject.SetActive(true);
        }
        public void ShowDialog()
        {
            while (transform.parent.GetComponent<Canvas>() == null && transform.parent.parent != null)
            {
                transform.SetParent(transform.parent.parent);
            }
            transform.SetAsLastSibling();

            var cancelFButton = cancelButton.AddComponent<FButton>();
            cancelFButton.OnClick += OnClickCancel;

            var confirmFButton = confirmButton.AddComponent<FButton>();
            confirmFButton.OnClick += OnClickApply;

            var bitumenOptions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Remove All", "Removes all bitumen from floor and storages."),
                new KeyValuePair<string, string>("Refund", "Removes  all bitumen from floor and storages, refund some oil for it (1:4)."),
                new KeyValuePair<string, string>("Leave", "Does not touch bitumen.")
            };

            bitumenFCycler = bitumenCycleSelector.AddComponent<FCycle>();
            bitumenFCycler.Options = bitumenOptions;
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
            if (tileToggle.isOn)
                Nuker.ChangeAllAsphaltToSandstoneTiles();

            switch (bitumenFCycler.GetValue())
            {
                case "Remove All":
                    Nuker.RemoveAllBitumenFromWorld(false);
                    break;
                case "Refund":
                    Nuker.RemoveAllBitumenFromWorld(true);
                    break;
                case "Leave":
                default:
                    break;
            }
            Deactivate();
        }

        public void OnClickCancel()
        {
            Deactivate();
        }
    }
}

