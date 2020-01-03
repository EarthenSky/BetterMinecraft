using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// this class represents a single chunk of the world.
public class Chunk : MonoBehaviour
{
    public const byte S_FAST = 0;
    public const byte FAST = 1;
    public const byte MED = 2;
    public const byte GOOD = 3;

    public const int Z_SIZE = 32;
    public const int X_SIZE = 32;
    public const int Y_SIZE = 256;
    public const int SUB_CHUNKS = 32;
    public const int SUB_CHUNK_Y_SIZE = Y_SIZE / SUB_CHUNKS;


    private byte limit = 180;

    private byte[,,] _data;  // byte is uint8_t for c users
    /*private*/public Mesh[] sub_chunks;  // holds mesh data for 

    private BlockGallery _blocks;
    
    public void InitChunk(byte quality, GameObject world, BlockGallery blocks) 
    {
        _data = new byte[Y_SIZE, X_SIZE, Z_SIZE];  // accessed in xz slices.
        this.transform.parent = world.transform;

        _blocks = blocks;

        // Generate data here
        RandFillData();

        // Load Sub-Meshes
        LoadMeshLayers();
    }
    
    // Use position and seed to get data.
    private void RandFillData() 
    {
        for(int y = 0; y < Y_SIZE; y++) {
            for(int x = 0; x < X_SIZE; x++) {
                for(int z = 0; z < Z_SIZE; z++) {
                    byte val = (byte) UnityEngine.Random.Range(0f, 255f);

                    // assign block data.
                    if (val > limit + 32) {
                        _data[y, x, z] = (byte) 1;
                    } else if (val > limit) {
                        _data[y, x, z] = (byte) 2;
                    } else {
                        _data[y, x, z] = (byte) 0;
                    }
                }
            }
        }
    }

    // todo: do a "smart combine" where chunks of blocks are replaced with one large block. (if it has 6 sides)
    // TODO (Soon): side remover & vert combiner. 
    private void LoadMeshLayers() 
    {
        // Todo: use Mesh.CombineMeshes() //here: https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html

        int startTime = Environment.TickCount;
        Debug.Log("Loading Chunk @0");

        // need to create a gameobject to access the transform --> there is a better way to do this?
        GameObject posObj = new GameObject();

        sub_chunks = new Mesh[SUB_CHUNKS];
        for(int i = 0; i < SUB_CHUNKS; i++) {

            // counting how many blocks to combine
            int size = 0;
            for(int y = 0; y < Y_SIZE / SUB_CHUNKS; y++)
                for(int x = 0; x < X_SIZE; x++)
                    for(int z = 0; z < Z_SIZE; z++)
                        if (_data[y + i*SUB_CHUNK_Y_SIZE, x, z] == 1)
                            size++;

            CombineInstance[] combine = new CombineInstance[size]; //X_SIZE * SUB_CHUNK_Y_SIZE * Z_SIZE];
                            
            // iterate through the local space of a single chunk.
            int oi = 0;  // object index
            for(int y = 0; y < Y_SIZE / SUB_CHUNKS; y++) {
                for(int x = 0; x < X_SIZE; x++) {
                    for(int z = 0; z < Z_SIZE; z++) {
                        if (_data[y + i*SUB_CHUNK_Y_SIZE, x, z] == 1) {
                            combine[oi].mesh = _blocks.cube;
                            // todo: make meshes save data.
                            
                            posObj.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity); 
                            combine[oi].transform = posObj.transform.localToWorldMatrix;

                            oi++;
                        }
                    }
                }
            }
            
            sub_chunks[i] = new Mesh();
            sub_chunks[i].CombineMeshes(combine);
        }

        Destroy(posObj);
        
        Debug.Log("Done Loading Chunk @" + (Environment.TickCount - startTime).ToString());

    }

    public byte GetAt(int x, int y, int z) 
    {
        return _data[y, x, z];
    }

    public void SetAt(int x, int y, int z, byte val) 
    {
        _data[y, x, z] = val;
    }

    // Update is called once per frame
    void Update()
    {
        //_blocks.Draw(_data, transform.position);  // better?
        DrawLayers();
    }
    

    // Draws the loaded mesh layers.
    // todo: consider stepped chunk loading to be nice to gpu.
    public void DrawLayers() 
    {
        for(int y = 0; y < SUB_CHUNKS; y++) {
            Vector3 blockPos = transform.position + new Vector3(0, y * SUB_CHUNK_Y_SIZE, 0);
            Graphics.DrawMesh(sub_chunks[y], blockPos, Quaternion.identity, _blocks.dirt, 0);
        }
    }

}
