using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For block catagorizing
public enum Block : byte 
{
    Air = 0,
    Dirt = 1,
    Grass = 2,
    Stone = 3
}

// Sort of like a factory but not.
// acutally, not really. TODO: Change name to BlockManager
public class BlockGallery : MonoBehaviour
{
    public Mesh cube;
    public Material grass;
    public Material dirt;

    // DEPRECATED function
    public void Draw(byte[,,] data, Vector3 chunkPos) 
    {
        for(int y = 0; y < Chunk.Y_SIZE; y++) {
            for(int x = 0; x < Chunk.X_SIZE; x++) {
                for(int z = 0; z < Chunk.Z_SIZE; z++) {
                    if (data[y, x, z] == 1) 
                        Graphics.DrawMesh(cube, chunkPos + new Vector3(x, y, z), Quaternion.identity, dirt, 0);
                }
            }
        }
    }

}
