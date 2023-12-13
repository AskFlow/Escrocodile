using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> doorSpawnPoint = new List<GameObject>();
    [SerializeField] private int DepthMin;
    [SerializeField] private int DepthMax;

    [SerializeField] private int MaxDoorGenerated;

    [SerializeField] private int PercentageOfTwoLink = 30;
    [SerializeField] private int PercentageOfThreeLink = 5;

    [SerializeField] private int PercentageOfTwoDepth = 20;
    [SerializeField] private int PercentageOfThreeDepth = 5;

    [SerializeField] private GameObject doorPrefab;

    private List<GameObject> doorSpawnPointRun = new List<GameObject>();
    private List<GameObject> instanciedDoors = new List<GameObject>();
    private List<List<GameObject>> instanciedDoorsDepth = new List<List<GameObject>>();

    public void Start()
    {
        Random.InitState(GameManager.Instance.Seed);
        StartCoroutine(StartNewRound());
    }

    public IEnumerator StartNewRound()
    {
        doorSpawnPointRun = new List<GameObject>(doorSpawnPoint);
        foreach (var item in instanciedDoors)
        {
            Destroy(item);
        }
        instanciedDoors.Clear();
        Shuffle(doorSpawnPointRun);

        List<int> doorId = Enumerable.Range(0, doorSpawnPointRun.Count+1).ToList();
        Shuffle(doorId);
        int depth = Random.Range(DepthMin, DepthMax);

        GameObject firstDoor = Instantiate(doorPrefab, doorSpawnPointRun[0].transform);
        Door firstDoorComponent = firstDoor.GetComponent<Door>();
        firstDoorComponent.Id = doorId[0];
        doorId.RemoveAt(0);
        doorSpawnPointRun.RemoveAt(0);
        instanciedDoors.Add(firstDoor);
        instanciedDoorsDepth.Add(new List<GameObject>());
        instanciedDoorsDepth[0].Add(firstDoor);


        yield return new WaitForSeconds(3);

        for (int y = 1; y < depth - 1; y++)
        {
            instanciedDoorsDepth.Add(new List<GameObject>());
            for (int i = 0; i < Random.Range(1, MaxDoorGenerated); i++)
            {
                GameObject door = Instantiate(doorPrefab, doorSpawnPointRun[0].transform);
                Door doorComponent = door.GetComponent<Door>();
                doorComponent.Id = doorId[0];
                doorComponent.DoorDepth = y;
                doorId.RemoveAt(0);
                doorSpawnPointRun.RemoveAt(0);
                instanciedDoors.Add(door);
                instanciedDoorsDepth[y].Add(door);
            }
            foreach (var item in instanciedDoorsDepth[y])
            {
                Door doorItem = item.GetComponent<Door>();
                int randomNumberLink = Random.Range(0, 99);
                for (int x = 0; x < GetLink(); x++)
                {
                    int depthCurrent = GetDepth(y);
                    Door previousDoorRandom = instanciedDoorsDepth[y - depthCurrent][Random.Range(0, instanciedDoorsDepth[y - depthCurrent].Count)].GetComponent<Door>();
                    if (!doorItem.previousDoorsComponent.Contains(previousDoorRandom))
                    {
                        doorItem.previousDoors.Add(previousDoorRandom.Id);
                        doorItem.previousDoorsComponent.Add(previousDoorRandom);
                    }
                }
                //if (randomNumberLink <= PercentageOfThreeLink)
                //{
                //    for (int x = 0; x < 3; x++)
                //    {
                //        Door previousDoorRandom = instanciedDoorsDepth[y - 1][Random.Range(0, instanciedDoorsDepth[y - 1].Count)].GetComponent<Door>();
                //        doorItem.previousDoors.Add(previousDoorRandom.Id);
                //        doorItem.previousDoorsComponent.Add(previousDoorRandom);
                //    }
                //}
                //else if(PercentageOfThreeLink <= randomNumberLink && randomNumberLink <= PercentageOfTwoLink)
                //{
                //    for (int x = 0;x < 2; x++)
                //    {
                //        Door previousDoorRandom = instanciedDoorsDepth[y - 1][Random.Range(0, instanciedDoorsDepth[y - 1].Count)].GetComponent<Door>();
                //        doorItem.previousDoors.Add(previousDoorRandom.Id);
                //        doorItem.previousDoorsComponent.Add(previousDoorRandom);
                //    }
                //}
                //else
                //{
                //    Door previousDoorRandom = instanciedDoorsDepth[y - 1][Random.Range(0, instanciedDoorsDepth[y - 1].Count)].GetComponent<Door>();
                //    doorItem.previousDoors.Add(previousDoorRandom.Id);
                //    doorItem.previousDoorsComponent.Add(previousDoorRandom);
                //}

            }
            yield return new WaitForSeconds(3);
        }


        GameObject lastDoor = Instantiate(doorPrefab, doorSpawnPointRun[0].transform);
        Door lastDoorComponent = lastDoor.GetComponent<Door>();
        lastDoorComponent.Id = doorId[0];
        lastDoorComponent.DoorDepth = depth;

        doorId.RemoveAt(0);
        doorSpawnPointRun.RemoveAt(0);
        instanciedDoors.Add(lastDoor);
        instanciedDoorsDepth.Add(new List<GameObject>());
        instanciedDoorsDepth[depth-1].Add(lastDoor);
        Door previousLastDoorRandom = instanciedDoorsDepth[depth - 2][Random.Range(0, instanciedDoorsDepth[depth - 2].Count)].GetComponent<Door>();
        lastDoorComponent.previousDoors.Add(previousLastDoorRandom.Id);
        lastDoorComponent.previousDoorsComponent.Add(previousLastDoorRandom);























        //for (int i = 0; i < numberOfDoor; i++)
        //{
        //    int lastElement = doorId.Last();
        //    doorId.RemoveAt(doorId.Count -1);
        //    if (i != 0)
        //    {
        //        instanciedDoors[i-1].GetComponent<Door>().NextDoorId = lastElement;
        //    }
        //    GameObject go = Instantiate(doorPrefab, doorSpawnPointRun.Last().transform);
        //    doorSpawnPointRun.RemoveAt(doorSpawnPointRun.Count -1);
        //    go.GetComponent<Door>().Id = lastElement;
        //    instanciedDoors.Add(go);
        //}

        //FindObjectOfType<Player>().currentKey = instanciedDoors[0].GetComponent<Door>().Id;
        //instanciedDoors[instanciedDoors.Count -1].GetComponent<Door>().isLastDoor = true;
    }

    public List<T> Shuffle<T>(List<T> nom)
    {
        var count = nom.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {

            var r = UnityEngine.Random.Range(i, count);
            var tmp = nom[i];
            nom[i] = nom[r];
            nom[r] = tmp;
        }
        return nom;
    }

    public int GetDepth(int currentDepth)
    {
        int randomNumberDepth = Random.Range(0, 99);
        int depthRandom = 0;
        if (randomNumberDepth <= PercentageOfThreeDepth)
        {
            depthRandom = 3;
        }
        else if (PercentageOfThreeDepth <= randomNumberDepth && randomNumberDepth <= PercentageOfTwoDepth)
        {
            depthRandom = 2;

        }
        else
        {
            depthRandom = 1;

        }
        return Mathf.Clamp(depthRandom,1,currentDepth);
    }

    public int GetLink()
    {
        int randomNumberLink = Random.Range(0, 99);
        int linkRandom = 0;
        if (randomNumberLink <= PercentageOfThreeLink)
        {
            linkRandom = 3;
        }
        else if (PercentageOfThreeLink <= randomNumberLink && randomNumberLink <= PercentageOfTwoLink)
        {
            linkRandom = 2;

        }
        else
        {
            linkRandom = 1;
        }
        return linkRandom;
    }
}
