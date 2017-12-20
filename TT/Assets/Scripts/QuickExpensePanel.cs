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

    public List<GameObject> hideList = new List<GameObject>();

    // Use this for initialization
    void OnEnable () {
        string s = nameField.GetComponent<InputField>().text;
        if (String.IsNullOrEmpty(nameField.GetComponent<InputField>().text))
            nField = nameField.GetComponent<InputField>().text = DateTime.Now.ToString();
        else
        {
            nField = nameField.GetComponent<InputField>().text = "";
        }
        lvlOne = GameObject.Find("Canvas").GetComponent<LevelOne>();
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

    public void ResetQuickExpenses() {
        nameField.GetComponent<InputField>().text = "";
        moneyField.GetComponent<InputField>().text = "";
        nField = "";
        totalValue = 0;
    }

    void SaveQuickExpense() {
        if (nameField.transform.Find("Text").gameObject.GetComponent<Text>().text == "") {
            nField = nameField.GetComponent<InputField>().text = DateTime.Now.ToString();
        }
        if (moneyField.GetComponent<InputField>().text == "")
            totalValue = 0;

        Structs.MoneyEntry newRand = new Structs.MoneyEntry()
        {
            date = DateTime.Now,
            name = nField,
            value = totalValue,
            months = -1,
            excludeFromAvenrage = false
        };
        lvlOne.AddToList("randomExpenses", newRand);
        lvlOne.SaveData();
        ResetQuickExpenses();
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
