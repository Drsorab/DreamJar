using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonth : MonoBehaviour {
    LevelOne gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GetComponent<LevelOne>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Structs.MonthStats WelcomeNewMonth(Structs.MonthStats stats) {
       Start();
       return TransferPreviousMonthValues(stats);
    }

    Structs.MonthStats TransferPreviousMonthValues(Structs.MonthStats stats)
    {

        List<Structs.MoneyEntry> incList = TransferValues(stats.income);
        List<Structs.MoneyEntry> expList = TransferValues(stats.standarExpenses);



        Structs.MonthStats newMonth = new Structs.MonthStats()
        {
            month = gameManager.curMonth,
            year = gameManager.curYear,
            income = incList,
            standarExpenses = expList,
            randomExpenses = new List<Structs.MoneyEntry>(),
            balance = stats.balance + CalcPreviousMonthBalance(stats),
            avgBalance = stats.balance + CalcPreviousMonthBalance(stats,true)
        };
        return newMonth;
    }

    float CalcPreviousMonthBalance(Structs.MonthStats stats, bool avg_balance = false) {
        return gameManager.AddListItems(stats.income) - gameManager.AddListItems(stats.standarExpenses) - gameManager.AddListItems(stats.randomExpenses, avg_balance);
    }

    List<Structs.MoneyEntry> TransferValues(List<Structs.MoneyEntry> statsList)
    {
        List<Structs.MoneyEntry> tempList = new List<Structs.MoneyEntry>();
        foreach (Structs.MoneyEntry inc in statsList)
        {
            if (inc.months-1 > 0)
                tempList.Add(new Structs.MoneyEntry()
                {
                    date = inc.date,
                    name = inc.name,
                    value = inc.value,
                    months = inc.months - 1
                });
        }
        return tempList;
    }

    //go to previous month
    //add balance
    //bring down months
    //populate new months income and standard expenses
}
