using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Personnage : Entite
{

    public Ennemi ennemi;       //propre a personnage
    public Text display_info_personnage;        //propre a personnage
    public Qlearning qtable;

    public bool aventure;
    public string action;
    public bool canDoAction = true;
    public bool auto = false;
    public bool auto_aventure = false;
    public float epsylon;
    public float nbr_tours;
    public string derniere_action;
    private const float delai_attente = 0.01f; 


    protected override void Start()
    {
        base.Start();       //appel de la fonction Start() de la superclass Entite

        degats = 40;
        defense = 2;
        heal = 30;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        qtable = new Qlearning("Joueur");
        Debug.Log("Qtable Joueur : \n " + qtable.DisplayQtable(qtable.Qtable_));
        if (aventure)
        {
            qtable.editable = false;
            nbr_tours = 0;
        }

    }

    protected override void Update()
    {
        epsylon = qtable.epsylon_;
        base.Update();      //appel de la fonction Update() de la superclass Entite

        // Mettez à jour le paramètre "HP" dans l'Animator
        animator.SetFloat("HP", HP);
        if (qtable.editable)
        {    //afficher les informations du personnage
            display_info_personnage.text = "Vies utilisées: " + Vies_utilisees + "; ennemi: " + ennemi.Vies_utilisees;
        }
        //actualisation de l'ennemi
        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();

        if (canDoAction)
        {
            canDoAction = false;
            string proposition_action = qtable.ChoixActionUnique(HP, Stamina);
            //Changement de couleur du bouton correspondant à l'action choisie
            ChangeColorButton(proposition_action);


            if (auto)
            {
                action = proposition_action;
                //action = "Attaquer";      // pour tester le cas ou l'action est fixée
                canDoAction = false;
            }

            if (auto_aventure)
            {
                Auto_Aventure();
                // action = proposition_action;
                // //action = "Attaquer";      // pour tester le cas ou l'action est fixée
                // canDoAction = false;
            }
        }
    }

    void Auto_Aventure()
    {
        StartCoroutine(AttenteEntreActions());
    }

    IEnumerator AttenteEntreActions()
    {
        if (auto_aventure)
        {
            auto_aventure=false;
            nbr_tours += 1;
            // Récupérer l'action à effectuer
            string proposition_action = qtable.ChoixActionUnique(HP, Stamina);

            // Changer la couleur du bouton correspondant à l'action choisie
            ChangeColorButton(proposition_action);

            // Attendre une seconde avant de passer à l'action suivante
            yield return new WaitForSeconds(delai_attente);

            // Effectuer l'action
            action = proposition_action;

            // Attendre la fin du frame avant de continuer
            yield return new WaitForEndOfFrame();
            auto_aventure=true;
            derniere_action = action;
        }
    }
    private void OnApplicationQuit()
    {
        qtable.SaveQtable();
    }


    //fonction pour changé la couleur d'un bouton en fonction de l'action décidée par le Qlearning
    public void ChangeColorButton(string action)
    {
        if (qtable.editable)
        {    //remettre les couleurs des boutons à la normale
            GameObject.Find("Attaquer").GetComponent<Image>().color = Color.white;
            GameObject.Find("Defendre").GetComponent<Image>().color = Color.white;
            GameObject.Find("Heal").GetComponent<Image>().color = Color.white;
            GameObject.Find("Esquiver").GetComponent<Image>().color = Color.white;

            //changer la couleur du bouton correspondant à l'action choisie
            GameObject.Find(action).GetComponent<Image>().color = Color.green;
        }
    }

}