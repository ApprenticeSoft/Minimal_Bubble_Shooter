using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuNiveauxScript : MonoBehaviour {
    
    [SerializeField] GameObject boutonPrefab;
    [SerializeField] Transform menuCanevasNiveaux, menuCanevasGroupe;
    [SerializeField] Animator animatorNiveau, animatorGroupe;
    private List<GameObject> boutonsNiveau = new List<GameObject>();

    //Test animation boutons niveau
    private int indiceBouton = 0;
    private float compteur = 0;
    //private Vector3 angleVector = new Vector3(90, -73, -8);
    private List<GameObject> boutons = new List<GameObject>();


    // Use this for initialization
    void Start () {
        print("Groupe = " + PlayerPrefs.GetInt("Groupe"));
        print("Niveau = " + PlayerPrefs.GetInt("Niveau"));
        //Création des boutons groupe
        for (int i = 1; i < MyData.NbGroupe + 1; i++)
        {
            GameObject boutonGroupe = (GameObject)Instantiate(boutonPrefab);
            boutonGroupe.transform.SetParent(menuCanevasGroupe, false);
            boutonGroupe.GetComponentInChildren<Text>().text = i.ToString();
            boutonGroupe.GetComponent<Button>().onClick.AddListener(
                                                                    () => { SelectGroupe(int.Parse(boutonGroupe.GetComponentInChildren<Text>().text)); }
                                                                   );
            
            if (i > PlayerPrefs.GetInt("Groupe"))
                boutonGroupe.GetComponent<Button>().interactable = false;

            //boutonGroupe.transform.rotation = Quaternion.Euler(angleVector);

            boutons.Add(boutonGroupe);
        }

        //Création des boutons niveau
        for (int i = 1; i < MyData.NbNiveauxDansGroupe + 1; i++)
        {
            GameObject boutonNiveau = (GameObject)Instantiate(boutonPrefab);
            boutonNiveau.transform.SetParent(menuCanevasNiveaux, false);
            boutonNiveau.GetComponentInChildren<Text>().text = i.ToString();
            boutonNiveau.GetComponent<Button>().onClick.AddListener(
                                                                    () => { StartLevel(int.Parse(boutonNiveau.GetComponentInChildren<Text>().text)); }
                                                                   );

            if (MyData.groupeSelectione == PlayerPrefs.GetInt("Groupe"))
                if (i > PlayerPrefs.GetInt("Niveau"))
                    boutonNiveau.GetComponent<Button>().interactable = false;

            //boutonNiveau.transform.rotation = Quaternion.Euler(angleVector);

            boutons.Add(boutonNiveau);
            boutonsNiveau.Add(boutonNiveau);
        }

        animatorNiveau.SetBool("isHidden", true);
    }
	
	// Update is called once per frame
	void Update () {
        activiteBoutonsNiveaux();
    }

    public void Retour()
    {
        if (animatorGroupe.GetBool("isHidden"))
        {
            animatorNiveau.SetBool("isHidden", true);
            animatorGroupe.SetBool("isHidden", false);
            //ModifieTitre("CHOISISSEZ UN GROUPE");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            MyData.ecran = MyData.Ecrans.Accueil;
        }
    }

    public void SelectGroupe(int groupe)
    {
        MyData.groupeSelectione = groupe;
        animatorNiveau.SetBool("isHidden", false);
        animatorGroupe.SetBool("isHidden", true);
        //ModifieTitre("CHOISISSEZ UN NIVEAU");

        if (MyData.groupeSelectione == PlayerPrefs.GetInt("Groupe"))
        {
            for (int i = 0; i < boutonsNiveau.Count; i++)
            {
                if (i + 1 > PlayerPrefs.GetInt("Niveau"))
                    boutonsNiveau[i].GetComponent<Button>().interactable = false;
                else
                    boutonsNiveau[i].GetComponent<Button>().interactable = true;
            }
        }
        else
            for (int i = 0; i < boutonsNiveau.Count; i++)
                boutonsNiveau[i].GetComponent<Button>().interactable = true;
    }

    public void StartLevel(int level)
    {
        MyData.niveauSelectione = level;
        MyData.ecran = MyData.Ecrans.Jeu;
        SceneManager.LoadScene("GameScene");
    }

    public void activiteBoutonsNiveaux()
    {
        if (indiceBouton < boutons.Count)
        {
            compteur += Time.deltaTime;
            if (compteur > 0.03f)
            {
                boutons[indiceBouton].tag = "Bouton";
                compteur = 0;
                indiceBouton++;
            }
        }

        for (int i = 0; i < boutons.Count; i++)
        {
            if (boutons[i].tag.Equals("Bouton"))
                boutons[i].transform.rotation = Quaternion.Lerp(boutons[i].transform.rotation, Quaternion.Euler(new Vector3(0, 0, 0)), 0.1f);
        }
    }

    /*
    public void ModifieTitre(string nouveauTitre)
    {
        GameObject objetTitre = GameObject.Find("Titre_Ecran");
        objetTitre.GetComponent<Text>().text = nouveauTitre;
    }
    */
}
