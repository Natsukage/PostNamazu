using Newtonsoft.Json;

namespace PostNamazu.Models
{
    public class UseAction
    {
        public class Action
        {
            [JsonProperty] public int? ActionType { get; set; }

            [JsonProperty] public uint? ActionID { get; set; }

            [JsonProperty] public long? TargetID { get; set; }
            

            [JsonProperty] public bool IsGroundTarget { get; set; }
            
            [JsonProperty] public float? X { get; set; }
            
            [JsonProperty] public float? Y { get; set; }
            
            [JsonProperty] public float? Z { get; set; }
        }
    }
}