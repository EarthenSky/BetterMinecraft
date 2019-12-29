using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static byte limit = 255 - 25;

    private byte[,,] _data;  // byte is uint8_t for c users

    private BlockGallery _blocks;
    
    public void InitChunk(byte quality, GameObject world, BlockGallery blocks) 
    {
        _data = new byte[Y_SIZE, X_SIZE, Z_SIZE];  // accessed in xz slices.
        this.transform.parent = world.transform;

        _blocks = blocks;

        // Generate data here
        RandFillData();
    }
    
    // Use position and seed to get data.
    private void RandFillData() 
    {
        //float persistance = 1.2f;
        for(int y = 0; y < Y_SIZE; y++) {
            for(int x = 0; x < X_SIZE; x++) {
                for(int z = 0; z < Z_SIZE; z++) {
                    byte val = (byte) /* Mathf.Clamp(*/ Random.Range(0f, 255f); /* / y * 200, 0, 255 ); */
                    _data[y, x, z] = val;
                }
            }
        }
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
        _blocks.Draw(_data, transform.position);  // better?
    }

}
