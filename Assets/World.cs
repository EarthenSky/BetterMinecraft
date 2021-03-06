﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a generated world.
public class World : MonoBehaviour
{
    private bool _worldExists = false;
    private GameObject _chunk;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() { }

    private void DestroyWorld() 
    {
        GameObject.Destroy(_chunk);
        _worldExists = false;
    }

    public void GenerateWorld(BlockGallery blocks) 
    {
        if(_worldExists) DestroyWorld();

        // Create New Chunk (4x)
        for(int x = 0; x < 4; x++) {
            _chunk = new GameObject("Chunk");
            Chunk chunk = _chunk.AddComponent<Chunk>();
            chunk.InitChunk(Chunk.GOOD, gameObject, blocks);
            chunk.transform.Translate(new Vector3(x*32, 0, 0));
        }
        

        _worldExists = true;
    }
}
