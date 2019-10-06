using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour {

    [SerializeField] Animator animatorPause, animatorUI;

    private void Start()
    {
        ChargeDonnees();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("GameScene"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!MyData.pause)
                    Pause();
                else
                    Resume();
            }
        }
    }

    private void ChargeDonnees()
    {
        if (PlayerPrefs.HasKey("Niveau"))
        {
            MyData.numeroNiveau = PlayerPrefs.GetInt("Niveau");
            print("La valeur Niveau existe: " + MyData.numeroNiveau);
        }
        else
        {
            PlayerPrefs.SetInt("Niveau", 1);
            print("La valeur Niveau n'existe pas encore");
        }

        if (PlayerPrefs.HasKey("Groupe"))
        {
            MyData.numeroNiveau = PlayerPrefs.GetInt("Groupe");
            print("La valeur Groupe existe: " + MyData.numeroNiveau);
        }
        else
        {
            PlayerPrefs.SetInt("Groupe", 1);
            print("La valeur Groupe n'existe pas encore");
        }

        /*
        print("-------------------------Test niveaux");
        PlayerPrefs.SetInt("Groupe", 2);
        PlayerPrefs.SetInt("Niveau", 15);
        print("Groupe: " + PlayerPrefs.GetInt("Groupe") + " || Niveau: " + PlayerPrefs.GetInt("Niveau"));
        */
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        MyData.ecran = MyData.Ecrans.Jeu;
    }

    public void StartEditeur()
    {
        SceneManager.LoadScene("EditeurScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        MyData.ecran = MyData.Ecrans.Accueil;
    }

    public void SelectionNiveaux()
    {
        animatorUI.SetBool("animMenu", true);
        MyData.ecran = MyData.Ecrans.Niveaux;
    }

    public void Options()
    {
        animatorUI.SetBool("animMenu", true);
        MyData.ecran = MyData.Ecrans.Options;

    }

    public void BoutonFlip()
    {
        print("Bouton flip");
        animatorUI.SetBool("Flip", true);
    }

    public void Test()
    {
        print("Test du bouton!!!!");
        animatorPause.SetBool("Slide", true);
    }

    public void Quit()
    {
        animatorUI.SetBool("animMenu", true);
        MyData.ecran = MyData.Ecrans.Quitter;
    }
    /*
    public void StartLevel(string nom)
    {
        print("Start");
        print("Nom " + nom);
    }
    */
    public void Pause()
    {
        animatorPause.SetBool("isHidden", false);
        MyData.pause = true;
        //Time.timeScale = 0.1f;
    }

    public void Resume()
    {
        animatorPause.SetBool("isHidden", true);
        MyData.pause = false;
        //Time.timeScale = 1;
    }

    public void Facebook()
    {
        if (Application.platform == RuntimePlatform.Android)
            Application.OpenURL("https://m.facebook.com/profile.php?id=157533514581396");
        else
            Application.OpenURL("https://facebook.com/profile.php?id=157533514581396");
    }

    public void Twitter()
    {
        Application.OpenURL("https://twitter.com/ApprenticeSoft");
    }

    public void Rate()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.bubble.shooter.android");
    }

    private void OnApplicationQuit()
    {
        print("Quit");
    }


}
