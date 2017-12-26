using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManager : MonoBehaviour {
    public Navigation nav;
    public GameObject item;
    public List<Structs.MoneyEntry> curList;
    public Transform itemParent;
    LevelOne lvlOne;
    public GameObject editPanel;
    public string curListName;
	// Use this for initialization
	void Awake () {
        lvlOne = GameObject.Find("Canvas").GetComponent<LevelOne>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void AddItemToList(string name , string value, bool newItem, int index=-1)
    {
        GameObject go =(GameObject)Instantiate(item);
        go.transform.SetParent( itemParent);
        GameObject but = go.transform.GetChild(0).gameObject;
        Button b = but.GetComponent<Button>();
        b.transform.Find("Name").GetComponent<Text>().text = name;
        b.transform.Find("Value").GetComponent<Text>().text = value;
        if (index != -1)
            go.name = index.ToString();
        if (gameObject.name == "ListPanel")
        {
            b.onClick.AddListener(() => nav.OpenEditPanel(go, newItem));
            b.onClick.AddListener(() => nav.GoTo(editPanel));
            if (newItem)
            {
                go.name = curList.Count.ToString();
                nav.OpenEditPanel(go, true);
            }
        }
        else {
            b.onClick.AddListener(() => nav.GoTo(editPanel));
            if (newItem)
            {
                go.name = curList.Count.ToString();
                nav.GoTo(editPanel);
            }
        }
    }



    public int CheckForDuplicateName( string n) {
        foreach (Structs.MoneyEntry me in curList) {
            if (me.name == n) {
                return curList.IndexOf(me);
            }
        }
        return -1;
    }

    public int CheckForDuplicateItem(string n, float v)
    {
        foreach (Structs.MoneyEntry me in curList)
        {
            if (me.name == n && me.value == v)
            {
                return curList.IndexOf(me);
            }
        }
        return -1;
    }

    public void AddNewItemToList() {
        AddItemToList("Name of Expense","Value",true);
    }

    public void RemoveItemFromList()
    {
        //find tag if it exists and make amount 0
        Structs.Tag zeroTag = lvlOne.tagList.Find(i => i.name == curList[int.Parse(lvlOne.openListItem.name)].name);
        zeroTag.value = 0;
        lvlOne.tagList.Remove(lvlOne.tagList.Find(i => i.name == curList[int.Parse(lvlOne.openListItem.name)].name));
        lvlOne.tagList.Add(zeroTag);

        curList.RemoveAt(int.Parse(lvlOne.openListItem.name));
        lvlOne.SaveData();
        Destroy(lvlOne.openListItem);
        //nav.CloseEditPanel();
    }

    public void SaveItemInList(string n, float v, int ms, int ys, bool exavg, bool merge) {
        if (merge) { 
            int index = CheckForDuplicateName(n);
            if (index != -1)
            {
                //merge
                float previousValue = curList[index].value;
                curList[index] = lvlOne.NewMoneyEntry(n, v + previousValue, ms, ys,exavg);
            }
            //TODO its the same as below we might be better off with just one
            else {
                //there is no other item with the same name on the list just save it.
                Structs.MoneyEntry newEntry = lvlOne.NewMoneyEntry(n, v, ms, ys,exavg);
                curList.Add(newEntry);
            }
        }
        else
        {
            //not on list
            Structs.MoneyEntry newEntry = lvlOne.NewMoneyEntry(n,v,ms,ys,exavg);
            curList.Add(newEntry);
        }
        ClearList();
        nav.PickListClick(curListName);
        lvlOne.SaveData();
    }


    public void PopulateList(List<Structs.MoneyEntry> ltd) {
        curList = ltd;
        int count = 0;
        foreach (Structs.MoneyEntry me in ltd) {
            AddItemToList(me.name,me.value.ToString(),false,count);
            count++;
        }
    }

    public void PopulateTagList(List<Structs.Tag> ltd)
    {
        int count = 0;
        foreach (Structs.Tag me in ltd)
        {
            AddItemToList(me.name, me.value.ToString(), false, count);
            count++;
        }
    }

    public void ClearList()
    {
        foreach (Transform go in itemParent) {
            Destroy(go.gameObject);
        }
    }
}
