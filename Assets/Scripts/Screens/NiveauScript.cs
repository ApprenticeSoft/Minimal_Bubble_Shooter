using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NiveauScript : MonoBehaviour {

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
    void Start()
    {
        //PlayerPrefs.SetInt("Groupe", 4);
        //PlayerPrefs.SetInt("Niveau", 18);


        // On désactive les boutons de groupe pas encore débloqués
        foreach (Transform child in GameObject.Find("Canvas_Groupe").transform)     // On cherche tout les objets enfant du canevas Groupe
        {
            foreach (Transform child2 in child)
            {
                if (int.Parse(child.name.Split(' ')[1]) > PlayerPrefs.GetInt("Groupe")) // On cherche tout les objets enfant du canevas Groupe
                {
                    child2.GetComponent<Button>().interactable = false;
                    child2.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 0);
                    child2.GetComponentInChildren<Text>().color = new Color(0.4f, 0.4f, 0.4f, 0);
                }
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        //activiteBoutonsNiveaux();
    }

    public void Retour()
    {
        if (animatorGroupe.GetBool("isHidden"))
        {
            /*
            animatorNiveau.SetBool("isHidden", true);
            animatorGroupe.SetBool("isHidden", false);
            */
            // Solution bien sale
            SceneManager.LoadScene("NiveauxScene");
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
            DesactiveBoutonNiveau();
        }
        else
            ActiveBoutonNiveau();
    }

    public void DesactiveBoutonNiveau()
    {
        // On désactive les boutons de niveau pas encore débloqués
        foreach (Transform child in GameObject.Find("Canvas_Niveaux").transform)     // On cherche tout les objets enfant du canevas Niveau
        {
            foreach (Transform child2 in child)
            {
                if (int.Parse(child.name.Split(' ')[1]) > PlayerPrefs.GetInt("Niveau")) // On cherche tout les objets enfant du canevas Niveau
                {
                    child2.GetComponent<Button>().interactable = false;
                    child2.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 0);
                    child2.GetComponentInChildren<Text>().color = new Color(0.4f, 0.4f, 0.4f, 0);
                }
                else
                {
                    child2.GetComponent<Button>().interactable = true;
                    child2.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    child2.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    public void ActiveBoutonNiveau()
    {
        // On active tous les boutons de niveau pas encore débloqués
        foreach (Transform child in GameObject.Find("Canvas_Niveaux").transform)     // On cherche tout les objets enfant du canevas Niveau
        {
            foreach (Transform child2 in child)
            {
                child2.GetComponent<Button>().interactable = true;
                child2.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                child2.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0);
            }
        }
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
}
