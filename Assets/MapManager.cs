﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class MapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject = null;
    [Range(1, 100)]
    private int blocksAhead = 3;
    [Range(1, 100)]
    private int blocksBehind = 2;
    [SerializeField]
    private GameObject[] blocksToSpawn = null;

    private Dictionary<GameObject, Vector3> endPoints = null;
    private List<GameObject> spawnedBlocks = null;
    private List<GameObject> blocksToDelete = null;
    public Dictionary<GameObject, GameObject> originalPrefabs = null;
    private float longuestBlockSize = 0f;

    private void Start()
    {
        spawnedBlocks = new List<GameObject>();
        blocksToDelete = new List<GameObject>();
        endPoints = new Dictionary<GameObject, Vector3>();
        originalPrefabs = new Dictionary<GameObject, GameObject>();

        foreach (GameObject g in blocksToSpawn)
        {
            Spline s = g.GetComponent<SpriteShapeController>().spline;
            int count = s.GetPointCount();

            endPoints.Add(g, GetLastPoint(g));

            longuestBlockSize = Mathf.Max(longuestBlockSize, endPoints[g].x);
        }

        NewMap();
    }

    private Vector3 GetLastPoint(GameObject g)
    {
        SpriteShapeController spriteShapeController = g.GetComponent<SpriteShapeController>();

        Vector3 point = Vector3.zero;

        Spline s = spriteShapeController.spline;
        int count = s.GetPointCount();

        for (int i = 0; i < count; i++)
        {
            Vector3 temp = s.GetPosition(i);
            if (temp.y <= -150f) continue;

            if (temp.x > point.x) point = temp;
        }

        return point;
    }

    private Vector3 GetLastPosition(GameObject block)
    {
        GameObject prefab = originalPrefabs[block];
        if (endPoints[prefab] == null)
        {
            return Vector3.zero;
        }
        return block.transform.localPosition + endPoints[prefab];
    }

    private void Update()
    {
        blocksToDelete.Clear();

        foreach (GameObject block in spawnedBlocks)
        {
            // for each block, we check if it is behind us to remove it
            if (block.transform.localPosition.x < playerObject.transform.position.x - longuestBlockSize * blocksBehind)
            {
                blocksToDelete.Add(block);
            }
        }

        // we now remove blocks and spawn new ones instead
        foreach (GameObject g in blocksToDelete)
        {
            DeleteBlock(g);
            SpawnRandomBlock();
        }
    }



    private void DeleteBlock(GameObject g)
    {
        originalPrefabs.Remove(g);
        spawnedBlocks.Remove(g);
        Destroy(g);
    }

    private void SpawnRandomBlock()
    {
        // get a random block type
        int i = Random.Range(0, blocksToSpawn.Length);
        GameObject blockToSpawn = blocksToSpawn[i];

        // get the very last block of the map
        GameObject lastBlock = spawnedBlocks[spawnedBlocks.Count - 1];

        // get the global position of the last point
        Vector3 endPosition = GetLastPosition(lastBlock);

        // spawn a new black at the end of the map
        GameObject spawnedBlock = Instantiate(blockToSpawn, endPosition, transform.localRotation, transform);
        originalPrefabs.Add(spawnedBlock, blockToSpawn);
        spawnedBlocks.Add(spawnedBlock);
    }

    public void NewMap()
    {
        // if there are blocks, remove them
        if (spawnedBlocks.Count > 0)
        {
            for (int x = spawnedBlocks.Count - 1; x >= 0; x--)
            {
                DeleteBlock(spawnedBlocks[x]);
            }
        }

        spawnedBlocks.Clear();
        originalPrefabs.Clear();

        // spawn the very first block
        GameObject spawnedBlock = Instantiate(blocksToSpawn[0], new Vector3(-(longuestBlockSize * blocksBehind), 0f), Quaternion.identity, transform);
        originalPrefabs.Add(spawnedBlock, blocksToSpawn[0]);
        spawnedBlocks.Add(spawnedBlock);

        // then, spawn the other blocks
        while (1 + blocksAhead + blocksBehind > spawnedBlocks.Count)
        {
            SpawnRandomBlock();
        }
    }
}