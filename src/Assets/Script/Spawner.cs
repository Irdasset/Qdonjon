using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<GameObject> ennemisPrefabs;
    private List<string> qtables = new List<string>();

    void Start()
    {
        ennemisPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("ennemis"));
        string path = Path.Combine(Application.dataPath, "Qtables/Ennemis_Aventure");
        string[] files = Directory.GetFiles(path, "*.txt");
        foreach (string file in files)
        {
            qtables.Add(Path.GetFileNameWithoutExtension(file));
        }
    }

    public void SpawnEnnemi()
    {
        int index = Random.Range(0, ennemisPrefabs.Count);
        GameObject ennemiInstance = Instantiate(ennemisPrefabs[index], transform.position, Quaternion.Euler(0, -62, 0));
        ennemiInstance.tag = "Ennemi";
        ennemiInstance.AddComponent<Ennemi>();
        ennemiInstance.GetComponent<Ennemi>().qtable = new Qlearning("Ennemis_Aventure");
        Debug.Log("Qtable Ennemi : \n " + ennemiInstance.GetComponent<Ennemi>().qtable.DisplayQtable(ennemiInstance.GetComponent<Ennemi>().qtable.Qtable_));
    }
}