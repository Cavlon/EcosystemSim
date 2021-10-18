using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    public static float[,] GeneratePerlin(int mapWidth, int mapHeight, int scale, int seed)
    {

        System.Random rand = new System.Random(seed);

        float[,] noiseMap = new float[mapWidth, mapHeight];
        if (scale <= 0)
        {
            scale = 1;
        }

        float xOffset = rand.Next(-100000, 100000);
        float yOffset = rand.Next(-100000, 100000);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float tempX = (float)x  / mapWidth * scale + xOffset;
                float tempY = (float)y / mapHeight * scale + yOffset;

                float noiseVal = Mathf.PerlinNoise(tempX, tempY);
                noiseMap[x, y] = noiseVal;
            }
        }
        return noiseMap;
    }

}
