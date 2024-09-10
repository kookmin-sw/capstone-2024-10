using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
    enum Texts
    {
        ItemName,
    }

    public enum GameObjects
    {
        Item0,
        Item1,
        Item2,
        Item3
    }
    public List<GameObject> ItemList { get; protected set; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        ItemList = new List<GameObject>(Define.MAX_ITEM_NUM);

        ItemList.Add(GetObject((int)GameObjects.Item0));
        ItemList.Add(GetObject((int)GameObjects.Item1));
        ItemList.Add(GetObject((int)GameObjects.Item2));
        ItemList.Add(GetObject((int)GameObjects.Item3));

        ItemList[0].SetActive(false);
        ItemList[1].SetActive(false);
        ItemList[2].SetActive(false);
        ItemList[3].SetActive(false);

        Highlight(0);

        RemoveItemName();

        return true;
    }

    public void Show(int idx, int itemId)
    {
        RawImage img = ItemList[idx].GetComponent<RawImage>();
        img.texture = Managers.ResourceMng.Load<Texture>($"Images/{itemId}");
        ItemList[idx].SetActive(true);
    }
    public void Hide(int idx)
    {
        ItemList[idx].SetActive(false);
    }
    public void Highlight(int idx)
    {
        switch (idx)
        {
            case 0:
                GetImage(Images.InventoryBox0).transform.DOScale(new Vector3(1.2f,1.2f), 0.5f);
                GetImage(Images.InventoryBox1).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox2).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox3).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                break;
            case 1:
                GetImage(Images.InventoryBox0).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox1).transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
                GetImage(Images.InventoryBox2).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox3).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                break;
            case 2:
                GetImage(Images.InventoryBox0).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox1).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox2).transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
                GetImage(Images.InventoryBox3).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                break;
            case 3:
                GetImage(Images.InventoryBox0).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox1).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox2).transform.DOScale(new Vector3(1f, 1f), 0.5f);
                GetImage(Images.InventoryBox3).transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
                break;
        }

    }

    public void ShowItemName(int itemId)
    {
        if (itemId == -1)
        {
            GetText(Texts.ItemName).text = "";
            return;
        }

        foreach (var itemData in Managers.DataMng.ItemDataDict)
        {
            if (itemData.Value.DataId == itemId)
            {
                GetText(Texts.ItemName).text = itemData.Value.Name;
            }
        }
    }

    public void RemoveItemName()
    {
        GetText(Texts.ItemName).text = "";
    }
}
