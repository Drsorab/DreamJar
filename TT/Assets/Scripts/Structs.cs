using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

public class Structs : MonoBehaviour {
    public enum belongToList {income, standarExpense, randomExpense };

    [Serializable]
    public struct MoneyEntry {
        public DateTime date;
        public string name;
        public float value;
        public int months;
        public bool excludeFromAvenrage;
    }
    [Serializable]
    public struct MonthStats {
        public List<MoneyEntry> income;
        public List<MoneyEntry> standarExpenses;
        public List<MoneyEntry> randomExpenses;
        public string month;
        public string year;
        public float balance { get; set; }
        public float avgBalance { get; set; }
    }

    [Serializable]
    public class DataCollection
    {
        [SerializeField]
        public List<MonthStats> Data;
        [SerializeField]
        public List<float> MonthAverages;
    }
    //public static DataTable ToDataTable<T>(this IList<T> data)
    //{
    //    PropertyDescriptorCollection props =
    //    TypeDescriptor.GetProperties(typeof(T));
    //    DataTable table = new DataTable();
    //    for (int i = 0; i < props.Count; i++)
    //    {
    //        PropertyDescriptor prop = props[i];
    //        table.Columns.Add(prop.Name, prop.PropertyType);
    //    }
    //    object[] values = new object[props.Count];
    //    foreach (T item in data)
    //    {
    //        for (int i = 0; i < values.Length; i++)
    //        {
    //            values[i] = props[i].GetValue(item);
    //        }
    //        table.Rows.Add(values);
    //    }
    //    return table;
    //}
    //UserList.ToDataTable<User>();

}
