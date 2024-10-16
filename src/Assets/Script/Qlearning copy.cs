// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using System.IO;
// using UnityEditor.EditorTools;
// using Unity.VisualScripting;
// using System.Net.Mime;
// using System.Linq;

// public class Qlearning
// {
//     // ----------------------------------------------
//     // ||    Variables du jeu et du Q-learning     ||
//     // ----------------------------------------------

//     //Hyperparam�tres
//     private float alpha_ = 0.1f;                // Taux d'apprentissage : 0 = pas d'apprentissage, 1 = apprentissage total
//     private float gamma_ = 0.98f;               // Taux d'actualisation : 0 = l'agent regarde la r�compense la plus proche, 1 = regarde les r�compenses futures
//     private float epsylon_ = 0.9f;              // Taux d'exploration : 0 = pas d'exploration, 1 = exploration totale
//     private float epsylon_delta = 0.0005f;      // Diminution du taux d'exploration � chaque action

//     //Variables du jeu
//     private float recompense_ = 0f;
//     private string etat_actuel_ = "";
//     private string etat_nouveau_ = "";
//     public string action_ = "";
//     private string[] actions = { "Attaquer", "Defendre", "Esquiver", "Heal" };

//     // Etat + stamina de l'ennemie + stamina du joueur 
//     private string[] etats = { "lowHP", "midHP", "highHP", };

//     //Condition Qtable éditable   
//     public bool editable = true;


//     //Qtable

//     public Dictionary<Tuple<string, string>, float> Qtable_ = new Dictionary<Tuple<string, string>, float>();

//     // ----------------------------------------------
//     // ||    Fonctions pour le début                ||
//     // ----------------------------------------------


//     public Qlearning(string path)
//     {
//         // On récupère tous les fichiers dans le dossier Qtables avec l'extension ".txt"
//         string[] qtableFiles = System.IO.Directory.GetFiles("Assets/Qtables/" + path, "*.txt");

//         // Si le dossier Qtables contient des fichiers ".txt"
//         if (qtableFiles.Length > 0)
//         {
//             // On trie les fichiers par date de modification (du plus récent au plus ancien)
//             Array.Sort(qtableFiles, (a, b) => System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a)));

//             // On récupère le fichier le plus récent
//             string latestQtableFile = qtableFiles[0];

//             // On récupère la Q-table dans le fichier le plus récent
//             string[] qtableString = System.IO.File.ReadAllLines(latestQtableFile);

//             // On récupère les états, les actions et les récompenses dans la Q-table
//             foreach (string line in qtableString)
//             {
//                 // On récupère les valeurs de la ligne
//                 string[] values = line.Split(',');
//                 string state = values[0];
//                 string action = values[1];
//                 float qvalue = float.Parse(values[2]);

//                 // On ajoute les valeurs dans la Q-table
//                 Qtable_.Add(new Tuple<string, string>(state, action), qvalue);
//             }
//         }
//         else
//         {
//             InitialisationQTable();
//         }

//     }


//     //Fonction pour initialiser la Q-table :
//     // La Q-table est de la forme : <etat,action> -> Qvalue
//     //        action1, action2, action3, action4
//     // etat1  qvalue   qvalue     ...     ...
//     // etat2    ...      ...      ...     ...
//     // etat3    ...      ...      ...     ...
//     private void InitialisationQTable()
//     {

//         // On ajoute les �tats et les actions dans la Q-table
//         foreach (string etat in etats)
//         {
//             foreach (string action in actions)
//             {
//                 //Qtable_.Add(new Tuple<string, string>(etat, action), UnityEngine.Random.Range(-1f,1f));
//                 Qtable_.Add(new Tuple<string, string>(etat, action), 15f);
//             }
//         }
//     }


//     //Fonction pour choisir l'action suivante
//     private void ChoixAction(string etat)
//     {
//         // On ajoute un peu d'exploration
//         // Si, en prenant un nombre aléatoire entre 0 et 1, on est en dessous du seuil d'exploration, l'agent va explorer = il fait nimp
//         if (UnityEngine.Random.Range(0f, 1f) < epsylon_)
//         {
//             // On choisit une action al�atoire
//             action_ = actions[UnityEngine.Random.Range(0, actions.Length)];
//             //Debug.Log("RANDOM");
//             // On diminue le taux d'exploration
//             epsylon_ -= epsylon_delta;

//         }
//         else
//         {
//             //Debug.Log("QLEARNING");
//             //Parcours de la Qtable pour trouver la meilleure action en fonction de l'état actuel
//             float max = -100000f;
//             foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
//             {
//                 if (cle.Key.Item1 == etat)
//                 {
//                     if (cle.Value > max)
//                     {
//                         max = cle.Value;
//                         action_ = cle.Key.Item2;
//                     }
//                 }
//             }
//         }
//     }

//     //Fonction pour faire un choix d'action unique
//     public void ChoixActionUnique(int HP)
//     {
//         // On ajoute un peu d'exploration
//         // Si, en prenant un nombre aléatoire entre 0 et 1, on est en dessous du seuil d'exploration, l'agent va explorer = il fait nimp
//         if (UnityEngine.Random.Range(0f, 1f) < epsylon_)
//         {
//             // On choisit une action al�atoire
//             action_ = actions[UnityEngine.Random.Range(0, actions.Length)];
//             //Debug.Log("RANDOM");
//             // On diminue le taux d'exploration
//             epsylon_ -= epsylon_delta;

//         }
//         else
//         {

//             if (etat_actuel_ == "")
//             {
//                 GetEtatActuel(HP);
//             }

//             float max = -100000f;
//             foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
//             {
//                 if (cle.Key.Item1 == etat_actuel_)
//                 {
//                     if (cle.Value > max)
//                     {
//                         max = cle.Value;
//                         action_ = cle.Key.Item2;
//                     }
//                 }
//             }
//         }
//     }

//     // Fonction de mise a jour de la Qtable
//     // la mise a jourt se fait apr�s avoir fait l'action
//     public void MaJQtable(int HP_avant, int HP_apres, int HP_apres_ennemi, int HP_avant_ennemi,
//                           int stamina_avant, int stamina_apres, int stamina_apres_ennemi, int stamina_avant_ennemi)
//     {
//         if (!editable)
//         {
//             return;
//         }
//         else
//         {
//             GetEtatActuel(HP_avant);

//             if (action_ == "")
//             {
//                 ChoixAction(etat_actuel_);
//             };

//             float valeur_ancienne = Qtable_[new Tuple<string, string>(etat_actuel_, action_)];
//             string action_ancienne = action_;

//             GetEtatActuel(HP_apres);
//             etat_nouveau_ = etat_actuel_;

//             float valeur_actuelle = Qtable_[new Tuple<string, string>(etat_nouveau_, action_ancienne)];
//             recompense_ = QReward(HP_apres, HP_avant, HP_apres_ennemi, HP_avant_ennemi, stamina_apres, stamina_avant, stamina_apres_ennemi, stamina_avant_ennemi);
           
//             float Maj_valeur = (1 - alpha_) * valeur_ancienne + alpha_ * (recompense_ + gamma_ * (valeur_actuelle - valeur_ancienne));
//             Qtable_[new Tuple<string, string>(etat_actuel_, action_)] = Maj_valeur;
//         }
//     }


//     public void SaveQtable()
//     {
//         // On sauvegarde la Q-table dans un fichier texte
//         // Le formatage du fichier texte est le suivant :
//         // etat,action,recompense
//         // Exemple : bgauche,gauche,0.5

//         string[] Qtable_string = new string[Qtable_.Count];
//         int i = 0;
//         foreach (KeyValuePair<Tuple<string, string>, float> cle in Qtable_)
//         {
//             string etat = cle.Key.Item1;
//             string action = cle.Key.Item2;
//             float qvalue = cle.Value;
//             Qtable_string[i] = etat + "," + action + "," + qvalue;
//             i++;
//         }

//         // On écrit la Q-table dans un fichier texte (qui est dans le dossier ./Assets/Qtables/) avec comme nom "Qtable-[Date du jour + Heure].txt"
//         string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
//         string filePath = System.IO.Path.Combine(Application.dataPath, "Qtables/Joueur", fileName);
//         System.IO.File.WriteAllLines(filePath, Qtable_string);
//     }


//     // ---------------------------------------
//     // ||    Fonction pour les rewards      ||
//     // ---------------------------------------

//     private float QReward(int HP_apres_joueur, int HP_avant_joueur, int HP_apres_ennemi, int HP_avant_ennemi,
//                           int stamina_apres_joueur, int stamina_avant_joueur, int stamina_apres_ennemi, int stamina_avant_ennemi)
//     {   
//         float diff_HP = (HP_avant_ennemi - HP_apres_ennemi) - (HP_apres_joueur - HP_avant_joueur);
//         float diff_stamina = (stamina_avant_ennemi - stamina_apres_ennemi) - (stamina_apres_joueur - stamina_avant_joueur);

//         return diff_HP + diff_stamina; 
//     }


//     // --------------------------------------------
//     // ||    Fonctions pour les actions/etat     ||
//     // --------------------------------------------


//     //Fonction pour avoir l'�tat actuel de l'agent
//     public void GetEtatActuel(int HP)
//     {
//         if (HP < 30)
//         {
//             etat_actuel_ = "lowHP";
//         }
//         else if (HP < 70)
//         {
//             etat_actuel_ = "midHP";
//         }
//         else
//         {
//             etat_actuel_ = "highHP";
//         }
//     }


//     // ---------------------------------------------------
//     // ||    Fonctions pour la gestion des �pisodes     ||
//     // ---------------------------------------------------

//     //A chaque frame, cette fonction se lance
//     public void Update()
//     {
//     }

//     public string DisplayQtable(Dictionary<Tuple<string, string>, float> qTable)
//     {
//         string[] uniqueStates = qTable.Keys.Select(pair => pair.Item1).Distinct().ToArray();
//         string[] uniqueActions = qTable.Keys.Select(pair => pair.Item2).Distinct().ToArray();
//         string header = "\t" + string.Join("\t", uniqueActions) + "\n";
//         string table = "";

//         foreach (var state in uniqueStates)
//         {
//             table += state + "\t";
//             foreach (var action in uniqueActions)
//             {
//                 if (qTable.ContainsKey(Tuple.Create(state, action)))
//                 {
//                     table += qTable[Tuple.Create(state, action)].ToString("F2") + "\t";
//                 }
//                 else
//                 {
//                     table += "N/A\t";
//                 }
//             }
//             table += "\n";
//         }

//         return "test" + header + table;

//     }


// }
