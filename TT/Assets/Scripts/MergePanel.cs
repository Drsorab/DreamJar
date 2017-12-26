using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergePanel : MonoBehaviour {
    public Text mainText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeMainText(string txt) {
        mainText.text = txt;
    }
}
