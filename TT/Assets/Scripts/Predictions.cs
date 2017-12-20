using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Predictions : MonoBehaviour {
    LevelOne gameManager;
    // Use this for initialization
    void Start()
    {
        gameManager = GetComponent<LevelOne>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void CheckIfThereIsDictionaryInData(Structs.DataCollection loadedData) {
        if (loadedData.MonthAverages == null || loadedData.MonthAverages.Count==0) {
            loadedData.MonthAverages = new List<float>();
            for (int i = 1; i < 13; i++)
            {
                loadedData.MonthAverages.Add(0);
            }
            gameManager.SaveData();
        }
    }

    public void GetMonthAverage(Structs.DataCollection loadedData, string month) {
        if (gameManager == null)
            Start();

        int idx = gameManager.MonthToInt[month.ToLower()];

        List<Structs.MonthStats> ls = loadedData.Data.Where(x => x.month.ToLower() == month.ToLower()).ToList();
        float total = 0;
        foreach (Structs.MonthStats item in ls) {
            total += item.avgBalance;
        }
        float avg = total / ls.Count;
        loadedData.MonthAverages[idx] = avg;
        gameManager.SaveData();
    }
}
