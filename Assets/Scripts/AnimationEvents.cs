using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Event()
    {
        if (MyData.ecran == MyData.Ecrans.Niveaux)
            SceneManager.LoadScene("NiveauxScene");
        else if (MyData.ecran == MyData.Ecrans.Options)
            SceneManager.LoadScene("OptionsScene");
        else if (MyData.ecran == MyData.Ecrans.Quitter)
        {
            Application.Quit();
            print("je veux quitter !!!!!!!!!!!");
            //if(UnityEditor.EditorApplication.isPlaying)
            //    UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
