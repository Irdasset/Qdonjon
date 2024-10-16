using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aventure : MonoBehaviour
{
    public Personnage personnage;
    public Ennemi ennemi;
    public Spawner spawner;
    private int nbr_ennemis_tues;
    public Text display_info;
    public const float delai_entre_tours = 0.5f;
    

    void Start()
    {  
        nbr_ennemis_tues = 0;
        personnage = GameObject.FindWithTag("Player").GetComponent<Personnage>();
        spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        spawner.SpawnEnnemi();
        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        // display_info.text = "Nombre d'ennemis tués : " + nbr_ennemis_tues;
        // display_info.text = display_info.text + "Derniere action joueur : " + personnage.action;
        // display_info.text = display_info.text + "Derniere action ennemi : " + ennemi.action;

    }

    void Update()
    {
        
        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        if (personnage != null && personnage.HP <= 0)
        {
            Debug.Log("Game Over");
            Destroy(personnage.gameObject);
        }
        if (ennemi != null && ennemi.HP <= 0)
        {
            nbr_ennemis_tues ++;
            Debug.Log("Victory");
            Destroy(ennemi.gameObject);
            spawner.SpawnEnnemi();
            ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        }
        display_info.text = "Nombre d'ennemis tués : " + nbr_ennemis_tues + "\n";
        display_info.text = display_info.text + "Nombre de tours : " + personnage.nbr_tours + "\n";
        display_info.text = display_info.text + "Derniere action joueur : " + personnage.derniere_action + "\n";
        display_info.text = display_info.text + "Derniere action ennemi : " + ennemi.derniere_action;
    }
}
