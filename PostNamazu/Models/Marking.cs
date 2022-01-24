#nullable enable 
using Newtonsoft.Json;

namespace PostNamazu.Models
{
    public class Marking {
        [JsonProperty]
        public string? Name { get; set; }
        [JsonProperty]
        public uint? ActorID { get; set; }
        [JsonProperty]
        public MarkType? MarkType { get; set; }
        [JsonProperty]
        public bool LocalOnly { get; set; } = false;
    }

    public enum MarkType : byte
    {
        attack1 = 1,
        attack2,
        attack3,
        attack4,
        attack5,
        bind1,
        bind2,
        bind3,
        stop1,
        stop2,
        square,
        circle,
        cross,
        triangle,
    }
}
