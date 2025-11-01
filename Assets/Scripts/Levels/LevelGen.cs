using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    [Header("Toutes les rooms")]
    public List<GameObject> allPrefabsEasy;
    public List<GameObject> allPrefabsMid;
    public List<GameObject> allPrefabsHard;

    [Header("Sélection Random")]
    public int numberToSelect = 3;

    public GameObject dayscore;
    private List<GameObject> selectedPrefabs;

public GameObject player; 
    private GameObject currentInstance;
    private int currentIndex = 0;

    void Start()

    {
        GameScore gameScore = dayscore.GetComponent<GameScore>();
        if(gameScore.day == 1)
        {
            
        selectedPrefabs = GetRandomPrefabs(allPrefabsEasy, numberToSelect);
        selectedPrefabs[0] = allPrefabsEasy[0];
        } else if (gameScore.day == 2)
        {
            
        selectedPrefabs = GetRandomPrefabs(allPrefabsMid, numberToSelect);
        } else { 
        selectedPrefabs = GetRandomPrefabs(allPrefabsHard, numberToSelect);}

        NextRoom();
    }

    public void NextRoom()
    {
        if (currentInstance != null)
            Destroy(currentInstance);

        if (currentIndex >= selectedPrefabs.Count)
        {
            return;
        }

        currentInstance = Instantiate(selectedPrefabs[currentIndex], transform.position, Quaternion.identity);
        currentIndex++;

         Transform spawnPoint = currentInstance.transform.Find("Spawn");
        {
            player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
    }

    private List<GameObject> GetRandomPrefabs(List<GameObject> sourceList, int count)
    {
        List<GameObject> result = new List<GameObject>();
        List<GameObject> copy = new List<GameObject>(sourceList);

        count = Mathf.Min(count, copy.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(1, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }
}
