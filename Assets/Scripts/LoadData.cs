using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{

    public float[] deerStats;
    public float[] bearStats;
    public float gestationTime;
    public float matureTime;
    public Vector2 mapSize;
    public int noiseScale;
    public string seed;
    public int initialFood;
    public float foodDelay;
    public int[] animalNum;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
