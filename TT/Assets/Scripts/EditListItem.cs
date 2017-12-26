using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditListItem : MonoBehaviour {
    [HideInInspector]
    public string nameField;
    [HideInInspector]
    public float valueField;
    [HideInInspector]
    public int monthsField = -1;
    [HideInInspector]
    public int yearsField = -1;

    public GameObject listManagerObj;
    public GameObject messageGO;
    public GameObject nameText;
    public GameObject valueText;
    public GameObject permaButton;
    public GameObject addAverage;
    public GameObject excludeAverageButtonText;

    public Sprite dissabledField;
    public Sprite enabledField;
    public Sprite permEnabled;
    public Sprite permDisabled;
    public bool perm = true;
    bool populated = false;
    [HideInInspector]
    public bool excludeFromAverage = false;
    public List<GameObject> permanentBound = new List<GameObject>();
    public List<GameObject> permanentPanel = new List<GameObject>();
    ListManager listMngr;
    LevelOne lvlOne;
    Navigation nav;
    public List<GameObject> UiButtons;
	// Use this for initialization
	void Awake () {
        listMngr = listManagerObj.GetComponent<ListManager>();
        lvlOne = GameObject.Find("Canvas").GetComponent<LevelOne>();
        nav = lvlOne.GetComponent<Navigation>();
	}

    private void OnEnable()
    {
        if (!populated)
        {
            if (listMngr.curListName.Contains("Random"))
            {
                //DisablePerms(false);
                EnableDisablePerms(false, false);
            }
            else
            {
                //DisablePerms(true);
                HandlePermanentBound();
                perm = true;
            }
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public void DoneChangingName(string input)
    {
        nameField = input;
    }

    public void DoneChangingValue(string input)
    {
        if (input != "")
            valueField = float.Parse(input);
        else
        {
            valueField = 0;
        }
    }

    public void DoneMonths(string input)
    {
        if(input!="")
            monthsField = int.Parse(input);
        else
        {
            monthsField = 0;
        }
    }

    public void DoneYears(string input)
    {
        if (input != "")
            yearsField = int.Parse(input);
        else
        {
            yearsField = 0;
        }
    }

    public void SaveItem() {
        if (nameField == "")
        {
            messageGO.GetComponent<Text>().text = "Name can't be empty";
            return;
        }
        else if (valueField == 0) {
            messageGO.GetComponent<Text>().text = "Value can't be zero";
            return;
        }
        //TODO check if valid value, name can be default
        if (String.IsNullOrEmpty(lvlOne.curListItem.name) && lvlOne.curListItem.value == 0)
        {
            //new item to be saved
            if (listMngr.CheckForDuplicateName(nameField) == -1)
            {
                listMngr.SaveItemInList(nameField, valueField, monthsField, yearsField,excludeFromAverage, false);
                nav.GoBack();
            }
            else
            {
                nav.OpenMergePanel();
            }
        }
        else
        {
            //existing item modified to be saved.
            if (listMngr.CheckForDuplicateName(nameField) == -1 || nameField == lvlOne.curListItem.name)
            {
                //REMARK: better to handle zero value in both fields
                if (monthsField == 0 && yearsField == 0)
                    perm = true;

                if (perm)
                    lvlOne.UpdateSavedEntry(nameField, valueField, -1, -1, excludeFromAverage);
                else
                {
                    lvlOne.UpdateSavedEntry(nameField, valueField, monthsField, yearsField, excludeFromAverage);
                }
                nav.GoBack();
            }
            else
            {
                nav.editing = true;
                nav.OpenMergePanel();
            }
        }
        lvlOne.SaveData();
    }

    public void DeleteItem() {
        if (String.IsNullOrEmpty(lvlOne.tagList.Find(i => i.name == listMngr.curList[int.Parse(lvlOne.openListItem.name)].name).name)) {
            listMngr.RemoveItemFromList();
            nav.GoBack();
        } else {
            nav.OpenMergePanel(true);
        }
    }

    public void ResetEditPanel() {
        populated = false;
        perm = true;
        messageGO.GetComponent<Text>().text = "";
        nameText.GetComponent<InputField>().text="";
        valueText.GetComponent<InputField>().text="";
        permanentBound[0].GetComponent<InputField>().text = "0";
        permanentBound[1].GetComponent<InputField>().text = "0";
        monthsField = 0;
        yearsField = 0;
        nameField = "";
        valueField = 0;
        UiButtons[0].SetActive(true);
        UiButtons[1].transform.localPosition = new Vector3(257, -490, 0);
        UiButtons[2].transform.localPosition = new Vector3(0, -490, 0);
    }

    public void ButtonPositions() {
        UiButtons[0].SetActive(false);
        UiButtons[1].transform.localPosition = new Vector3(150,-490,0);
        UiButtons[2].transform.localPosition = new Vector3(-150,-490,0);
    }

    public void PopulateEditPanel(Structs.MoneyEntry item) {
        nameText.GetComponent<InputField>().text = item.name;
        valueText.GetComponent<InputField>().text = item.value.ToString();

        nameField = nameText.GetComponent<InputField>().text;
        valueField = float.Parse(valueText.GetComponent<InputField>().text);
        excludeAverageButtonText.GetComponent<Text>().text = item.excludeFromAvenrage ? "Yes" : "No";

        if (item.months > 0) {
            Awake();
            perm = false;
            populated = true;
            HandlePermanentBound();
            int ys = item.months / 12;
            permanentBound[0].GetComponent<InputField>().text = (item.months - ys * 12).ToString();
            permanentBound[1].GetComponent<InputField>().text = ys.ToString();
            monthsField = int.Parse(permanentBound[0].GetComponent<InputField>().text);
            yearsField = int.Parse(permanentBound[1].GetComponent<InputField>().text);
        }
    }

    public void PermawButtonClick() {
        perm = perm ? false : true;
        HandlePermanentBound();
    }

    public void HandlePermanentBound() {
        if (perm)
        {
            EnableDisablePerms(false, true);
            permaButton.GetComponent<Image>().sprite = permEnabled;
            monthsField = -1;
            permanentBound[0].GetComponent<InputField>().text = "0";
            yearsField = -1;
            permanentBound[1].GetComponent<InputField>().text = "0";
        }
        else {
            EnableDisablePerms(true,true);
            permaButton.GetComponent<Image>().sprite = permDisabled;
            monthsField = 0;
            permanentBound[0].GetComponent<InputField>().text = "0";
            yearsField = 0;
            permanentBound[1].GetComponent<InputField>().text = "0";
        }
    }

    void EnableDisablePerms (bool enable, bool show)
    {
            foreach (GameObject go in permanentPanel)
            {
                go.SetActive (show);
            }

        if (!listMngr.curListName.Contains("Random"))
        {
            foreach (GameObject go in permanentBound)
            {
                go.GetComponent<InputField>().enabled = enable;
                go.GetComponent<Image>().sprite = enable ? enabledField : dissabledField;
            }
            addAverage.SetActive(false);
        }
        else {
            addAverage.SetActive(true);
        }
    }

    public void IncludeInAverage() {
        excludeAverageButtonText.GetComponent<Text>().text = excludeAverageButtonText.GetComponent<Text>().text == "Yes" ? "No" : "Yes";
        excludeFromAverage = excludeAverageButtonText.GetComponent<Text>().text=="Yes"?true:false;
    }
}
