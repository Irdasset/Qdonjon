using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor.EditorTools;
using Unity.VisualScripting;
using System.Net.Mime;
using System.Linq;

public class Qlearning
{
    // ----------------------------------------------
    // ||    Variables du jeu et du Q-learning     ||
    // ----------------------------------------------

    //Hyperparam�tres
    private float alpha_ = 0.6f;                // Taux d'apprentissage : 0 = pas d'apprentissage, 1 = apprentissage total
    private float gamma_ = 0.2f;               // Taux d'actualisation : 0 = l'agent regarde la r�compense la plus proche, 1 = regarde les r�compenses futures
    public float epsylon_ = 1f;              // Taux d'exploration : 0 = pas d'exploration, 1 = exploration totale
    private float epsylon_delta = 0.0001f;      // Diminution du taux d'exploration � chaque action

    //private float tunage = 0.9f;       //Tunage de la récompense

    //Variables du jeu
    private float recompense_ = 0f;
    public string etat_actuel_ = "";
    public string action_ = "";
    private string[] actions = { "Attaquer", "Defendre", "Esquiver", "Heal" };

    // Etat + stamina de l'ennemie + stamina du joueur 
    private List<string> etats = new List<string>();

    //Condition Qtable éditable   
    public bool editable = true;

    //Condition penalité
    public bool penalite = false;
    private const float PENALITE = 100f;

    public bool kill = false;

    private const float REWARD = 1000f;


    //Qtable

    public Dictionary<Tuple<string, string>, float> Qtable_ = new Dictionary<Tuple<string, string>, float>();

    // ----------------------------------------------
    // ||    Fonctions pour le début                ||
    // ----------------------------------------------


    public Qlearning(string path)
    {
        // On récupère tous les fichiers dans le dossier Qtables avec l'extension ".txt"
        string[] qtableFiles = System.IO.Directory.GetFiles("Assets/Qtables/" + path, "*.txt");

        // Si le dossier Qtables contient des fichiers ".txt"
        if (qtableFiles.Length > 0 && (path == "Joueur" || path == "Classique"))
        {
            // On trie les fichiers par date de modification (du plus récent au plus ancien)
            Array.Sort(qtableFiles, (a, b) => System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a)));

            // On récupère le fichier le plus récent
            string latestQtableFile = qtableFiles[0];

            // On récupère la Q-table dans le fichier le plus récent
            string[] qtableString = System.IO.File.ReadAllLines(latestQtableFile);

            // On récupère les états, les actions et les récompenses dans la Q-table
            foreach (string line in qtableString)
            {
                // On récupère les valeurs de la ligne
                string[] values = line.Split(',');
                string state = values[0];
                string action = values[1];
                float qvalue = float.Parse(values[2]);

                // On ajoute les valeurs dans la Q-table
                Qtable_.Add(new Tuple<string, string>(state, action), qvalue);
            }
        }
        else if (path.Contains("Ennemis_Aventure"))
        {
            //prendre une qtable aléatoire qui est au format .txt
            string[] qtableFiles_ = System.IO.Directory.GetFiles("Assets/Qtables/Ennemis_Aventure", "*.txt");
            string qtable = qtableFiles_[UnityEngine.Random.Range(0, qtableFiles_.Length)];
            string[] qtableString = System.IO.File.ReadAllLines(qtable);

            // On récupère les états, les actions et les récompenses dans la Q-table
            foreach (string line in qtableString)
            {
                // On récupère les valeurs de la ligne
                string[] values = line.Split(',');
                string state = values[0];
                string action = values[1];
                float qvalue = float.Parse(values[2]);

                // On ajoute les valeurs dans la Q-table
                Qtable_.Add(new Tuple<string, string>(state, action), qvalue);
            }

        }
        else
        {
            Debug.Log("Pas de Qtable trouvée, initialisation de la Qtable");
            InitialisationQTable();
        }
    }


    //Fonction pour initialiser la Q-table :
    // La Q-table est de la forme : <etat,action> -> Qvalue
    //        action1, action2, action3, action4
    // etat1  qvalue   qvalue     ...     ...
    // etat2    ...      ...      ...     ...
    // etat3    ...      ...      ...     ...
    private void InitialisationQTable()
    {
        CombinaisonEtat();
        // On ajoute les états et les actions dans la Q-table
        foreach (string etat in etats)
        {
            foreach (string action in actions)
            {
                //Qtable_.Add(new Tuple<string, string>(etat, action), UnityEngine.Random.Range(-1f,1f));
                Qtable_.Add(new Tuple<string, string>(etat, action), 0f);
            }
        }
    }



    // Fonction de mise a jour de la Qtable
    // la mise a jourt se fait apr�s avoir fait l'action
    public void MaJQtable(string a, int HP_avant, int HP_apres, int HP_apres_ennemi, int HP_avant_ennemi,
                          int stamina_avant, int stamina_apres, int stamina_apres_ennemi, int stamina_avant_ennemi)
    {
        if (!editable)
        {
            return;
        }
        else
        {
            GetEtatActuel(HP_avant, stamina_avant);

            float Q_s_a = Qtable_[new Tuple<string, string>(etat_actuel_, a)];    //Q(s,a)
            string action_ancienne = a;   // On enregistre l'action précédente pour la mettre à jour après avoir choisi la nouvelle action

            string etat_ancien = etat_actuel_;          // On enregistre l'état précédent pour le mettre à jour après avoir choisi la nouvelle action
            GetEtatActuel(HP_apres, stamina_apres);     // On récupère l'état actuel après avoir fait l'action --> nouvel etat actuel

            float max_Q_sprime_aprime = maxQ(etat_actuel_); //maxQ(s',a')

            recompense_ = QReward(HP_apres, HP_avant, HP_apres_ennemi, HP_avant_ennemi, stamina_apres, stamina_avant, stamina_apres_ennemi, stamina_avant_ennemi); // r , calcul de la reward

            float Maj_valeur = (1 - alpha_) * Q_s_a + alpha_ * (recompense_ + gamma_ * max_Q_sprime_aprime); // calcul de la valeur de Q(s,a)

            Qtable_[new Tuple<string, string>(etat_ancien, action_ancienne)] = Maj_valeur;
        }
    }


    // ---------------------------------------
    // ||    Fonction pour les rewards      ||
    // ---------------------------------------

    private float QReward(int HP_apres_joueur, int HP_avant_joueur, int HP_apres_ennemi, int HP_avant_ennemi,
                          int stamina_apres_joueur, int stamina_avant_joueur, int stamina_apres_ennemi, int stamina_avant_ennemi)
    {
        float diff_HP = (HP_avant_ennemi - HP_apres_ennemi) + (HP_apres_joueur - HP_avant_joueur);

        float diff_stamina = (stamina_avant_ennemi - stamina_apres_ennemi) + (stamina_apres_joueur - stamina_avant_joueur);

        if (penalite)
        {
            penalite = false;
            // return (tunage * diff_HP + (1 - tunage) * diff_stamina) - PENALITE;
            return -PENALITE;

        }
        if (kill)
        {
            kill = false;
            return REWARD;
        }
        // return tunage * diff_HP + (1 - tunage) * diff_stamina;
        return 0f;
    }



    // -----------------------------------------------
    // ||    Fonctions pour les etat et actions     ||
    // -----------------------------------------------


    //Fonction pour avoir l'état actuel de l'agent
    public void GetEtatActuel(int HP, int Stamina)
    {
        if (HP < 30)
        {
            etat_actuel_ = "lowHP";
        }
        else if (HP < 70)
        {
            etat_actuel_ = "midHP";
        }
        else
        {
            etat_actuel_ = "highHP";
        }

        if (Stamina < 40)
        {
            etat_actuel_ += "_lowStamina";
        }
        else
        {
            etat_actuel_ += "_highStamina";
        }
    }


    //Fonction pour return l'action qui à la plus grande valeur dans la Qtable en fonction d'un état
    public float maxQ(string etat)
    {
        float max = -100000f;
        //Parcours de la Qtable pour trouver la meilleure action en fonction de l'état actuel + Si 2 actions ont la même valeur, on en prends une aléatoire
        List<float> actions_max = new List<float>();
        foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
        {
            if (cle.Key.Item1 == etat)
            {
                if (cle.Value > max)
                {
                    max = cle.Value;
                    actions_max.Clear();
                    actions_max.Add(cle.Value);
                }
                else if (cle.Value == max)
                {
                    actions_max.Add(cle.Value);
                }
            }
        }
        // On choisit une action aléatoire parmi les actions ayant la plus grande valeur dans la Q-table
        return actions_max[UnityEngine.Random.Range(0, actions_max.Count)];
    }

    //Fonction pour choisir l'action suivante
    private void ChoixAction(string etat)
    {
        // On ajoute un peu d'exploration
        // Si, en prenant un nombre aléatoire entre 0 et 1, on est en dessous du seuil d'exploration, l'agent va explorer = il fait nimp
        if (UnityEngine.Random.Range(0f, 1f) < epsylon_)
        {
            // On choisit une action al�atoire
            action_ = actions[UnityEngine.Random.Range(0, actions.Length)];
            // On diminue le taux d'exploration

        }
        else
        {
            float max = -100000f;
            //Parcours de la Qtable pour trouver la meilleure action en fonction de l'état actuel + Si 2 actions ont la même valeur, on en prends une aléatoire
            List<string> actions_max = new List<string>();
            foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
            {
                if (cle.Key.Item1 == etat_actuel_)
                {
                    if (cle.Value > max)
                    {
                        max = cle.Value;
                        actions_max.Clear();
                        actions_max.Add(cle.Key.Item2);
                    }
                    else if (cle.Value == max)
                    {
                        actions_max.Add(cle.Key.Item2);
                    }
                }
            }
            // On choisit une action aléatoire parmi les actions ayant la plus grande valeur dans la Q-table
            action_ = actions_max[UnityEngine.Random.Range(0, actions_max.Count)];

        }
        if (epsylon_ > 0.05f)
        {
            epsylon_ -= epsylon_delta;
        }
    }

    //Fonction pour faire un choix d'action unique
    public string ChoixActionUnique(int HP, int Stamina)
    {
        if (epsylon_ > 0.05f)
        {
            epsylon_ -= epsylon_delta;
        }
        // On ajoute un peu d'exploration
        // Si, en prenant un nombre aléatoire entre 0 et 1, on est en dessous du seuil d'exploration, l'agent va explorer = il fait nimp
        if (UnityEngine.Random.Range(0f, 1f) < epsylon_)
        {

            // On choisit une action al�atoire
            return actions[UnityEngine.Random.Range(0, actions.Length)];

        }
        else
        {

            if (etat_actuel_ == "")
            {
                GetEtatActuel(HP, Stamina);
            }

            float max = -100000f;
            //Parcours de la Qtable pour trouver la meilleure action en fonction de l'état actuel + Si 2 actions ont la même valeur, on en prends une aléatoire
            List<string> actions_max = new List<string>();
            foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
            {
                if (cle.Key.Item1 == etat_actuel_)
                {
                    if (cle.Value > max)
                    {
                        max = cle.Value;
                        actions_max.Clear();
                        actions_max.Add(cle.Key.Item2);
                    }
                    else if (cle.Value == max)
                    {
                        actions_max.Add(cle.Key.Item2);
                    }
                }
            }
            // On choisit une action aléatoire parmi les actions ayant la plus grande valeur dans la Q-table
            return actions_max[UnityEngine.Random.Range(0, actions_max.Count)];


        }


    }

    // ----------------------------------
    // ||    Fonctions utilitaires     ||
    // ----------------------------------

    public string DisplayQtable(Dictionary<Tuple<string, string>, float> qTable)
    {
        string[] uniqueStates = qTable.Keys.Select(pair => pair.Item1).Distinct().ToArray();
        string[] uniqueActions = qTable.Keys.Select(pair => pair.Item2).Distinct().ToArray();
        string header = "\t \t \t \t" + string.Join("\t", uniqueActions) + "\n";
        string table = "";

        foreach (var state in uniqueStates)
        {
            table += "\t" + state + "\t";
            foreach (var action in uniqueActions)
            {
                if (qTable.ContainsKey(Tuple.Create(state, action)))
                {
                    table += qTable[Tuple.Create(state, action)].ToString("F1") + "\t \t";
                }
                else
                {
                    table += "N/A\t \t";
                }
            }
            table += "\n";
        }

        return header + table;

    }

    public void CombinaisonEtat()
    {
        List<string> liste_HP = new List<string>() { "lowHP", "midHP", "highHP" };
        List<string> liste_Stamina = new List<string>() { "_lowStamina", "_highStamina" };

        foreach (string HP in liste_HP)
        {
            foreach (string Stamina in liste_Stamina)
            {
                etats.Add(HP + Stamina);
            }
        }

    }

    public void SaveQtable()
    {
        // On sauvegarde la Q-table dans un fichier texte
        // Le formatage du fichier texte est le suivant :
        // etat,action,recompense
        // Exemple : bgauche,gauche,0.5

        string[] Qtable_string = new string[Qtable_.Count];
        int i = 0;
        foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
        {
            string etat = cle.Key.Item1;
            string action = cle.Key.Item2;
            float qvalue = cle.Value;
            Qtable_string[i] = etat + "," + action + "," + qvalue;
            i++;
        }

        // On écrit la Q-table dans un fichier texte (qui est dans le dossier ./Assets/Qtables/) avec comme nom "Qtable-[Date du jour + Heure].txt"
        string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
        string filePath = System.IO.Path.Combine(Application.dataPath, "Qtables/Joueur", fileName);
        System.IO.File.WriteAllLines(filePath, Qtable_string);
    }

}
