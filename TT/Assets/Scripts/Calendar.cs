using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour {
    public Navigation nav;
    public Text year;
    public Text today;
    public LevelOne lvlone;
    public PredictionByDate predictions;
    public List<GameObject> MonthButtons = new List<GameObject>();
    public Sprite disabled;
    public Sprite enabledSprite;
    string cameFrom = "";
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DisableNextMonths() {
        List<string> ms = new List<string>();
        ms=lvlone.Blaarg(year.text);
            for (int i = 0; i < MonthButtons.Count; i++)
            {
                if (!ms.Contains(MonthButtons[i].name.ToLower()))
                {
                    MonthButtons[i].GetComponent<Button>().enabled = false;
                    MonthButtons[i].GetComponent<Button>().GetComponent<Image>().sprite = disabled;
                }
                else {
                    MonthButtons[i].GetComponent<Button>().enabled = true;
                    MonthButtons[i].GetComponent<Button>().GetComponent<Image>().sprite = enabledSprite;
                }
            }
    }

    void DisablePreviousMonths()
    {
        //List<string> ms = new List<string>();
        //ms = lvlone.Blaarg(year.text);

        for (int i = 0; i < MonthButtons.Count; i++)
        {
            if (MonthButtons[i].name.ToLower() != lvlone.curMonth.ToLower())
            {
                MonthButtons[i].GetComponent<Button>().enabled = false;
                MonthButtons[i].GetComponent<Button>().GetComponent<Image>().sprite = disabled;
            }
            else
            {
                break;
            }
        }
    }

    void EnableAllMonths()
    {
        for (int i = 0; i < MonthButtons.Count; i++)
        {
            MonthButtons[i].GetComponent<Button>().enabled = true;
            MonthButtons[i].GetComponent<Button>().GetComponent<Image>().sprite = enabledSprite;
        }
    }

    public void InitializeCalendar(string y, string path) {
        Start();
        year.text = y;
        today.text = "Today " + DateTime.Now.ToString("dd-MM-yyyy");
        cameFrom = path;
        if (path == "edit")
            DisableNextMonths();
        else
            DisablePreviousMonths();
    }

    public void ChangeMonth(string m) {
        if (cameFrom == "edit")
        {
            lvlone.SetCalendarButton(m, year.text);
        }
        else {
            lvlone.CallMakePredictionByDate(m,year.text);
        }
    }

    public void YearUp() {
        year.text = (int.Parse(year.text) + 1).ToString();

        if (cameFrom == "edit")
            DisableNextMonths();
        else {
            if (int.Parse(year.text) > int.Parse(DateTime.Now.Year.ToString()))
            {
                EnableAllMonths();
            }
           
        }
    }

    public void YearDown() {
        string yt = (int.Parse(year.text) - 1).ToString();
        
        if (cameFrom == "edit")
            DisableNextMonths();
        else {
            if (int.Parse(yt) < int.Parse(DateTime.Now.Year.ToString()))
            {
                return;
            }
            else if (int.Parse(yt) == int.Parse(DateTime.Now.Year.ToString()))
            {
                DisablePreviousMonths();
            }
            else {
                EnableAllMonths();
            }
        }
        year.text = yt;
    }
}
