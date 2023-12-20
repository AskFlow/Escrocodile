using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

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
    [SerializeField] private GameObject linerendererPrefab;

    private List<GameObject> doorSpawnPointRun = new List<GameObject>();
    private List<GameObject> instanciedDoors = new List<GameObject>();
    private List<List<GameObject>> instanciedDoorsDepth = new List<List<GameObject>>();

    private Door firstDoorComponent;
    private Door lastDoorComponent;
    List<Door> path = new List<Door>();
    LineRenderer lr;

    // UII

    public TextMeshProUGUI placeholderDepthMin;
    public TMP_InputField inputDepthMin;

    public TextMeshProUGUI placeholderDepthMax;
    public TMP_InputField inputDepthMax;

    public TextMeshProUGUI placeholderMaxDoor;
    public TMP_InputField inputMaxDoor;


    private void OnEnable()
    {
        GameManager.Instance._enableDelegateSoluce += EnableSoluce;
        GameManager.Instance._disableDelegateSoluce += DisableSoluce;
    }

    private void OnDisable()
    {
        GameManager.Instance._enableDelegateSoluce -= EnableSoluce;
        GameManager.Instance._disableDelegateSoluce -= DisableSoluce;

    }

    public void Start()
    {
        Random.InitState(GameManager.Instance.Seed);
        StartCoroutine(StartNewRound());
        placeholderDepthMax.text = DepthMax.ToString();
        placeholderDepthMin.text = DepthMin.ToString();
        placeholderMaxDoor.text = MaxDoorGenerated.ToString();
    }

    public void Restart()
    {
        Destroy(lr);
        lr = null;
        Random.InitState(GameManager.Instance.Seed);
        foreach (var item in instanciedDoors)
        {
            Destroy(item);
        }
        foreach (var item in instanciedDoorsDepth)
        {
            item.Clear();
        }

        instanciedDoorsDepth.Clear();

        if(inputDepthMin.text != "")
        {
            int result;
            int.TryParse(inputDepthMin.text, out result);
            result = Mathf.Clamp(result, 2, 8);
            inputDepthMin.text = result.ToString();
            DepthMin = result;
        }
        if(inputDepthMax.text != "")
        {
            int result;
            int.TryParse(inputDepthMax.text, out result);
            result = Mathf.Clamp(result, 2, 8);
            inputDepthMax.text = result.ToString();
            DepthMax = result;
        }
        if(inputMaxDoor.text != "")
        {
            int result;
            int.TryParse(inputMaxDoor.text, out result);
            result = Mathf.Clamp(result, 2, 7);
            MaxDoorGenerated = result;
            inputMaxDoor.text = result.ToString();
        }
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
        firstDoorComponent = firstDoor.GetComponent<Door>();
        firstDoorComponent.Id = doorId[0];
        doorId.RemoveAt(0);
        doorSpawnPointRun.RemoveAt(0);
        instanciedDoors.Add(firstDoor);
        instanciedDoorsDepth.Add(new List<GameObject>());
        instanciedDoorsDepth[0].Add(firstDoor);


        //yield return new WaitForSeconds(3);

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
            }
            //yield return new WaitForSeconds(3);
        }


        GameObject lastDoor = Instantiate(doorPrefab, doorSpawnPointRun[0].transform);
        lastDoorComponent = lastDoor.GetComponent<Door>();
        lastDoorComponent.Id = doorId[0];
        lastDoorComponent.DoorDepth = depth;
        lastDoorComponent.isLastDoor = true;    

        doorId.RemoveAt(0);
        doorSpawnPointRun.RemoveAt(0);
        instanciedDoors.Add(lastDoor);
        instanciedDoorsDepth.Add(new List<GameObject>());
        instanciedDoorsDepth[depth-1].Add(lastDoor);
        Door previousLastDoorRandom = instanciedDoorsDepth[depth - 2][Random.Range(0, instanciedDoorsDepth[depth - 2].Count)].GetComponent<Door>();
        lastDoorComponent.previousDoors.Add(previousLastDoorRandom.Id);
        lastDoorComponent.previousDoorsComponent.Add(previousLastDoorRandom);


        firstDoor.GetComponent<Door>().isDoorOpenable = true;
        foreach (var item in instanciedDoors)
        {
            item.GetComponent<Door>().Setup();
        }

        GetFastestSoluce();
        Setup();
        yield return null;
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

    public void GetFastestSoluce()
    {

        Door startNode = lastDoorComponent;
        Door targetNode = firstDoorComponent;

        List<Door> openSet = new List<Door>();
        HashSet<Door> closedSet = new HashSet<Door>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Door currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Door neighbor in currentNode.previousDoorsComponent)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parentDoor = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }
    int GetDistance(Door nodeA, Door nodeB)
    {
        return nodeB.DoorDepth - nodeA.DoorDepth;
    }

    void RetracePath(Door startNode, Door endNode)
    {
        path = new List<Door>();
        Door currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentDoor;
        }
        path.Add(startNode);
        path.Reverse();
    }


    public void Setup()
    {
        lr = Instantiate(linerendererPrefab).GetComponent<LineRenderer>();
        lr.positionCount = path.Count;
        int index = 0;
        foreach (var item in path)
        {
            lr.SetPosition(index, item.transform.position);
            index++;
        }
        lr.gameObject.SetActive(false);
    }

    public void EnableSoluce()
    {
        lr.gameObject.SetActive(true);
    }

    public void DisableSoluce()
    {
        lr.gameObject.SetActive(false);

    }
}
