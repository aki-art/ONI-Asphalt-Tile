using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Asphalt
{
    [Serializable]
    public class UserSettings : MonoBehaviour
    {
        [JsonProperty]
        public bool DisableBitumenProduction { get; set; } = false;
        [JsonProperty]
        public float SpeedMultiplier { get; set; } = 2.0f;
        [JsonProperty]
        public bool NukeAsphaltTiles { get; set; } = false;
        [JsonProperty]
        public bool UseLocalFolder { get; set; } = false; // not in UI
    }
}
