using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
