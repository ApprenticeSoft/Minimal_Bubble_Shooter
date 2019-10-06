using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class xmlReader : MonoBehaviour
{
    // Glissez ici votre dictionnaire XML
    public TextAsset dictionary;
    private XmlDocument xmlDoc = new XmlDocument();
    private string langue = "EN";

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Langue"))
            PlayerPrefs.SetString("Langue", "EN");

        print("langue: " + PlayerPrefs.GetString("Langue"));
        MyData.langue = PlayerPrefs.GetString("Langue");
    }

    void Awake()
    {
        xmlDoc.LoadXml(dictionary.text);
        GameObject[] objs = GameObject.FindGameObjectsWithTag("xml");
        print("objs.Length: " + objs.Length);
        if (objs.Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        Reader();
    }

    private void OnLevelWasLoaded(int level)
    {
        Reader();
    }

    void Update()
    {
        if(langue != MyData.langue)
        {
            langue = MyData.langue;
            Reader();
        }
    }

    void Reader()
    {
        if (MyData.ecran == MyData.Ecrans.Accueil)
        {
            GameObject.Find("Btn_Start").GetComponentInChildren<Text>().text = getText("Start");
            GameObject.Find("Btn_Options").GetComponentInChildren<Text>().text = getText("Options");
            GameObject.Find("Btn_Quit").GetComponentInChildren<Text>().text = getText("Quit");
        }
        else if (MyData.ecran == MyData.Ecrans.Options)
        {
            GameObject.Find("Titre").GetComponent<Text>().text = getText("Options");
            GameObject.Find("Btn_Retour").GetComponentInChildren<Text>().text = getText("Return");
        }
        else if (MyData.ecran == MyData.Ecrans.Niveaux)
        {
            GameObject.Find("Titre").GetComponent<Text>().text = getText("LevelSelection");
            GameObject.Find("Btn_Retour").GetComponentInChildren<Text>().text = getText("Return");
        }
        else if (MyData.ecran == MyData.Ecrans.Jeu)
        {
            //Button[] boutons = GameObject.Find("Canvas").GetComponentsInChildren<Button>();

            foreach (Button bouton in GameObject.Find("Canvas").GetComponentsInChildren<Button>())
            {
                if (bouton.name == "Btn_Resume")
                    bouton.GetComponentInChildren<Text>().text = getText("Resume");
                else if (bouton.name == "Btn_Restart")
                    bouton.GetComponentInChildren<Text>().text = getText("Restart");
                else if(bouton.name == "Btn_Quit")
                    bouton.GetComponentInChildren<Text>().text = getText("Quit");
                else if(bouton.name == "Btn_Next")
                    bouton.GetComponentInChildren<Text>().text = getText("Next");
            }
            
            GameObject.Find("PanelGagne").GetComponentInChildren<Text>().text = getText("LevelCleared");
            GameObject.Find("PanelPause").GetComponentInChildren<Text>().text = getText("Pause");
            GameObject.Find("PanelPerd").GetComponentInChildren<Text>().text = getText("YouLost");
            GameObject.Find("PanelFinGroupe").GetComponentInChildren<Text>().text = getText("Group") + " " + MyData.groupeSelectione + "\n" + getText("Completed");
            GameObject.Find("PanelFin").GetComponentInChildren<Text>().text = getText("Congratulations") + "\n\n" + getText("ThankYou");
        }
    }
    

    /*
     * Méthode retournant la valeur d'un attribut "langue" d'un élément "message"
     * 
     */

    public string getText(string message) {
        string text = xmlDoc.GetElementsByTagName(message)[0].Attributes[MyData.langue].Value;
        return text;
    }
}