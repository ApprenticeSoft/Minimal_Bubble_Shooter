using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadioButtonScript : MonoBehaviour {

    private void Start()
    {
        if(gameObject.GetComponentInChildren<Text>().text.Substring(0, 2).ToUpper() == MyData.langue)
            activer();
        else
            desactiver();
    }

    private void selectButton()
    {
        Button[] boutons = transform.parent.GetComponentsInChildren<Button>();
        
        foreach (Button bouton in boutons)
            desactiver(bouton);

        activer();

        MyData.langue = gameObject.GetComponentInChildren<Text>().text.Substring(0,2).ToUpper();
        PlayerPrefs.SetString("Langue", MyData.langue);
    }

    private void activer()
    {
        gameObject.GetComponentInChildren<Text>().color = Color.white;
        gameObject.GetComponentInChildren<Text>().fontSize = 25;
        gameObject.GetComponentInChildren<Outline>().enabled = true;
    }

    private void desactiver(Button bouton)
    {
        bouton.GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        bouton.GetComponentInChildren<Text>().fontSize = 20;
        bouton.GetComponentInChildren<Outline>().enabled = false;
    }

    private void desactiver()
    {
        gameObject.GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        gameObject.GetComponentInChildren<Text>().fontSize = 20;
        gameObject.GetComponentInChildren<Outline>().enabled = false;
    }
}
