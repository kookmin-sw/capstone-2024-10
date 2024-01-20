using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    /// <summary>
    /// 예시로 만든 Stat 데이터
    /// 해당 형식을 참고해 데이터 네임스페이스 안에서 region을 나눠 구현하면 된다.
    /// json 데이터 형식과 클래스의 변수 이름이 일치해야 한다는 것을 주의해야 한다.
    /// 데이터는 Resource/Data/ 폴더에 저장되어 있다.
    /// </summary>
    [Serializable]
    public class Stat
    {
        public int level;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);
            return dict;
        }
    }
    #endregion
    #region Fish
    /// <summary>
    /// 예시 데이터
    /// 해당 형식을 참고해 데이터 네임스페이스 안에서 region을 나눠 구현하면 된다.
    /// json 데이터 형식과 클래스의 변수 이름이 일치해야 한다는 것을 주의해야 한다.
    /// 데이터는 Resource/Data/ 폴더에 저장되어 있다.
    /// </summary>
    [Serializable]
    public class Fish
    {
        public int id;
        public string name;
        public int[] level;
        public int moveSpeed;
        public int[] size;
        public int health;
        public string[] lure;
        public string[] color;
        public string feature;
        public int Iscatching;
    }

    [Serializable]
    public class FishData : ILoader<int, Fish>
    {
        public List<Fish> fishes = new List<Fish>();

        public Dictionary<int, Fish> MakeDict()
        {
            Dictionary<int, Fish> dict = new Dictionary<int, Fish>();
            foreach (Fish fish in fishes)
                dict.Add(fish.id, fish);
            return dict;
        }
    }
    #endregion
}

