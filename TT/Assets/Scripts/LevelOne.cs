using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class LevelOne : MonoBehaviour {
    public Text gross;
    public Text net;
    public Text balanceTxt;
    public Text hackTxt;
    public GameObject listPanel;
    ListManager listMngr;
    Navigation nav;
    NewMonth welcomeNewMonth;
    public GameObject selectionImage;
    public GameObject predictions;
    public Sprite selectionImage2;
    public Sprite defaultButton;
    public GameObject monthButton;
    public GameObject balance;
    public Sprite buttDisabled;
    [HideInInspector]
    public GameObject openListItem;
    public Structs.MonthStats selectedMonth;
    public string curMonth;
    public string curYear;

    public Structs.MoneyEntry curListItem = new Structs.MoneyEntry();
    Structs.DataCollection loadedData = null;

    [HideInInspector]
    public Dictionary<string,int> MonthToInt = new Dictionary<string, int>()
                {
                    { "january", 0},
                    { "february", 1},
                    { "march", 2},
                    { "april", 3},
                    { "may", 4},
                    { "june", 5},
                    { "july", 6},
                    { "august", 7},
                    { "september", 8},
                    { "october", 9},
                    { "november", 10},
                    { "december", 11}
                };

public List<GameObject> ItemsToDisableForPrevMonths = new List<GameObject>();


    void Start()
    {
        nav = GetComponent<Navigation>();
        listMngr = listPanel.GetComponent<ListManager>();
        welcomeNewMonth = GetComponent<NewMonth>();
        curMonth = DateTime.Now.ToString("MMMM");
        curYear = DateTime.Now.Year.ToString();
        loadedData = LoadData();
        GetComponent<Predictions>().CheckIfThereIsDictionaryInData(loadedData);
        CheckForCurMonth();
        monthButton.GetComponentInChildren<Text>().text = curMonth + " " + curYear;
        if(loadedData.Data.Count>0)
            selectedMonth = loadedData.Data[loadedData.Data.Count - 1];
        BalanceButton();
    }

    public void SetCalendarButton(string m, string y) {
        curMonth = m;
        curYear = y;
        monthButton.GetComponentInChildren<Text>().text = curMonth + " " + curYear;
    }

    void BalanceButton() {
        if (DateTime.Now.Year.ToString() == curYear && DateTime.Now.ToString("MMMM") == curMonth)
        {
            balance.GetComponent<Button>().enabled = false;
            balance.GetComponent<Button>().GetComponent<Image>().sprite = buttDisabled;
        }
        else {

        }
    }

    public List<string> Blaarg(string y) {
        List<Structs.MonthStats> ls = loadedData.Data.Where(x => x.year ==y).ToList();
        List<string> ms = new List<string>();
        foreach (Structs.MonthStats l in ls) {
            ms.Add(l.month.ToLower());
        }
        return ms;
    }

    bool EntryExists() {
        foreach (Structs.MonthStats l in loadedData.Data)
        {
            if (l.month == curMonth && l.year == curYear)
                return true;
        }
        return false;
    }

    public void CheckForCurMonth() {

        if (loadedData!=null && EntryExists())
        {
            //entryExists for this month of the year
            CalculateMonthStats();
        }
        //else create it
        else
        {

            //TODO should we try again to load data incase START missed them??
            if (loadedData == null)
            {
                CreateNewData();
                SaveData();
            }
            else
            {
                loadedData.Data.Add( loadedData.Data.Count>0 ? welcomeNewMonth.WelcomeNewMonth(loadedData.Data[loadedData.Data.Count - 1]): new Structs.MonthStats());
                
                if (loadedData.Data.Count>1)
                    GetComponent<Predictions>().GetMonthAverage(loadedData, loadedData.Data[loadedData.Data.Count-2].month); //uparxei minas prin vgaltou to avg gia sigoura
                SaveData();
            }

        }
    }

    void CreateNewData() {
        loadedData = new Structs.DataCollection();
        loadedData.MonthAverages = new List<float>();
        for (int i = 1; i < 13; i++) {
            loadedData.MonthAverages.Add(0);
        }
        loadedData.Data = new List<Structs.MonthStats>();
        loadedData.Data.Add(new Structs.MonthStats()
        {
            month = curMonth,
            year = curYear,
            income = new List<Structs.MoneyEntry>(),
            standarExpenses = new List<Structs.MoneyEntry>(),
            randomExpenses = new List<Structs.MoneyEntry>()
        });
    }

    public float SumListItems(List<Structs.MoneyEntry> lst, bool avg_balance=false) {
        float res = 0;
        foreach (Structs.MoneyEntry inc in lst)
        {
            if((avg_balance && (!inc.excludeFromAvenrage)) || !avg_balance)
                res += inc.value;
        }
        return res;
    }

    void CalculateMonthStats() {
        GetMonthData();
        UpdateBalance();
        GetComponent<Predictions>().GetMonthAverage(loadedData, curMonth);
        float income = 0;
        float sdrExpenses = 0;
        float rndExpenses = 0;

        income = SumListItems(selectedMonth.income);
        sdrExpenses = SumListItems(selectedMonth.standarExpenses);
        rndExpenses = SumListItems(selectedMonth.randomExpenses);

        float gp = income - sdrExpenses;
        float np = gp - rndExpenses;

        //TODO add previous months balance plus current net
        //incase he selects from calendar another month than the current
        //if (curMonth == DateTime.Now.ToString("MMMM") && curYear == DateTime.Now.Year.ToString())
        //{
        //    balanceTxt.text = "Balance: " + (selectedMonth.balance + np).ToString();
        //}
        //else {
            balanceTxt.text = "Balance: " + selectedMonth.balance.ToString();
        //}

        gross.text = "Gross: " + gp.ToString();
        net.text = "Net: " + np.ToString();
    }

    public void SaveData() {
        string path = Application.persistentDataPath + "/Data.json";
        string json = JsonUtility.ToJson(loadedData);
        Debug.Log("json:" + json);
        File.WriteAllText(path, json);

    }

    Structs.DataCollection LoadData() {
        Debug.Log(Application.persistentDataPath);
        if (CheckJsonExistance())
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/Data.json");
            loadedData = JsonUtility.FromJson<Structs.DataCollection>(json);

            //TODO remove
            //GetComponent<Predictions>().GetMonthAverage(loadedData);
            //Debug.Log(loadedData.Data[0].randomExpenses[1].inAvenrage);
            return loadedData;
        }
        else {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/Data.json","");
            return null;
        }
    }

    bool CheckJsonExistance() {
        string filePath = Application.persistentDataPath + "/Data.json";

        if (System.IO.File.Exists(filePath))
        {
            // The file exists -> run event
            return true;
        }
        // The file does not exist -> run event
        return false;
    }

    public void GetMonthData() {
        //check month from button to load items
        foreach (Structs.MonthStats ms in loadedData.Data)
        {
            if (ms.month == curMonth && ms.year == curYear)
            {
                selectedMonth = ms;
            }
        }
        //TODO handle if selectedMonth = null
    }


    public void UpdateSavedEntry(string n, float v, int ms, int ys, bool exavg=true) {
        Button b = openListItem.transform.GetChild(0).gameObject.GetComponent<Button>();
        //if (n!=curListItem.name && listMngr.CheckForDuplicateName(n) != -1) {
        //    nav.editing = true;
        //    nav.OpenMergePanel();
        //    return;
        //}
        b.transform.Find("Name").GetComponent<Text>().text = n;
        b.transform.Find("Value").GetComponent<Text>().text = v.ToString();
        if (listMngr.curList.Count == 0)
        {
            Structs.MoneyEntry me = new Structs.MoneyEntry()
            {
                date = DateTime.Now,
                name = n,
                value = v,
                months = (ms + 12 * ys) < -1 ? -1 : ms + 12 * ys,
                excludeFromAvenrage = exavg
            };

            listMngr.curList.Add(me);
        }
        else
        {
            if (int.Parse(openListItem.name) >= listMngr.curList.Count)

                listMngr.curList.Add(new Structs.MoneyEntry()
                {
                    date = DateTime.Now,
                    name = n,
                    value = v,
                    months = (ms + 12 * ys) < -1 ? -1 : ms + 12 * ys,
                    excludeFromAvenrage = exavg
                });
            else
                listMngr.curList[int.Parse(openListItem.name)] = new Structs.MoneyEntry()
                {
                    date = DateTime.Now,
                    name = n,
                    value = v,
                    months = (ms + 12 * ys) < -1 ? -1 : ms + 12 * ys,
                    excludeFromAvenrage = exavg
                };
        }
        UpdateBalance();
        //update month average data by sending the month
        GetComponent<Predictions>().GetMonthAverage(loadedData, curMonth);
    }

    void UpdateBalance() {
        selectedMonth.balance = SumListItems(selectedMonth.income) - SumListItems(selectedMonth.standarExpenses) - SumListItems(selectedMonth.randomExpenses);
        selectedMonth.avgBalance = SumListItems(selectedMonth.income) - SumListItems(selectedMonth.standarExpenses) - SumListItems(selectedMonth.randomExpenses,true);
        int index = loadedData.Data.FindIndex(a => a.month == selectedMonth.month && a.year == selectedMonth.year);

        loadedData.Data[index] = new Structs.MonthStats()
        {
            income = loadedData.Data[index].income,
            standarExpenses = loadedData.Data[index].standarExpenses,
            randomExpenses = loadedData.Data[index].randomExpenses,
            month = loadedData.Data[index].month,
            year = loadedData.Data[index].year,
            balance = selectedMonth.balance, 
            avgBalance = selectedMonth.avgBalance
        };

        hackTxt.text = "\n balance :" + selectedMonth.balance.ToString() + "\n avgBalance :" + selectedMonth.avgBalance.ToString();
    }


    public void AddToList(string where, Structs.MoneyEntry what) {
        switch (where)
        {
            case "income":
                selectedMonth.income.Add(what);
                break;
            case "standarExpenses":
                selectedMonth.standarExpenses.Add(what);
                break;
            case "randomExpenses":
                selectedMonth.randomExpenses.Add(what);
                break;
        }
    }

    public Structs.MoneyEntry NewMoneyEntry(string n, float v, int ms, int ys, bool exavg)
    {
        Structs.MoneyEntry newEntry = new Structs.MoneyEntry()
        {
            date = DateTime.Now,
            name = n,
            value = v,
            months = (ms + 12 * ys) < -1 ? -1 : ms + 12 * ys,
            excludeFromAvenrage = exavg
        };

        return newEntry;
    }

    public void OnPointerEnter(GameObject gb)
    {
        if (!gb.name.Contains("Income") && !gb.name.Contains("Random") && !gb.name.Contains("Standar"))
            selectionImage.transform.position = gb.transform.position;
        else
            gb.GetComponent<Button>().GetComponent<Image>().sprite = selectionImage2;
    }

    public void OnPointerExit(GameObject gb)
    {
        if (!gb.name.Contains("Income") && !gb.name.Contains("Random") && !gb.name.Contains("Standar"))
            selectionImage.transform.localPosition = new Vector3(137,750,0);
        else
            gb.GetComponent<Button>().GetComponent<Image>().sprite = defaultButton;
    }

    public void CallMakePredictionByDate(string m , string y) {
        predictions.GetComponent<PredictionByDate>().PredictByDate(m,y,loadedData);
    }

    public void CallMakePredictionByAmount()
    {
        predictions.GetComponent<PredictionByDate>().PredictByAmount(curMonth, curYear, loadedData);
    }

    IEnumerator ClickFlash() {
        yield return new WaitForSeconds(0.1f);
        selectionImage.transform.position = new Vector3(0,1750,0);
    }
}
