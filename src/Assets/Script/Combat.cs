using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Combat : MonoBehaviour
{
    private const int STAMINADECR = 20;
    private const int STAMINAINCR = 30;
    private Personnage perso;
    private Ennemi ennemi;

    private int NB_TOUR = 1000;
    private int tour = 1;

    void Start()
    {
        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        perso = GameObject.FindWithTag("Player").GetComponent<Personnage>();
    }


    void Update()
    {
        ennemi = GameObject.FindWithTag("Ennemi").GetComponent<Ennemi>();
        perso = GameObject.FindWithTag("Player").GetComponent<Personnage>();


        // On récupère les deux actions des deux agents
        string[] actions = new string[] { perso.action, ennemi.action };

        if (actions[0] != "" && actions[1] != "")
        {
            // On récupère les deux actions des deux agents
            string action_perso = perso.action;
            string action_ennemi = ennemi.action;

            //On lance les animations    
            perso.animator.SetTrigger(perso.action);
            ennemi.animator.SetTrigger(ennemi.action);

            //On sauvegarde les valeurs de HP et de Stamina avant le combat
            perso.Stamina_avant = perso.Stamina;
            perso.HP_avant = perso.HP;

            resultat(perso, ennemi, action_perso, action_ennemi);

            // On vide les actions des deux agents
            perso.action = "";
            ennemi.action = "";
            ennemi.canDoAction = true;
            perso.canDoAction = true;

            // On affiche la qtable tous les NB_TOUR tours
            if (tour % NB_TOUR == 0)
            {
                Debug.Log("Qtable Joueur - n°" + tour + "\n" + perso.qtable.DisplayQtable(perso.qtable.Qtable_));
            }
            tour++;

            //On vérifie que la stamina et les HP ne dépassent pas les valeurs max
            check_max_Stamina_Heal(perso, ennemi);

            //On met à jour les barres de vie et de stamina
            maj_bars(perso, ennemi);

            //On met à jour la qtable du Joueur si l'attribut editable est à true
            if (perso.qtable.editable)
            {
                //regarder les HP du joueur et s'ils sont inférieurs à 0, on met la pénalité
                if (perso.HP <= 0)
                {
                    perso.qtable.penalite = true;
                }
                if (ennemi.HP <= 0)
                {
                    perso.qtable.kill = true;
                }
                perso.qtable.MaJQtable(action_perso, perso.HP_avant, perso.HP, ennemi.HP_avant, ennemi.HP,
                                        perso.Stamina_avant, perso.Stamina, ennemi.Stamina_avant, ennemi.Stamina);
            }

            check_min_Heal(perso, ennemi);

        }


    }

    private void maj_bars(Personnage perso, Ennemi ennemi)
    {
        //On met à jour les barres de vie
        perso.healthBar.UpdateHealthBar(perso.HP, Entite.MAXHP);
        ennemi.healthBar.UpdateHealthBar(ennemi.HP, Entite.MAXHP);

        //On met à jour les barres de stamina
        perso.staminaBar.UpdateStaminaBar(perso.Stamina, Entite.MAXSTAMINA);
        ennemi.staminaBar.UpdateStaminaBar(ennemi.Stamina, Entite.MAXSTAMINA);
    }

    private void check_min_Heal(Personnage perso, Ennemi ennemi)
    {
        Debug.Log(perso.qtable.editable);
        if (perso.qtable.editable)
        {// On vérifie que les HP ne descendent pas en dessous de 0
            if (perso.HP <= 0)
            {
                perso.utiliserVie();
            }
            if (ennemi.HP <= 0)
            {
                ennemi.utiliserVie();
            }
        }
    }

    private void check_max_Stamina_Heal(Personnage perso, Ennemi ennemi)
    {
        // On vérifie que la stamina ne dépasse pas la valeur max
        if (perso.Stamina > Entite.MAXSTAMINA)
        {
            perso.Stamina = Entite.MAXSTAMINA;
        }
        if (ennemi.Stamina > Entite.MAXSTAMINA)
        {
            ennemi.Stamina = Entite.MAXSTAMINA;
        }

        // On vérifie que les HP ne dépassent pas la valeur max
        if (perso.HP > Entite.MAXHP)
        {
            perso.HP = Entite.MAXHP;
        }
        if (ennemi.HP > Entite.MAXHP)
        {
            ennemi.HP = Entite.MAXHP;
        }
    }


    private void resultat(Personnage perso, Ennemi ennemi, string action_perso, string action_ennemi)
    {
        if (action_perso == "Attaquer")
        {
            //D'abord on vérifie qu'il a assez de stamina pour Attaquerr
            if (perso.Stamina >= STAMINADECR)
            {
                //ensuite on décrémente la stamina
                perso.Stamina -= STAMINADECR;

                // On compare les deux actions
                if (action_ennemi == "Attaquer")
                {
                    // Les deux agents s'Attaquernt
                    if (ennemi.Stamina >= STAMINADECR)
                    {
                        perso.HP_avant = perso.HP;
                        perso.HP -= ennemi.degats;
                        ennemi.Stamina -= STAMINADECR;
                    }

                    ennemi.HP_avant = ennemi.HP;
                    ennemi.HP -= perso.degats;
                }
                else if (action_ennemi == "Defendre")
                {
                    // L'ennemi se défend
                    ennemi.HP_avant = ennemi.HP;
                    ennemi.HP -= perso.degats / ennemi.defense;
                    ennemi.Stamina += STAMINAINCR;
                }
                else if (action_ennemi == "Heal")
                {
                    // L'ennemi se soigne
                    ennemi.HP_avant = ennemi.HP;
                    ennemi.HP -= perso.degats;
                    ennemi.HP += ennemi.heal;
                    //////////////ennemi.Stamina += STAMINAINCR;
                }
                else if (action_ennemi == "Esquiver")
                {
                    // L'ennemi Esquiver avec un pourcentage de chance de 50%
                    int chance = UnityEngine.Random.Range(0, 100);
                    if (chance < 50)
                    {
                        ennemi.HP_avant = ennemi.HP;
                        ennemi.HP -= perso.degats;
                    }
                    ennemi.Stamina += STAMINAINCR;
                }
            }
            else
            {
                //Debug.Log("Stamina insuffisante pour Attaquerr");
                if (action_ennemi == "Attaquer")
                {
                    if (ennemi.Stamina >= STAMINADECR)
                    {
                        perso.HP_avant = perso.HP;
                        perso.HP -= ennemi.degats;
                        ennemi.Stamina -= STAMINADECR;
                    }
                }
                else if (action_ennemi == "Defendre")
                {
                    ennemi.Stamina += STAMINAINCR;
                }
                else if (action_ennemi == "Heal")
                {
                    // L'ennemi se soigne gratuitement
                    ennemi.HP_avant = ennemi.HP;
                    ennemi.HP += ennemi.heal;
                    //////////////ennemi.Stamina += STAMINAINCR;
                }
                else if (action_ennemi == "Esquiver")
                {
                    ennemi.Stamina += STAMINAINCR;
                }
            }



        }

        if (action_perso == "Defendre")
        {
            //D'abord on recharge la stamina vu qu'il Attaquer pas
            perso.Stamina += STAMINAINCR;

            // Le joueur se défend
            if (action_ennemi == "Attaquer")
            {
                if (ennemi.Stamina >= STAMINADECR)
                {
                    perso.HP_avant = perso.HP;
                    perso.HP -= ennemi.degats / perso.defense;
                    ennemi.Stamina -= STAMINADECR;
                }
            }
            else if (action_ennemi == "Defendre")
            {
                // Les deux agents se défendent
                ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Heal")
            {
                // L'ennemi se soigne
                ennemi.HP_avant = ennemi.HP;
                ennemi.HP += ennemi.heal;
                //////////////ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Esquiver")
            {
                ennemi.Stamina += STAMINAINCR;
            }
        }

        if (action_perso == "Heal")
        {
            //D'abord on recharge la stamina vu qu'il Attaquer pas
            //////////////perso.Stamina += STAMINAINCR;

            // Le joueur se soigne

            if (action_ennemi == "Attaquer")
            {
                if (ennemi.Stamina >= STAMINADECR)
                {
                    perso.HP_avant = perso.HP;
                    perso.HP -= ennemi.degats;
                    perso.HP += perso.heal;
                    ennemi.Stamina -= STAMINADECR;
                }

            }
            else if (action_ennemi == "Defendre")
            {
                perso.HP_avant = perso.HP;
                perso.HP += perso.heal;
                ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Heal")
            {
                // Les deux agents se soignent
                perso.HP_avant = perso.HP;
                perso.HP += perso.heal;
                ennemi.HP_avant = ennemi.HP;
                ennemi.HP += ennemi.heal;
                //////////////ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Esquiver")
            {
                perso.HP_avant = perso.HP;
                perso.HP += perso.heal;
                ennemi.Stamina += STAMINAINCR;
            }
        }

        if (action_perso == "Esquiver")
        {
            //D'abord on recharge la stamina vu qu'il Attaquer pas
            perso.Stamina += STAMINAINCR;

            // On compare les deux actions
            if (action_ennemi == "Attaquer")
            {
                if (ennemi.Stamina >= STAMINADECR)
                {
                    // Le perso Esquiver avec un pourcentage de chance de 50%
                    int chance = UnityEngine.Random.Range(0, 100);
                    if (chance < 50)
                    {
                        perso.HP_avant = perso.HP;
                        perso.HP -= ennemi.degats;
                        ennemi.Stamina -= STAMINADECR;
                    }
                }

            }
            else if (action_ennemi == "Defendre")
            {
                ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Heal")
            {
                // L'ennemi se soigne
                ennemi.HP_avant = ennemi.HP;
                ennemi.HP += ennemi.heal;
                //////////////ennemi.Stamina += STAMINAINCR;
            }
            else if (action_ennemi == "Esquiver")
            {
                ennemi.Stamina += STAMINAINCR;
            }
        }

    }

}