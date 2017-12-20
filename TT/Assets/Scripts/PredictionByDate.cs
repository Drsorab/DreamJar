using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PredictionByDate : MonoBehaviour {

    public GameObject calendarButton;
    public LevelOne gameManager;
    public Text answerTxt;
    public Text amountAnswerTxt;
    float desiredTotal=0;


	public void PopulateMe () {
        calendarButton.GetComponentInChildren<Text>().text = gameManager.curMonth + " " + gameManager.curYear;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PredictByDate(string month, string year, Structs.DataCollection loadedData) {
        //write code for prediction. This is called after we close calendar
        calendarButton.GetComponentInChildren<Text>().text = month + " " + year;
        int idx_now = gameManager.MonthToInt[DateTime.Now.ToString("MMMM").ToLower()];
        int idx_then = gameManager.MonthToInt[month.ToLower()];
        int year_now = int.Parse(DateTime.Now.Year.ToString());
        int year_then = int.Parse(year);

        int monthYears = (year_then - year_now) * 12;
        int monthSolo = idx_then - idx_now;

        int totalMonths = monthYears + monthSolo;
        float predictionTotal = 0;
        int count = 0;
        float lastFoundAvg = GetFirstAverage(idx_now, loadedData);

        while (count < totalMonths) {

            int realCount = idx_now + count;
            while(realCount > 11)
                realCount -= 12 ;

            if (loadedData.MonthAverages[realCount] != 0) {
                lastFoundAvg = loadedData.MonthAverages[realCount];
            }

            predictionTotal += lastFoundAvg;
            count++;
        }
        //fail safe if you choose the current month and program does do in the while loop above
        if (predictionTotal == 0)
            predictionTotal = lastFoundAvg;

        answerTxt.text = "Predicted balance : " + predictionTotal.ToString();
    }

    public void valueChanged(string input) {
        if (input != "")
            desiredTotal = float.Parse(input);
        else
        {
            desiredTotal = 0;
        }
    }

    public void PredictByAmount(string month, string year, Structs.DataCollection loadedData) {
        int idx_now = gameManager.MonthToInt[month.ToLower()];
        float lastFoundAvg = GetFirstAverage(idx_now, loadedData);
        float calcTotal = 0;
        int count = 0;
        int predictionMonth = 0;
        int y = 0;
        while (calcTotal <= desiredTotal)
        {

            int realCount = idx_now + count;
            y = 0;
            while(realCount > 11)
            {
                realCount -= 12;
                y++;
            }

            if (loadedData.MonthAverages[realCount] != 0)
            {
                lastFoundAvg = loadedData.MonthAverages[realCount];              
            }
            predictionMonth = realCount;
            calcTotal += lastFoundAvg;
            count++;
        }

        //int y = count / 12;
        //int m = count - y * 12;
        string myKey = gameManager.MonthToInt.FirstOrDefault(x => x.Value == predictionMonth).Key;
        amountAnswerTxt.text = myKey + " / " + (int.Parse(year) + y).ToString();
    }

    float GetFirstAverage(int idx, Structs.DataCollection loadedData) {
        //check incase current month has 0 as average get the next month that does not
        if (loadedData.MonthAverages[idx] == 0)
        {
            for (int i = idx; i < 12; i++)
            {
                if (loadedData.MonthAverages[i] != 0)
                {
                    return loadedData.MonthAverages[i];
                }
                if (i == 11)
                    i = -1;
            }
        }
        return 0;
    }
}
