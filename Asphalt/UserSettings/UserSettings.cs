using Newtonsoft.Json;
using System;

namespace Asphalt
{
    [Serializable]
    public class UserSettings
    {
        [JsonProperty]
        public bool DisableBitumenProduction { get; set; } = false;
        [JsonProperty]
        public float SpeedMultiplier { get; set; } = 2.0f;
        [JsonProperty]
        public bool UseLocalFolder { get; set; } = false;
        [JsonProperty]
        public string BitumenColor { get; set; } = "41414FFF";
    }
}
