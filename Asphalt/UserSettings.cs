using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using System;

namespace Asphalt
{
    [Serializable]
    [ModInfo("Asphalt Tile", "https://github.com/aki-art/ONI-Asphalt-Tile", "hatch.png")]
    public class UserSettings : POptions.SingletonOptions<UserSettings>
    {

        [Option("Bitumen production", "Enable/Disable the production of bitumen.", "Tile Settings")]
        [JsonProperty]
        public bool BitumenProduction { get; set; } = true;
        [Option("Speed bonus", "Speed multiplier.", "Tile Settings")]
        [Limit(0, 20)]
        [JsonProperty]
        public float SpeedMultiplier { get; set; } = 2.0f;
        [Option("Nuke Asphalt Tiles", "On next world load, replace all asphalt tiles with sandstone tiles.", "Nuke mod")]
        [JsonProperty]
        public bool NukeAsphaltTiles { get; set; } = true;
    }
}
