using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sort of like a factory but not.
// acutally, not really. TODO: Change name to BlockManager
public class BlockGallery : MonoBehaviour
{
    public Mesh cube;
    public Material grass;
    public Material dirt;

    public void Draw(byte[,,] data, Vector3 chunkPos) 
    {
        for(int y = 0; y < Chunk.Y_SIZE; y++) {
            for(int x = 0; x < Chunk.X_SIZE; x++) {
                for(int z = 0; z < Chunk.Z_SIZE; z++) {
                    if (data[y, x, z] > Chunk.limit) 
                        Graphics.DrawMesh(cube, chunkPos + new Vector3(x, y, z), Quaternion.identity, dirt, 0);
                }
            }
        }
    }

/*
    public void DrawLayers(byte[,,] data, Vector3 chunkPos) 
    {
        for(int y = 0; y < Chunk.Y_SIZE; y++) {
            for(int x = 0; x < Chunk.X_SIZE; x++) {
                for(int z = 0; z < Chunk.Z_SIZE; z++) {
                    if (data[y, x, z] > Chunk.limit) 
                        Graphics.DrawMesh(cube, chunkPos + new Vector3(x, y, z), Quaternion.identity, dirt, 0);
                }
            }
        }
    }
*/
}
