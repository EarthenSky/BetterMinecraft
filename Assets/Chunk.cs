﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
public class ChunkBuilder 
{
    public static Mesh BuildMesh(byte[,,] data) {

    }

}
*/


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
    
    // aka render data --> each number represents a part of building the mesh.
    // each byte is mapped to mean something: 
    // 1 byte = 0000_0000 = Top, Under, Left, Right, Front, Back, 0, 0  
    //        = TULR_FB00
    // these are varaibles that represent whether the mentioned side exists or not. 
    private byte[,,] _sideData;  

    /*private*/public Mesh[] subChunks;  // holds mesh data for 

    private BlockGallery _blocks;

    private Vector3[] verts;
    //private Vector2[] uvs;  // do i need uvs?
    private int[] tris;
    
    public void InitChunk(byte quality, GameObject world, BlockGallery blocks) 
    {
        _data = new byte[Y_SIZE, X_SIZE, Z_SIZE];  // accessed in xz slices.
        _sideData = new byte[Y_SIZE, X_SIZE, Z_SIZE];  // accessed in xz slices.
        
        this.transform.parent = world.transform;
        _blocks = blocks;

        // Generate block data, and side data.
        RandFillData();
        BakeData();
        
        //LoadMeshLayers();
        // Create this chunk's sub meshes.
        subChunks = new Mesh[SUB_CHUNKS];
        for(int i = 0; i < subChunks.Length; i++)
            CreateMeshLayer(i);
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
                        _data[y, x, z] = (byte) Block.Dirt;
                    } else if (val > limit) {
                        _data[y, x, z] = (byte) Block.Grass;
                    } else {
                        _data[y, x, z] = (byte) Block.Air;
                    }
                }
            }
        }
    }

    // This function determines what sides should be shown. (side data)
    private void BakeData () 
    {
        for(int y = 0; y < Y_SIZE; y++) {
            for(int x = 0; x < X_SIZE; x++) {
                for(int z = 0; z < Z_SIZE; z++) {
                    // Air blocks have no sides.
                    // TODO: construct a lookup table.
                    if(_data[y, x, z] == (byte) Block.Air) { 
                        _sideData[y, x, z] = (byte) 0;
                    } else {
                        bool t=false, u=false, l=false, r=false, f=false, b=false; // init switches.

                        if(y - 1 < Y_SIZE)
                            u = true;  // TODO: change this to ask side chunk if side chunk exists.
                        else
                            u = (_data[y - 1, x, z] == (byte) Block.Air) ? true : false;

                        if(y + 1 >= Y_SIZE)
                            t = true;
                        else
                            t = (_data[y + 1, x, z] == (byte) Block.Air) ? true : false;

                        if(x - 1 < 0)
                            l = true;
                        else
                            l = (_data[y, x - 1, z] == (byte) Block.Air) ? true : false;

                        if(x + 1 > X_SIZE)
                            r = true;
                        else
                            r = (_data[y, x + 1, z] == (byte) Block.Air) ? true : false;

                        if(z - 1 < 0)
                            f = true;
                        else
                            f = (_data[y, x, z - 1] == (byte) Block.Air) ? true : false;

                        if(z + 1 > Z_SIZE)
                            b = true;
                        else
                            b = (_data[y, x, z + 1] == (byte) Block.Air) ? true : false;

                        // TODO: find better way to do this
                        Func<bool, byte> btb = bo => (bo == true) ? (byte) 1 : (byte) 0;
                        _sideData[y, x, z] = (byte) ((byte) 128 * btb(t) + (byte) 64 * btb(u) + (byte) 32 * btb(l) + 
                                                     (byte) 16 * btb(r) + (byte) 8 * btb(f) + (byte) 4 * btb(b));
                    } 
                    // done with one block

                }
            }
        }
    }

    // This function creates the mesh for one layer (deletes old mesh? --> counts as refresh?)
    private void CreateMeshLayer(int i) {

    }

    // TODO: just change the array of triangles and stuff so that it's okay.
    private void EditMesh() {
    
    }

    // todo: do a "smart combine" where chunks of blocks are replaced with one large block. (if it has 6 sides)
    // TODO (Soon): side remover & vert combiner. 
    // DEPRECATED function
    private void LoadMeshLayers() 
    {
        // Todo: use Mesh.CombineMeshes() //here: https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html

        int startTime = Environment.TickCount;
        Debug.Log("Loading Chunk @0");

        // need to create a gameobject to access the transform --> there is a better way to do this?
        GameObject posObj = new GameObject();

        for(int i = 0; i < SUB_CHUNKS; i++) {

            // counting how many blocks to combine
            int size = 0;
            for(int y = 0; y < Y_SIZE / SUB_CHUNKS; y++)
                for(int x = 0; x < X_SIZE; x++)
                    for(int z = 0; z < Z_SIZE; z++)
                        if (_data[y + i*SUB_CHUNK_Y_SIZE, x, z] == (byte) Block.Dirt)
                            size++;
                            

            CombineInstance[] combine = new CombineInstance[size]; //X_SIZE * SUB_CHUNK_Y_SIZE * Z_SIZE];
                            
            // iterate through the local space of a single chunk.
            int oi = 0;  // object index
            for(int y = 0; y < Y_SIZE / SUB_CHUNKS; y++) {
                for(int x = 0; x < X_SIZE; x++) {
                    for(int z = 0; z < Z_SIZE; z++) {
                        if (_data[y + i*SUB_CHUNK_Y_SIZE, x, z] == (byte) Block.Dirt) {
                            combine[oi].mesh = _blocks.cube;
                            // todo: make meshes save data.
                            
                            posObj.transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity); 
                            combine[oi].transform = posObj.transform.localToWorldMatrix;

                            oi++;
                        }
                    }
                }
            }
            
            subChunks[i] = new Mesh();
            subChunks[i].CombineMeshes(combine);
        }

        Destroy(posObj);
        
        Debug.Log("Done Loading Chunk @" + (Environment.TickCount - startTime).ToString());

    }

    public byte GetBlock(int x, int y, int z) 
    {
        return _data[y, x, z];
    }

    // todo: this function will affect the bake (_sideData) and recall CreateMesh().
    public void SetBlock(int x, int y, int z, byte val) 
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
            Graphics.DrawMesh(subChunks[y], blockPos, Quaternion.identity, _blocks.dirt, 0);
        }
    }

}
