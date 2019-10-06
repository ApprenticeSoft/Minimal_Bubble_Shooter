using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Globalization;

public class GameManagerScript : MonoBehaviour {

    public GameObject[] ballPrefabs;
    public GameObject block, wall;
    [HideInInspector] public List<GameObject> balles, blocks, ballesIdentiques = new List<GameObject>(), ballesDisponibles = new List<GameObject>();
    [HideInInspector] public bool destruction = false, detection = false, deuxiemeDetection = false, utiliseBlocks = false;
    public int nbLigne = 1, nbColonne = 1, delaiNouvelleLigne = 4;
    public float rayonBalle, delaiDestruction = 0, delaiDetection = 0.9f;
    private List<GameObject> temp = new List<GameObject>();
    private int nbContact = 0;
    public AudioSource sonGagne;

    private string[] ballesDansLeNiveau;

    [SerializeField] Animator animatorGagne, animatorFinGroupe, animatorFinJeu;

    private TextAsset textAsset;

    void Start () {
        //PlayerPrefs.SetInt("Groupe", 1);
        //PlayerPrefs.SetInt("Niveau", 15);
        MyData.gameOver = false;
        MyData.pause = false;
        MyData.nouvelleLigne = 0;
        //rayonBalle = 0.05f * getCameraWidth();
        rayonBalle = 0.5f * (1 / (float)MyData.nbColonne) * getCameraWidth();

        //Création des paroies
        GameObject murDroite = Instantiate( wall,
                                            new Vector2(0, 0),
                                            Quaternion.identity);
        murDroite.GetComponent<WallScript>().setSize(getCameraWidth()/10, 1.2f * getCameraHeight());
        murDroite.GetComponent<WallScript>().setPosition(getCameraWidth()/2 + murDroite.GetComponent<WallScript>().getWidth()/2, 0);

        GameObject murGauche = Instantiate( wall,
                                            new Vector2(0, 0),
                                            Quaternion.identity);
        murGauche.GetComponent<WallScript>().setSize(getCameraWidth() / 10, 1.2f * getCameraHeight());
        murGauche.GetComponent<WallScript>().setPosition(-getCameraWidth() / 2 - murDroite.GetComponent<WallScript>().getWidth() / 2, 0);

        GameObject murHaut = Instantiate(   wall,
                                            new Vector2(0, 0),
                                            Quaternion.identity);
        murHaut.GetComponent<WallScript>().setSize(getCameraWidth(), getCameraWidth()/10);
        murHaut.GetComponent<WallScript>().setPosition(0, getCameraHeight()/2 + murHaut.GetComponent<WallScript>().getHeight()/2);
        murHaut.tag = "Plafond";

        GameObject murBas = Instantiate(wall,
                                        new Vector2(0, 0),
                                        Quaternion.identity);
        murBas.GetComponent<WallScript>().setSize(getCameraWidth(), getCameraWidth() / 10);
        murBas.GetComponent<WallScript>().setPosition(0, -0.6f*getCameraHeight());
        murBas.tag = "Destructor";
        murBas.layer = 0;

        //Création de la liste des balles disponibles
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            ballesDisponibles.Add(ballPrefabs[i]);
        }

        balles = new List<GameObject>();
        blocks = new List<GameObject>();

        //Création des balles
        LoadLevel();

        CompteBalles();
        GameObject.Find("Arrow").GetComponent<ArrowScript>().NouvelleBalle();
    }
	
	void Update () {
        if (!MyData.pause)
        {
            if (destruction)
            {
                detruitBallesIdentiques();
            }
            else if (detection)
            {
                detruitObjetNull(balles);
                markBallesIsolées(balles);
                dropBallesIsolees(balles);
                detruitObjetNull(balles);
                detection = false;
                CompteBalles();
                GameObject.Find("Arrow").GetComponent<ArrowScript>().recharge = true;
            }
            
            if (deuxiemeDetection)
            {
                delaiDetection -= Time.deltaTime;

                if (delaiDetection < 0)
                {
                    detruitObjetNull(balles);
                    markBallesIsolées(balles);
                    dropBallesIsolees(balles);
                    detruitObjetNull(balles);
                    detection = false;
                    CompteBalles();
                    GameObject.Find("Arrow").GetComponent<ArrowScript>().recharge = true;
                    deuxiemeDetection = false;
                    print("Deuxième détection effectuée");
                }
            }
            
            // Niveau gagné
            if (balles.Count - blocks.Count < 1)
                FinNiveau();
        }
        
    }

    public float getCameraWidth()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)));
    }

    public float getCameraHeight()
    {
        return Vector3.Distance(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0.0f, 0f)), Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)));
    }

    private void detruitBallesIdentiques()
    {
        delaiDestruction -= Time.deltaTime;

        if (ballesIdentiques.Count > 0)
        {
            if (delaiDestruction < 0)
            {
                balles.RemoveAt(balles.IndexOf(ballesIdentiques[ballesIdentiques.Count - 1]));
                ballesIdentiques[ballesIdentiques.Count - 1].GetComponent<BallScript>().destroy = true; 
                ballesIdentiques[ballesIdentiques.Count - 1].GetComponent<BallScript>().sonDisparait.Play();
                ballesIdentiques.RemoveAt(ballesIdentiques.Count - 1);
                delaiDestruction = 0.02f;
            }
        }
        else
        {
            delaiDestruction = 0;
            destruction = false;
            ballesIdentiques.Clear();

            detection = true;
            deuxiemeDetection = true;
            delaiDetection = 0.9f;
            print("Deuxième détection activée");
        }

        detruitObjetNull(balles);
    }

    public void markBallesIsolées(List<GameObject> balles)
    {
        SortList(balles);
        
        for (int i = 0; i < balles.Count; i++)
            if (balles[i].transform.position.y > (0.5f * getCameraHeight() - 1.1f * rayonBalle))
                balles[i].GetComponent<BallScript>().isolated = false;
            else
                balles[i].GetComponent<BallScript>().isolated = true;

        for (int i = 0; i < balles.Count; i++)
            if (balles[i].GetComponent<BallScript>().isolated == false)
                voisinIsolée(balles[i], balles);
    }

    private void voisinIsolée(GameObject balle, List<GameObject> balles)
    {
        for (int j = 0; j < balles.Count; j++)
        {
            if (Vector3.Distance(balle.transform.position, balles[j].transform.position) < 2.1f * rayonBalle)
                if (balles[j].GetComponent<BallScript>().isolated == true)
                {
                    balles[j].GetComponent<BallScript>().isolated = false;
                    voisinIsolée(balles[j], balles);
                }
        }
    }

    private void dropBallesIsolees(List<GameObject> balles)
    {
        for (int i = balles.Count - 1; i >= 0; i--)
        {
            if (balles[i].GetComponent<BallScript>().isolated == true)
            {
                balles[i].GetComponent<Rigidbody2D>().gravityScale = 1;
                balles[i].GetComponent<BallScript>().move = false;
                balles[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                balles[i].layer = 9;

                if (balles[i].tag.Equals("Block"))
                    blocks.Remove(balles[i]);

                balles.Remove(balles[i]);
            }
            
        }

        detruitObjetNull(balles);
    }

    public List<GameObject> RemoveDuplicates(List<GameObject> liste)
    {
        for (int i = liste.Count-1; i > -1; i--)
        {
            for(int j = i-1; j > -1; j--)
            {
                if (liste[i] == liste[j])
                {
                    liste.RemoveAt(i);
                    break;
                }
            }
        }

        return liste;
    }

    public void CompteBalles()
    {
        int nbBalleRouge = 0;
        int nbBalleVerte = 0;
        int nbBalleMauve = 0;
        int nbBalleBleue = 0;
        int nbBalleJaune = 0;
        int nbBalleOrange = 0;
        int nbBalleViolette = 0;

        foreach(GameObject balle in balles)
        {
            if (balle.tag == "BalleRouge")
                nbBalleRouge++;
            else if (balle.tag == "BalleBleue")
                nbBalleBleue++;
            else if (balle.tag == "BalleVerte")
                nbBalleVerte++;
            else if (balle.tag == "BalleMauve")
                nbBalleMauve++;
            else if (balle.tag == "BalleJaune")
                nbBalleJaune++;
            else if (balle.tag == "BalleOrange")
                nbBalleOrange++;
            else if (balle.tag == "BalleViolette")
                nbBalleViolette++;
        }

        if (nbBalleBleue == 0)
        {
            for(int i = ballesDisponibles.Count - 1; i > - 1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleBleue")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleRouge == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleRouge")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleVerte == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleVerte")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleMauve == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleMauve")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleJaune == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleJaune")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleOrange == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleOrange")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
        if (nbBalleViolette == 0)
        {
            for (int i = ballesDisponibles.Count - 1; i > -1; i--)
            {
                if (ballesDisponibles[i].tag == "BalleViolette")
                    ballesDisponibles.Remove(ballesDisponibles[i]);
            }
        }
    }
    
    private void detruitObjetNull(List<GameObject> listObjet)
    {
        for (int i = listObjet.Count - 1; i > -1; i--)
        {
            if (listObjet[i] == null)
            {
                listObjet.RemoveAt(i);
                print("*************************Balle null!!!!");
            }
        }
    }
    
    public void FinNiveau()
    {
        MyData.pause = true;
        sonGagne.Play();

        //Fin de niveau
        if (MyData.niveauSelectione < MyData.NbNiveauxDansGroupe)
        {
            animatorGagne.SetBool("isHidden", false);

            MyData.niveauSelectione++;
            if (MyData.niveauSelectione > PlayerPrefs.GetInt("Niveau"))
                MyData.SaveLevel();
        }
        else if (MyData.niveauSelectione == MyData.NbNiveauxDansGroupe)
        {
            //Fin de groupe
            if (MyData.groupeSelectione < MyData.NbGroupe)
            {
                //animatorGagne.SetBool("isHidden", false);
                animatorFinGroupe.SetBool("isHidden", false);

                MyData.niveauSelectione = 1;
                MyData.groupeSelectione++;
                if (MyData.groupeSelectione > PlayerPrefs.GetInt("Groupe"))
                    MyData.SaveLevel();
            }
            //Fin de jeu
            else
            {
                animatorFinJeu.SetBool("isHidden", false);
                //SceneManager.LoadScene("MainMenu");
            }
        }

        // Admob trigger
        MyData.gameOver = true;
        MyData.adTrigger++;
    }

    public void NiveauSuivant()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Perdu()
    {

    }

    public void LoadLevel()
    {
        print("Chargement du niveau n°" + MyData.numeroNiveau);

        //Emplacement du niveau
        string path = Application.streamingAssetsPath + "/Niveaux/Groupe " + MyData.groupeSelectione + "/Niveau " + MyData.niveauSelectione;
        print("path: " + path);

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(path);
            while (!reader.isDone) { }

            ballesDansLeNiveau = reader.text.Split('\n');
        }
        else
        {
            print("Version ordinateur !");
            ballesDansLeNiveau = File.ReadAllLines(path);
        }

        MyData.nbColonne = int.Parse(ballesDansLeNiveau[0]);
        rayonBalle = 0.5f * (1 / (float)MyData.nbColonne) * getCameraWidth();

        MyData.nouvelleLigne = int.Parse(ballesDansLeNiveau[1]);
        delaiNouvelleLigne = int.Parse(ballesDansLeNiveau[2]);

        for (int i = 3; i < ballesDansLeNiveau.Length; i++)
        {
            string[] données = ballesDansLeNiveau[i].Split(',');
            if(données.Length == 3)
            {
                int couleurBalle;

                switch (données[0])
                {
                    case "BalleRouge":
                        couleurBalle = 0;
                        break;
                    case "BalleBleue":
                        couleurBalle = 1;
                        break;
                    case "BalleVerte":
                        couleurBalle = 2;
                        break;
                    case "BalleMauve":
                        couleurBalle = 3;
                        break;
                    case "BalleJaune":
                        couleurBalle = 4;
                        break;
                    case "BalleOrange":
                        couleurBalle = 5;
                        break;
                    case "BalleViolette":
                        couleurBalle = 6;
                        break;
                    default:
                        couleurBalle = 0;
                        break;
                }

                float positionX = float.Parse(	données[1],
												System.Globalization.NumberStyles.Any,
												CultureInfo.InvariantCulture);
				float positionY = float.Parse(	données[2],
												System.Globalization.NumberStyles.Any,
												CultureInfo.InvariantCulture);

                if (!données[0].Equals("Block"))
                {
                    GameObject balle = Instantiate( ballesDisponibles[couleurBalle],
                                                    new Vector2(positionX * rayonBalle,
                                                                getCameraHeight() / 2 - positionY * rayonBalle),
                                                                Quaternion.identity);

                    balles.Add(balle);
                }
                else
                {
                    GameObject balle = Instantiate( block,
                                                    new Vector2(positionX * rayonBalle,
                                                                getCameraHeight() / 2 - positionY * rayonBalle),
                                                                Quaternion.identity);
                    balles.Add(balle);
                    blocks.Add(balle);
                    utiliseBlocks = true;
                }
            }
        }
    }

    public List<GameObject> SortList(List<GameObject> list)
    {
        int lenD = list.Count;
        int j = 0;
        GameObject tmp;
        for (int i = 0; i < lenD; i++)
        {
            j = i;
            for (int k = i; k < lenD; k++)
            {
                if (list[j].transform.position.y < list[k].transform.position.y)
                    j = k;
                else if (list[j].transform.position.y == list[k].transform.position.y)
                    if (list[j].transform.position.x < list[k].transform.position.x)
                        j = k;
            }
            tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        return list;
    }
}
