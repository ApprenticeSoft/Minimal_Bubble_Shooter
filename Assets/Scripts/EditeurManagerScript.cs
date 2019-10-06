using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class EditeurManagerScript : MonoBehaviour
{
    public int numéroGroupe, numéroNiveau, delaiNouvelleLigne, numéroLigne = 0;
    public int nbLigne, nbColonne;
    public GameObject[] ballPrefabs;
    public Sprite balleRouge, balleBleue, balleVerte, balleMauve, balleJaune, balleOrange, balleViolette, block, balleVide;
    [HideInInspector] public List<GameObject> balles, ballesDisponibles = new List<GameObject>();
    [HideInInspector] public float rayonBalle;

    //private GameObject balleSelectionnee;
    public string touchePressee { get; set; }

    void Start()
    {
        //TEST 14-06-2018
        MyData.nbColonne = nbColonne;

        //Création de la liste des balles disponibles
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            ballesDisponibles.Add(ballPrefabs[i]);
        }

        balles = new List<GameObject>();

        //rayonBalle = 0.05f * getCameraWidth();
        rayonBalle = 0.5f * (1 / (float)nbColonne) * getCameraWidth();

        //Création des balles
        for (int ligne = 0; ligne < nbLigne; ligne++)
        {
            for (int colonne = 0; colonne < nbColonne - ligne%2; colonne++)
            {
                GameObject balle = Instantiate( ballesDisponibles[0],
                                                new Vector2(-getCameraWidth() / 2 + ((1 + 2 * colonne) + (ligne % 2)) * rayonBalle,
                                                            getCameraHeight() / 2 - (rayonBalle*(1 + ligne * Mathf.Sqrt(3)))),
                                                            Quaternion.identity);
                balle.GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleVide;
                balle.transform.Find("Ombre").GetComponent<SpriteRenderer>().enabled = false;
                balle.tag = "Untagged";
                balles.Add(balle);
            }
        }
        foreach(GameObject balle in balles)
            balle.GetComponent<BallScript>().modeEdition = true;

        //Balle de référence
        GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleVide;
    }

    void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0))
        {
            touchePressee = Input.inputString;

            switch (/*GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().*/touchePressee)
            {
                case "1":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleRouge;
                    break;
                case "2":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleBleue;
                    break;
                case "3":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleVerte;
                    break;
                case "4":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleMauve;
                    break;
                case "5":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleViolette;
                    break;
                case "6":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleJaune;
                    break;
                case "7":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleOrange;
                    break;
                case "8":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = block;
                    break;
                case "9":
                    GameObject.Find("ReferenceBalle").GetComponent<SpriteRenderer>().sprite = balleVide;
                    break;
                case "w":
                    print("up!");
                    foreach (GameObject balle in balles)
                    {
                        balle.transform.position = new Vector3(balle.transform.position.x, balle.transform.position.y + rayonBalle * (Mathf.Sqrt(3)), 0);
                    }
                    numéroLigne++;
                    break;
                case "s":
                    print("down!");
                    foreach (GameObject balle in balles)
                    {
                        balle.transform.position = new Vector3(balle.transform.position.x, balle.transform.position.y - rayonBalle * (Mathf.Sqrt(3)), 0);
                    }
                    numéroLigne--;
                    break;
            }
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

    public void SaveLevel()
    {
        //StreamWriter writer = new StreamWriter("Assets/Niveaux/Groupe " + numéroGroupe + "/Niveau " + numéroNiveau, true);
        StreamWriter writer = new StreamWriter(Application.streamingAssetsPath + "/Niveaux/Groupe " + numéroGroupe + "/Niveau " + numéroNiveau, true);

        writer.WriteLine(nbColonne);
        writer.WriteLine(numéroLigne);
        writer.WriteLine(delaiNouvelleLigne);
        foreach (GameObject balle in balles)
        {
            if(balle.tag != "Untagged")
                writer.WriteLine(balle.tag + "," + (balle.GetComponent<BallScript>().getPosition().x / rayonBalle) + "," + ((getCameraHeight() / 2 - balle.GetComponent<BallScript>().getPosition().y) / rayonBalle));
        }
        writer.Close();
    }

    public void NouveauNiveau()
    {
        print("Nombre de balle: " + balles.Count);
        foreach (GameObject balle in balles)
            Destroy(balle);
        balles.Clear();
        print("Nombre de balle: " + balles.Count);

        rayonBalle = 0.5f * (1 / (float)nbColonne) * getCameraWidth();
        print("rayonBalle = " + rayonBalle);

        //Création des balles
        for (int ligne = 0; ligne < nbLigne; ligne++)
        {
            for (int colonne = 0; colonne < nbColonne - ligne % 2; colonne++)
            {
                GameObject balle = Instantiate(ballesDisponibles[0],
                                                new Vector2(-getCameraWidth() / 2 + ((1 + 2 * colonne) + (ligne % 2)) * rayonBalle,
                                                            getCameraHeight() / 2 - (rayonBalle * (1 + ligne * Mathf.Sqrt(3)))),
                                                            Quaternion.identity);
                print("*****************************************");
                print("Size: " + balle.GetComponent<BallScript>().getSize());
                balle.GetComponent<BallScript>().setSize(2 * rayonBalle);
                print("Size: " + balle.GetComponent<BallScript>().getSize());
                print("******************FIN********************");
                balle.GetComponent<SpriteRenderer>().sprite = GameObject.Find("EditeurManager").GetComponent<EditeurManagerScript>().balleVide;
                balle.transform.Find("Ombre").GetComponent<SpriteRenderer>().enabled = false;
                balle.tag = "Untagged";
                balles.Add(balle);
            }
        }
        foreach (GameObject balle in balles)
            balle.GetComponent<BallScript>().modeEdition = true;

        //SceneManager.LoadScene("EditeurScene");
    }
}
