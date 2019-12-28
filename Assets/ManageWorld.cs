using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageWorld : MonoBehaviour
{
    public int renderCount = 100;
    public GameObject cube;

    public bool generateWorld = false;  // turn this on to generate a new world.

    private World mainWorld;

    // Start is called before the first frame update
    void Start()
    {
        // create n cubes
        for(int i = 0; i < renderCount; i++) {
            float x = Random.Range(-100, 100), y = Random.Range(-100, 100), z = Random.Range(20, 220);
            Instantiate( cube, new Vector3(x, y, z), transform.rotation );
        }

        mainWorld = new World();  // instance the world.
    }

    // Update is called once per frame
    void Update()
    {
        if(generateWorld == true) {
            mainWorld.GenerateWorld();
            generateWorld = false;
        }
    }
}


// This class represents a generated world.
public class World 
{
    private bool worldExists;
    public Vector3 position;

    private Chunk mainChunk;

    public World() 
    {
        worldExists = false;
        position = new Vector3(0, 0, 0);
    }

    private void DestroyWorld() 
    {
        // todo: this
        worldExists = false;
    }

    public void GenerateWorld() 
    {
        if(worldExists) DestroyWorld();
        mainChunk = new Chunk(Chunk.GOOD);

        worldExists = true;
    }
}

// this class represents a single chunk of the world.
public class Chunk
{
    public static byte S_FAST = 0;
    public static byte FAST = 1;
    public static byte MED = 2;
    public static byte GOOD = 3;

    public const int Z_SIZE = 32;
    public const int X_SIZE = 32;
    public const int Y_SIZE = 256;

    private byte[,,] data;  // byte is uint8_t for c users
    
    public Chunk(byte quality) 
    {
        data = new byte[Y_SIZE, X_SIZE, Z_SIZE];  // acessed in xz slices.
        // todo: generate the chuck here
    }

    public byte GetAt(int x, int y, int z) {
        return data[y, x, z];
    }

    public void SetAt(int x, int y, int z, byte val) {
        data[y, x, z] = val;
    }

}
