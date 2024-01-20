using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PacketEnum
{
    public enum LoginMsg
    {
        Fail,
        Success,
    }

    public class Fish
    {
        public int dataId;
        public int level;
        public int color;
        public float size;
        public int objId;
    }
}
