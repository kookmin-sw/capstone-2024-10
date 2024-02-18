using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string Name;
        public int Hp;
        public int Speed;

    }
    #endregion

    #region CrewData

    [Serializable]
    public class CrewData : CreatureData
    {
    }

    [Serializable]
    public class CrewDataLoader : IData<int, CrewData>
    {
        public List<CrewData> crews = new List<CrewData>();

        // List형태의 Data를 Dictionary형태로 변환 후 반환
        public Dictionary<int, CrewData> MakeDict()
        {
            Dictionary<int, CrewData> dic = new Dictionary<int, CrewData>();
            foreach (CrewData stat in crews)
                dic.Add(stat.DataId, stat);

            return dic;
        }
    }

    #endregion

    #region AlienData

    [Serializable]
    public class AlienData : CreatureData
    {
    }

    [Serializable]
    public class AlienDataLoader : IData<int, AlienData>
    {
        public List<AlienData> aliens = new List<AlienData>();

        public Dictionary<int, AlienData> MakeDict()
        {
            Dictionary<int, AlienData> dic = new Dictionary<int, AlienData>();
            foreach (AlienData stat in aliens)
                dic.Add(stat.DataId, stat);

            return dic;
        }
    }

    #endregion

    #region ItemData

    [Serializable]
    public class ItemData
    {
        public int DataId;
        public string Name;
    }

    [Serializable]
    public class ItemDataLoader : IData<int, ItemData>
    {
        public List<ItemData> items = new List<ItemData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            var dic = new Dictionary<int, ItemData>();
            foreach (ItemData item in items)
                dic.Add(item.DataId, item);

            return dic;
        }
    }
    #endregion
}
