using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform floorParent;
    [SerializeField] private GameObject floor;
    [SerializeField] private Transform spikeParent;
    [SerializeField] private GameObject spike;
    [SerializeField] private float tolerance;
    [SerializeField] private Vector2 minMaxSpikes = new Vector2(3, 5);
    [SerializeField] private PlayerCamera playerCamera;

    private bool lastHadSpikes = false;

    [SerializeField] private bool changeHeight = true;
    [SerializeField] private bool alternateNoSpikes = true;
    [SerializeField] private int[] spikePattern = new int[0];

    private Queue<GameObject> floorQueue = new Queue<GameObject>();
    private Queue<GameObject[]> spikeQueue = new Queue<GameObject[]>();

    private float lastSpawn;
    private int numberOfFloors = 0;
    private float height = -3;

    private void Start()
    {
        SetParams(GameManager.changeHeight, GameManager.alternateNoSpikes, GameManager.minMaxSpikes, GameManager.spikePattern);

        Initialise();
    }

    public void SetParams(bool changeHeight, bool alternateNoSpikes, Vector2 minMaxSpikes, int[] spikePattern)
    {
        this.changeHeight = changeHeight;
        this.alternateNoSpikes = alternateNoSpikes;
        this.minMaxSpikes = minMaxSpikes;
        this.spikePattern = spikePattern;
    }

    public void Initialise()
    {
        numberOfFloors = 0;
        lastSpawn = -2;
        height = -3;
        player.position = Vector3.zero;
        while (floorQueue.Count > 0)
        {
            Destroy(floorQueue.Dequeue());
        }

        while (spikeQueue.Count > 0)
        {
            DeleteSpikes();
        }

        while (playerCamera.height.Count > 0)
            playerCamera.height.Dequeue();
        
        playerCamera.height.Enqueue(3);
        
        GameObject f = Instantiate(floor, new Vector3(0, height, 0), quaternion.identity,
            floorParent);

        lastHadSpikes = false;
        floorQueue.Enqueue(f);
        spikeQueue.Enqueue(new GameObject[0]);
    }

    private void Update()
    {
        if (player.position.x % floor.transform.localScale.x < tolerance)
        {
            //Debug.Log($"{player.position.x} % {floor.transform.localScale.x} = {player.position.x % floor.transform.localScale.x}");
        }

        if (GameManager.Dead || GameManager.Pause)
        {
            return;
        }

        if (player.position.x % floor.transform.localScale.x < tolerance && Time.time - lastSpawn > 1)
        {
            if (changeHeight)
            {
                height += Random.Range(0.65f, 1.3f) * Random.Range(-1f, 1f) > 0 ? -1 : 1;
                if (playerCamera.height.Count > 1)
                    playerCamera.height.Dequeue();
                
                playerCamera.height.Enqueue(height + 6);
            }
            
            numberOfFloors++;
            lastSpawn = Time.time;

            GameObject f = Instantiate(floor, new Vector3(floor.transform.localScale.x * numberOfFloors, height, 0),
                quaternion.identity,
                floorParent);

            floorQueue.Enqueue(f);
            if (floorQueue.Count > 2)
            {
                Destroy(floorQueue.Dequeue());
            }
            

            float start = (floor.transform.localScale.x * numberOfFloors) - (floor.transform.localScale.x / 2);
            int numberOfSpikes = Random.Range((int) minMaxSpikes.x, (int) minMaxSpikes.y);

            if (spikePattern.Length > 0)
            {
                numberOfSpikes = spikePattern[(numberOfFloors-1) % spikePattern.Length];
               // Debug.Log($"{(numberOfFloors-1)} % {spikePattern.Length}={(numberOfFloors-1) % spikePattern.Length}");
            }
            
            float spacing = floor.transform.localScale.x / (numberOfSpikes + 1);

            if (alternateNoSpikes && lastHadSpikes && spikePattern.Length == 0)
            {
                numberOfSpikes = 0;
                lastHadSpikes = false;
            }
            else
            {
                lastHadSpikes = true;
            }

            GameObject[] spikes = new GameObject[numberOfSpikes];

            for (int i = 1; i <= numberOfSpikes; i++)
            {
                GameObject s = Instantiate(spike, new Vector3(start + i * spacing, height + 3, 0), quaternion.identity,
                    spikeParent);
                spikes[i - 1] = s;
            }


            spikeQueue.Enqueue(spikes);
            if (spikeQueue.Count > 2)
            {
                DeleteSpikes();
            }
        }
    }

    private void DeleteSpikes()
    {
        GameObject[] oldSpikes = spikeQueue.Dequeue();

        foreach (GameObject s in oldSpikes)
        {
            Destroy(s);
        }
    }
}

