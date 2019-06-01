using System.Collections.Generic;
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

    private List<GameObject> spawnedBlocks = null;
    private List<GameObject> blocksToDelete = null;
    private float longuestBlockSize = 0f;

    private void Start()
    {
        spawnedBlocks = new List<GameObject>();
        blocksToDelete = new List<GameObject>();

        foreach (GameObject g in blocksToSpawn)
        {
            Spline s = g.GetComponent<SpriteShapeController>().spline;
            int count = s.GetPointCount();
            longuestBlockSize = Mathf.Max(longuestBlockSize, s.GetPosition(count - 1).x);
        }

        NewMap();
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

    private Vector3 GetLastPosition(GameObject block)
    {
        SpriteShapeController spriteShapeController = block.GetComponent<SpriteShapeController>();
        if (!spriteShapeController) return Vector3.zero;

        Spline s = spriteShapeController.spline;
        int count = s.GetPointCount();
        return block.transform.localPosition + s.GetPosition(count - 1);
    }

    private void DeleteBlock(GameObject g)
    {
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
            spawnedBlocks.Clear();
        }

        // spawn the very first block
        spawnedBlocks.Add(Instantiate(blocksToSpawn[0], new Vector3(-(longuestBlockSize * blocksBehind), 0f), Quaternion.identity, transform));

        // then, spawn the other blocks
        while (1 + blocksAhead + blocksBehind > spawnedBlocks.Count)
        {
            SpawnRandomBlock();
        }
    }
}