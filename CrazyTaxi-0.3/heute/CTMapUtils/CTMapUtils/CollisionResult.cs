using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTMapUtils
{
    public class CollisionResult
    {
        public bool Front { get; internal set; }
        public bool Left { get; internal set; }
        public bool Right { get; internal set; }
        public bool Back { get; internal set; }

        public CollisionResult()
        {
            Front = false;
            Left = false;
            Right = false;
            Back = false;
        }
    }
}
