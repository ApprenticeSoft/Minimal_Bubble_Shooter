
using UnityEngine;

public class MyData {

    private static int nbNiveauxDansGroupe = 20;
    public static int NbNiveauxDansGroupe
    {
        get
        {
            return nbNiveauxDansGroupe;
        }
    }

    private static int nbGroupe = 4;
    public static int NbGroupe
    {
        get
        {
            return nbGroupe;
        }
    }

    //Ancienne valeure, à supprimer
    public static int numeroNiveau = 1;
    //test
    public static int niveauSelectione = 1;
    public static int groupeSelectione = 1;
    // Compteur publicité
    public static int adTrigger = 1;
    public static bool gameOver = false;

    public static bool pause = false;
    public static int nouvelleLigne = 0;
    public static int nbColonne = 10;

    public static string langue = "EN";

    public static void SaveLevel()
    {
        PlayerPrefs.SetInt("Niveau", niveauSelectione);
        PlayerPrefs.SetInt("Groupe", groupeSelectione);
    }

    public static void LoadLevel()
    {
        PlayerPrefs.GetInt("Niveau");
        PlayerPrefs.GetInt("Groupe");
    }

    public enum Ecrans
    {
        Niveaux,
        Options,
        Jeu,
        Accueil,
        Quitter
    }

    public static Ecrans ecran = Ecrans.Accueil;
}
