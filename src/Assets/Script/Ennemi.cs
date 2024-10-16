using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ennemi : Entite
{
    public Qlearning qtable;
    public string action;
    public string derniere_action;
    public bool canDoAction = true;

    protected override void Start()
    {
        base.Start();       //appel de la fonction Start() de la superclass Entite
        
        degats = 30;
        defense = 2;
        heal = 10;

        qtable = new Qlearning("Classique");
    }

    protected override void Update()
    {
        base.Update();      //appel de la fonction Update() de la superclass Entite

        // Mettez à jour le paramètre "HP" dans l'Animator
        animator.SetFloat("HP", HP);

        if (canDoAction)        //propre a ennemi
        {
            action = qtable.ChoixActionUnique(HP, Stamina);
            derniere_action = action;
            canDoAction = false;
        }


    }

}
