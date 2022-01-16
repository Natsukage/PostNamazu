using System;

namespace PostNamazu.Models
{
    public class Mark { 
        public string Name { get; set; }
        public int ActorID { get; set; }
        public MarkingType MarkingType { get; set; }
        public bool LocalOnly =true;
    }

    public enum MarkingType : byte
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
