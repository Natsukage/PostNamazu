using System.Collections.Generic;

namespace PostNamazu.Models
{
    class WayMarks
    {
        public string Name { get; set; }
        public ushort MapID { get; set; }
        public Waymark A { get; set; }
        public Waymark B { get; set; }
        public Waymark C { get; set; }
        public Waymark D { get; set; }
        public Waymark One { get; set; }
        public Waymark Two { get; set; }
        public Waymark Three { get; set; }
        public Waymark Four { get; set; }

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
    }
}
