using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Bouton : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button
        Btn_attaque,
        Btn_defense,
        Btn_esquiver,
        Btn_Heal,
        Btn_Aventure,
        Btn_Entrainement,
        Btn_Auto,
        Btn_Auto_Aventure;

    private Personnage personnage;
    private GameObject personnageGO;

    void Start()
    {
        personnageGO = GameObject.FindWithTag("Player");
        personnage = personnageGO.GetComponent<Personnage>();


        Btn_attaque.onClick.AddListener(Attaquer);
        Btn_defense.onClick.AddListener(Defense);
        Btn_esquiver.onClick.AddListener(Esquiver);
        Btn_Heal.onClick.AddListener(Heal);

        Btn_Entrainement.onClick.AddListener(Entrainement);
        Btn_Aventure.onClick.AddListener(Aventure);
        Btn_Auto.onClick.AddListener(Auto);
        Btn_Auto_Aventure.onClick.AddListener(Auto_Aventure);
    }

    void Aventure()
    {
        //lance la scene "Aventure"
        Debug.Log("BIENVENUE DANS L'AVENTURE ! BONNE CHANCE !");
        SceneManager.LoadScene("Aventure");
        personnage.qtable.editable = false;
        personnage.display_info_personnage.text = "";
    }

    void Entrainement()
    {
        //lance la scene "Aventure"
        Debug.Log("BIENVENUE DANS L'ENTRAINEMENT !");
        SceneManager.LoadScene("Entrainement");
        personnage.qtable.editable = true;
        personnage.display_info_personnage.text = "";
    }

    void Auto()
    {
        personnage.canDoAction = true;
        if (personnage.auto)
        {
            personnage.auto = false;
            //rendre le bouton "Auto" couleur #FF8F8F
            Btn_Auto.GetComponent<Image>().color = new Color(1, 0.56f, 0.56f);
        }
        else
        {
            personnage.auto = true;
            //rendre le bouton "Auto" couleur #8FFF8F
            Btn_Auto.GetComponent<Image>().color = new Color(0.56f, 1, 0.56f);
        }
    }

    void Auto_Aventure()
    {
        personnage.canDoAction = true;
        if (personnage.auto_aventure)
        {
            personnage.auto_aventure = false;
            //rendre le bouton "Auto" couleur #FF8F8F
            Btn_Auto_Aventure.GetComponent<Image>().color = new Color(1, 0.56f, 0.56f);
        }
        else
        {
            personnage.auto_aventure = true;
            //rendre le bouton "Auto" couleur #8FFF8F
            Btn_Auto_Aventure.GetComponent<Image>().color = new Color(0.56f, 1, 0.56f);
        }
    }

    void Attaquer()
    {
        personnage.action = "Attaquer";
    }

    void Defense()
    {
        personnage.action = "Defendre";
    }

    void Esquiver()
    {
        personnage.action = "Esquiver";
    }

    void Heal()
    {
        personnage.action = "Heal";
    }


    // void TaskWithParameters(string message)
    // {
    //     //Output this to console when the Button2 is clicked
    //     Debug.Log(message);
    // }

    // void ButtonClicked(int buttonNo)
    // {
    //     //Output this to console when the Button3 is clicked
    //     Debug.Log("Button clicked = " + buttonNo);
    // }
}