using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    enum Images
    {
        InventoryBox0,
        InventoryBox1,
        InventoryBox2,
        InventoryBox3,
        
    }

    public enum GameObjects
    {
        Item0,
        Item1,
        Item2,
        Item3
    }
    public Crew Crew { get; set; }
    public List<GameObject> ItemList { get; protected set; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Crew.Inventory.UI_Inventory = this;
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        ItemList = new List<GameObject>(Define.MAX_ITEM_NUM);

        ItemList.Add(GetObject((int)GameObjects.Item0));
        ItemList.Add(GetObject((int)GameObjects.Item1));
        ItemList.Add(GetObject((int)GameObjects.Item2));
        ItemList.Add(GetObject((int)GameObjects.Item3));

        ItemList[0].SetActive(false);
        ItemList[1].SetActive(false);
        ItemList[2].SetActive(false);
        ItemList[3].SetActive(false);

        return true;
    }

    public void Show(int idx)
    {
        ItemList[idx].SetActive(true);
    }
    public void Hide(int idx)
    {
        ItemList[idx].SetActive(false);
    }
}
