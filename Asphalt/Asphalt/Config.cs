using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asphalt
{
    [Serializable]
    [RestartRequired]
    class Config : POptions.SingletonOptions<Config>
    {
        [Option("Speed multiplier", "Regular tiles: 1.25, Metal tiles: 1.5")]
        [Limit(0, 20)]
        [JsonProperty]
        public float speedMultiplier { get; set; } = 2.0f;
    }
}
