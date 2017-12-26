using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickExpensePanel : MonoBehaviour {
    public Navigation nav;
    LevelOne lvlOne;
    public GameObject nameField;
    public GameObject moneyField;
    string nField = "";
    float totalValue = 0;
    public bool hideMoneyButtons = false;
    public List<GameObject> hideList = new List<GameObject>();
    public List<GameObject> buttonList = new List<GameObject>();

    // Use this for initialization
    void OnEnable () {
        string s = nameField.GetComponent<InputField>().text;
        if (String.IsNullOrEmpty(nameField.GetComponent<InputField>().text))
            nField = nameField.GetComponent<InputField>().text = DateTime.Now.ToString();
        else
        {
            nField = nameField.GetComponent<InputField>().text;
        }
        lvlOne = GameObject.Find("Canvas").GetComponent<LevelOne>();
        foreach (GameObject g in hideList) {
            g.SetActive(!hideMoneyButtons);
        }
    }

    private void OnDisable()
    {
        ResetQuickExpenses();
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void Add(int value) {
        totalValue += value;
        moneyField.GetComponent<InputField>().text = totalValue.ToString();
    }
    public void Subtract(int value)
    {
        if (totalValue - value >= 0)
        {
            totalValue -= value;
            moneyField.GetComponent<InputField>().text = totalValue.ToString();
        }
    }
    public void SaveIt() {
        SaveQuickExpense();
    }

    public void DeleteIt()
    {
        lvlOne.tagList.RemoveAt(int.Parse(lvlOne.openListItem.name));
        lvlOne.SaveData();
        Destroy(lvlOne.openListItem);
    }

    public void ResetQuickExpenses() {
        nameField.GetComponent<InputField>().text = "";
        moneyField.GetComponent<InputField>().text = "";
        nField = "";
        totalValue = 0;
        buttonList[0].SetActive(false);
        buttonList[1].transform.localPosition = new Vector3(-150, -490, 0);
        buttonList[2].transform.localPosition = new Vector3(150, -490, 0);
    }

    void SaveQuickExpense() {

        if (String.IsNullOrEmpty(nField))
            return;

        if (hideMoneyButtons)
        {
            //new tag
            if (String.IsNullOrEmpty(lvlOne.tagList.Find(i => i.name == nField).name))
            {
                Structs.Tag newTag = new Structs.Tag()
                {
                    name = nField+"-tag",
                    value = totalValue
                };
                lvlOne.AddToList("tagList", new Structs.MoneyEntry(), newTag);
                //if (totalValue > 0) {
                    Structs.MoneyEntry newRand = lvlOne.NewMoneyEntry(nField + "-tag", totalValue, -1, 0, false);
                    lvlOne.AddToList("randomExpenses", newRand, new Structs.Tag());
               //}
            }
            //tag exists
            else {
                Structs.MoneyEntry newRand =  lvlOne.selectedMonth.randomExpenses.Find(i => i.name == nField);
                newRand.value += totalValue;
                lvlOne.selectedMonth.randomExpenses.Remove(lvlOne.selectedMonth.randomExpenses.Find(i => i.name == nField));
                lvlOne.selectedMonth.randomExpenses.Add(newRand);

                lvlOne.tagList.Remove(lvlOne.tagList.Find(i => i.name == nField));
                Structs.Tag newTag = new Structs.Tag()
                {
                    name = newRand.name,
                    value = newRand.value
                };
                lvlOne.tagList.Add(newTag);
                lvlOne.SaveData();
                Debug.Log(lvlOne.selectedMonth);
            }
        }
        else
        {
            if (nameField.transform.Find("Text").gameObject.GetComponent<Text>().text == "")
            {
                nField = nameField.GetComponent<InputField>().text = DateTime.Now.ToString();
            }
            if (moneyField.GetComponent<InputField>().text == "")
                totalValue = 0;
            Structs.MoneyEntry newRand = lvlOne.NewMoneyEntry(nField, totalValue,-1, 0,false);
            lvlOne.AddToList("randomExpenses", newRand, new Structs.Tag());
        }
        lvlOne.SaveData();
        ResetQuickExpenses();
    }

    public void ButtonPositions()
    {
        buttonList[0].SetActive(true);
        buttonList[1].transform.localPosition = new Vector3(0, -490, 0);
        buttonList[2].transform.localPosition = new Vector3(257, -490, 0);
    }

    public void DoneChangingName(string input)
    {
        nField = input;
    }

    public void DoneChangingValue(string input)
    {
        totalValue = float.Parse(input);
    }
}
