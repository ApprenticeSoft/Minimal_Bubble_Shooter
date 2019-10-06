using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour {



	// Use this for initialization
	void Start () {
        //print(" transform.parent.Find(\"AdMobManager\")" + transform.parent.Find("AdMobManager"));
    }
	
	// Update is called once per frame
	void Update () {
        this.GetComponent<Text>().text = "MyData.adTrigger: " +  MyData.adTrigger + "\nMyData.gameOver: " + MyData.gameOver;

    }
}
