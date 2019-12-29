using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageWorld : MonoBehaviour
{
    public bool generateWorld = false;  // turn this on to generate a new world.

    private BlockGallery _blocks;
    private GameObject _world;

    // Start is called before the first frame update
    void Start()
    {
        _blocks = GameObject.Find("Block Gallery").GetComponent<BlockGallery>();

        _world = new GameObject("World");
        World world = _world.AddComponent<World>();
    }

    // Update is called once per frame
    void Update()
    {
        if(generateWorld == true) {
            _world.GetComponent<World>().GenerateWorld(_blocks);
            generateWorld = false;
        }
    }
}
