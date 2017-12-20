using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Navigation : MonoBehaviour {

    ListManager listMngr;
    LevelOne gameMngr;
    public GameObject introPanel;
    public GameObject monthMenuPanel;
    public GameObject mergePanel;
    public GameObject editPanel;
    public GameObject quickExpensePanel;
    public GameObject calendar;
    public GameObject loadingScreen;
    public GameObject predictionsDate;
    public GameObject predictionsAmount;
    public GameObject listPanel;
    public GameObject tagListPanel;
    public bool editing = false;
    string path = "";

    List<GameObject> NavigationHistory = new List<GameObject>();
    // Use this for initialization
    void Start () {
        gameMngr = GetComponent<LevelOne>();
        listMngr = listPanel.GetComponent<ListManager>();
        NavigationHistory.Add(loadingScreen);

        introPanel.SetActive(false);
        monthMenuPanel.SetActive(false);
        mergePanel.SetActive(false);
        editPanel.SetActive(false);
        quickExpensePanel.SetActive(false);
        calendar.SetActive(false);
        loadingScreen.SetActive(true);
        predictionsDate.SetActive(false);
        predictionsAmount.SetActive(false);
        listPanel.SetActive(false);
        tagListPanel.SetActive(false);
}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (NavigationHistory.Count > 1)
                GoBack();
            else
                Application.Quit();

        }
    }

    public void GoTo(GameObject go) {
        go.SetActive(true);
        NavigationHistory[NavigationHistory.Count - 1].SetActive(false);
        NavigationHistory.Add(go);
        switch (go.name) {
            case "PredictionByDate":
                path = "predictions";
                predictionsDate.SetActive(true);
                predictionsDate.transform.parent.gameObject.GetComponent<PredictionByDate>().PopulateMe();
                break;
            case "IntroPanel":
                path = "edit";
                gameMngr.CheckForCurMonth();
                break;
            case "MonthMenu":
                gameMngr.GetMonthData();
                break;
            case "ListPanel":
                PickListClick(EventSystem.current.currentSelectedGameObject.name);
                break;
            case "Calendar":
                calendar.GetComponent<Calendar>().InitializeCalendar(gameMngr.curYear, path);
                break;
            case "QuickExpensePanel":
                break;
            case "PredictionByAmount":
                break;
            case "EditItemPanel":
                break;
            case "Merge":
                break;
            case "LoadingScreen":
                NavigationHistory.Clear();
                NavigationHistory.Add(loadingScreen);
                break;
        }
    }

    public void GoBack()
    {
        //go.SetActive(true);
        NavigationHistory[NavigationHistory.Count - 1].SetActive(false);
        NavigationHistory.RemoveAt(NavigationHistory.Count - 1);
        NavigationHistory[NavigationHistory.Count - 1].SetActive(true);

        switch (NavigationHistory[NavigationHistory.Count - 1].name)
        {
            case "IntroPanel":
                gameMngr.CheckForCurMonth();
                break;
            case "ListPanel":
                editPanel.GetComponent<EditListItem>().ResetEditPanel();
                gameMngr.curListItem = new Structs.MoneyEntry();
                gameMngr.openListItem = null;
                PickListClick(listMngr.curListName);
                break;
        }
    }



    public void PickListClick(string listName)
    {
        listMngr.ClearList();
        switch (listName)
        {
            case "Income":
                listMngr.PopulateList(gameMngr.selectedMonth.income);
                listMngr.curListName = "Income";
                break;
            case "StandarExpenses":
                listMngr.PopulateList(gameMngr.selectedMonth.standarExpenses);
                listMngr.curListName = "StandarExpenses";
                break;
            case "RandomExpenses":
                listMngr.PopulateList(gameMngr.selectedMonth.randomExpenses);
                listMngr.curListName = "RandomExpenses";
                break;
        }
    }

    public void OpenEditPanel(GameObject obj, bool newItem)
    {
        gameMngr.openListItem = obj;
        if (!newItem)
        {
            Button b = gameMngr.openListItem.transform.GetChild(0).gameObject.GetComponent<Button>();
            gameMngr.curListItem = listMngr.curList[listMngr.CheckForDuplicateItem(b.transform.Find("Name").GetComponent<Text>().text, float.Parse(b.transform.Find("Value").GetComponent<Text>().text))];
            editPanel.GetComponent<EditListItem>().PopulateEditPanel(gameMngr.curListItem);
        }
        else
        {
            editPanel.GetComponent<EditListItem>().ButtonPositions();
        }
    }

    //public void OpenCalendar(GameObject gb)
    //{
    //    calendar.SetActive(true);
        
    //    gb.SetActive(false);
    //}

    //public void GoToPreditions(GameObject go)
    //{
    //    if (loadingScreen.activeSelf)
    //    {
    //        if (go.name.Contains("Date"))
    //        {
    //            path = "predictions";
    //            predictionsDate.SetActive(true);
    //            predictionsDate.transform.parent.gameObject.GetComponent<PredictionByDate>().PopulateMe();

    //        }
    //        else {
    //            predictionsAmount.SetActive(true);
    //        }
    //        loadingScreen.SetActive(false);
    //    }
    //}

    //public void CloseCalendar()
    //{
    //    if (path == "edit")
    //    {
    //        BackToIntro(calendar);
    //    }
    //    else
    //    {
    //        predictionsDate.SetActive(true);

    //        calendar.SetActive(false);
    //    }
    //}

    public void CloseLoadingScreen()
    {
        if (loadingScreen.activeSelf)
        {
            path = "edit";
            introPanel.SetActive(true);
            loadingScreen.SetActive(false);
        }
    }

    public void OpenQuickExpensePanel()
    {
        quickExpensePanel.SetActive(true);
        introPanel.SetActive(false);
    }

    public void OpenMergePanel()
    {
        GoTo(mergePanel);
        //editPanel.SetActive(false);
        //mergePanel.SetActive(true);
    }

    public void YesMerge()
    {
        //if you are editing something and you change the name to something already in the list. Then we must remove the item you are editing since you accepted the merge
        if (editing)
        {
            listMngr.curList.Remove(gameMngr.curListItem);
            editing = false;
        }
        listMngr.SaveItemInList(editPanel.GetComponent<EditListItem>().nameField, editPanel.GetComponent<EditListItem>().valueField, editPanel.GetComponent<EditListItem>().monthsField, editPanel.GetComponent<EditListItem>().yearsField, editPanel.GetComponent<EditListItem>().excludeFromAverage, true);
    }

    public void NoMerge()
    {
        mergePanel.SetActive(false);
        editPanel.SetActive(true);
    }

}
