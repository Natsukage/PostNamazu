using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PostNamazu.Models
{
    [JsonObject]
    public class WayMarks : IEnumerable<Waymark>
    {
        public string Name { get; set; }
        /// <summary> 实际为 ContentFinderCondition ID，为保持标点 JSON 格式兼容性而写作 MapID。</summary>
        public ushort MapID { get; set; }
        public Waymark A { get; set; }
        public Waymark B { get; set; }
        public Waymark C { get; set; }
        public Waymark D { get; set; }
        public Waymark One { get; set; }
        public Waymark Two { get; set; }
        public Waymark Three { get; set; }
        public Waymark Four { get; set; }
        public bool Log { get; set; } = true;
        public bool LocalOnly { get; set; } = true;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Waymark> GetEnumerator()
        {
            yield return A;
            yield return B;
            yield return C;
            yield return D;
            yield return One;
            yield return Two;
            yield return Three;
            yield return Four;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Name != null) sb.Append($"Name={Name}; \n");
            if (MapID != 0) sb.Append($"MapId={MapID}; \n");

            foreach (var waymark in this)
            {
                if (waymark != null)
                {
                    sb.Append(waymark.ToString());
                    sb.Append("; \n");
                }
            }
            if (sb.Length > 3)
            {
                sb.Remove(sb.Length - 3, 3);
            }
            return sb.ToString();
        }

        public string ToJsonString() => @$"{{
    ""A"": {{{A?.ToJsonString() ?? ""}}},
    ""B"": {{{B?.ToJsonString() ?? ""}}},
    ""C"": {{{C?.ToJsonString() ?? ""}}},
    ""D"": {{{D?.ToJsonString() ?? ""}}},
    ""One"":   {{{One  ?.ToJsonString() ?? ""}}},
    ""Two"":   {{{Two  ?.ToJsonString() ?? ""}}},
    ""Three"": {{{Three?.ToJsonString() ?? ""}}},
    ""Four"":  {{{Four ?.ToJsonString() ?? ""}}},
}}";

        internal static void SetWaymarkIds(WayMarks wayMarks)
        {
            if (wayMarks.A != null) wayMarks.A.ID = WaymarkID.A;
            if (wayMarks.B != null) wayMarks.B.ID = WaymarkID.B;
            if (wayMarks.C != null) wayMarks.C.ID = WaymarkID.C;
            if (wayMarks.D != null) wayMarks.D.ID = WaymarkID.D;
            if (wayMarks.One != null) wayMarks.One.ID = WaymarkID.One;
            if (wayMarks.Two != null) wayMarks.Two.ID = WaymarkID.Two;
            if (wayMarks.Three != null) wayMarks.Three.ID = WaymarkID.Three;
            if (wayMarks.Four != null) wayMarks.Four.ID = WaymarkID.Four;
        }

    }
}
