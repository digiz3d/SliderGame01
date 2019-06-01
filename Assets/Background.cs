using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    private PlayerControl player;

    [SerializeField]
    private float relativeSpeed = 100f;

    private float currentXOffset = 0f;

    private MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        currentXOffset += Time.deltaTime * player.GetCurrentSpeedX() * relativeSpeed;

        mesh.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(currentXOffset, 0));
    }
}
