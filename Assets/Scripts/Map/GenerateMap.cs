using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Pathfinding;

public class GenerateMap : MonoBehaviour
{

    [Header("Map Generation")]
    [SerializeField] LayerMask heightLayers;
    LoadData loadData;
    Vector2 mapSize;
    int noiseScale;
    string seed;   
    [SerializeField] Transform tileSide;
    [SerializeField] Transform tile;
    [SerializeField] TileType[] tileTypes;

    [Header("Food")]
    [SerializeField] Transform foodDeposit;
    float foodDelay;
    int initialFood;

    [Header("Animals")]
    [SerializeField] Transform deer;
    [SerializeField] Transform bear;
    int[] animalNum = new int[2];
    float[] deerStats = new float[5];
    float[] bearStats = new float[6];
    float gestationTime;

    Vector3 tilePos;
    List<Vector2> waterPos = new List<Vector2>();
    List<Vector2> grassPos = new List<Vector2>();
    List<Vector2> drinkPos = new List<Vector2>();

    Material useMaterial;
    Transform mapHolder;
    Transform waterHolder;
    Transform grassHolder;
    Transform sandHolder;
    Transform foodHolder;
    Transform deerHolder;
    Transform bearHolder;

    bool foodReady;
    int currentSeed;

    AstarData data;

    GridGraph gg;

    private void Awake()
    {
        loadData = GameObject.Find("DataHolder").GetComponent<LoadData>();
        mapSize = loadData.mapSize;
        noiseScale = loadData.noiseScale;
        seed = loadData.seed;
        initialFood = loadData.initialFood;
        foodDelay = loadData.foodDelay;
        animalNum = loadData.animalNum;
        deerStats = (float[])loadData.deerStats.Clone();
        bearStats = (float[])loadData.bearStats.Clone();
        GameData.matureTime = loadData.matureTime;
        gestationTime = loadData.gestationTime;

        if (seed == "")
        {
            currentSeed = Random.Range(0, 10000000);
        } else
        {
            currentSeed = seed.GetHashCode();
        }

        Random.InitState(currentSeed);
    }

    void Start()
    {
        data = AstarPath.active.data;
        gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        gg.center = Vector3.zero;
        gg.collision.heightMask = heightLayers;
        gg.SetDimensions((int)mapSize.x, (int)mapSize.y, 1);       
        MapGeneration();
    }

    private void Update()
    {
        if (foodReady)
        {
            StartCoroutine(PlaceFoodDelay());
        }
    }

    public void MapGeneration()
    {
        float[,] noiseMap = PerlinNoise.GeneratePerlin((int)mapSize.x, (int)mapSize.y, noiseScale, currentSeed);
        string mapHolderName = "Generated Map";
        string waterHolderName = "Water Tiles";
        string grassHolderName = "Grass Tiles";
        string sandHolderName = "Sand Tiles";
        string foodHolderName = "Food Deposits";
        string deerHolderName = "Deer";
        string bearHolderName = "Bears";


        if (transform.Find(mapHolderName))
        {
            DestroyImmediate(transform.Find(mapHolderName).gameObject);
        }
        Transform[] tileParents = new Transform[tileTypes.Length];

        mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.parent = transform;
        waterHolder = new GameObject(waterHolderName).transform;
        tileParents[0] = waterHolder;
        sandHolder = new GameObject(sandHolderName).transform;
        tileParents[1] = sandHolder;
        grassHolder = new GameObject(grassHolderName).transform;
        tileParents[2] = grassHolder;
        foodHolder = new GameObject(foodHolderName).transform;
        deerHolder = new GameObject(deerHolderName).transform;
        bearHolder = new GameObject(bearHolderName).transform;

        for (int i = 0; i < tileParents.Length; i++)
        {
            tileParents[i].parent = mapHolder;
            tileParents[i].gameObject.AddComponent<MeshFilter>();
            tileParents[i].gameObject.AddComponent<MeshRenderer>();
            tileParents[i].gameObject.AddComponent<MeshCombiner>();
        }
        foodHolder.parent = mapHolder;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float perlinVal = noiseMap[x, y];
                float height = 0;
                string tileName = "";
                Transform parent = mapHolder;
                for (int i = 0; i < tileTypes.Length; i++)
                {
                    if (perlinVal <= tileTypes[i].perlinVal)
                    {
                        useMaterial = tileTypes[i].material;
                        height = tileTypes[i].height;
                        tileName = tileTypes[i].name;
                        parent = tileParents[i];
                        if (tileName == "Water")
                        {
                            waterPos.Add(new Vector2(x, y));
                        } else if (tileName == "Grass")
                        {
                            grassPos.Add(new Vector2(x, y));
                        }
                        break;
                    }
                }
                tilePos = newPos(x, height, y);
                Transform newTile = Instantiate(tile, tilePos, Quaternion.Euler(Vector3.right * 90), parent);
                newTile.GetComponent<MeshRenderer>().material = useMaterial;
                newTile.name = tileName;
            }
        }
        for (int i = 0; i < waterPos.Count; i++)
        {
            Vector2 currentPos = waterPos[i];
            CheckSide(currentPos);
        }
        for (int i = 0; i < tileParents.Length; i++)
        {
            tileParents[i].GetComponent<MeshCombiner>().CombineMeshes();
            tileParents[i].GetComponent<MeshRenderer>().material = tileTypes[i].material;
            tileParents[i].gameObject.AddComponent<MeshCollider>();
            tileParents[i].tag = tileTypes[i].name;
            if (tileTypes[i].name == "Water")
            {
                tileParents[i].gameObject.layer = 4;
            }
            tileParents[i].gameObject.isStatic = true;
        }      
        AstarPath.active.Scan();

        foodHolder.gameObject.isStatic = true;
        for (int i = 0; i < initialFood; i++)
        {
            PlaceFood();
        }
        foodReady = true;

        for (int i = 0; i < animalNum[0]; i++)
        {
            PlaceAnimal(deer, deerHolder, true);
        }
        GameData.deerPop = animalNum[0];
        if (animalNum[0] == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                GameData.deerAvgStats[i] = 0;
            }
        }

        for (int i = 0; i < animalNum[1]; i++)
        {
            PlaceAnimal(bear, bearHolder, false);
        }
        GameData.bearPop = animalNum[1];
        if (animalNum[1] == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                GameData.bearAvgStats[i] = 0;
            }
        }
    }

    [System.Serializable]
    public struct TileType {
        public string name;
        public float perlinVal;
        public float height;
        public Material material;
    }

    void CheckSide(Vector2 currentPos)
    {
        Vector2[] sideCheck = new Vector2[] { Vector2.up, Vector2.left, Vector2.right, Vector2.down };
        int[] angles = new int[] { 0, -90, 90, 180 };
        for (int i = 0; i < 4; i++)
        {
            Vector2 checkPos = currentPos + sideCheck[i];
            if (!waterPos.Contains(checkPos) && checkPos.x > -1 && checkPos.x < mapSize.x && checkPos.y > -1 && checkPos.y < mapSize.y)
            {
                drinkPos.Add(checkPos);
                tilePos = newPos(currentPos.x + (sideCheck[i].x / 2), -0.25f, currentPos.y + (sideCheck[i].y / 2));
                Transform newTile = Instantiate(tileSide, tilePos, Quaternion.Euler(Vector3.up * angles[i]), sandHolder);
                newTile.name = "TileSide";
            }
        }
    }

    Vector3 newPos (float x, float y, float z)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, y, -mapSize.y / 2 + 0.5f + z);
    }

    IEnumerator PlaceFoodDelay()
    {
        PlaceFood();
        foodReady = false;
        yield return new WaitForSeconds(foodDelay);
        foodReady = true;
    }

    void PlaceFood()
    {
        Vector2 randPos = grassPos[Random.Range(0, grassPos.Count)];
        Vector3 newRandPos = newPos(randPos.x, 0, randPos.y);
        Transform food = Instantiate(foodDeposit, newRandPos, Quaternion.identity, foodHolder);
        food.gameObject.isStatic = true;
        food.tag = "Food";
    }

    void PlaceAnimal(Transform animal, Transform parent, bool deer)
    {
        Vector2 randPos = grassPos[Random.Range(0, grassPos.Count)];
        Vector3 newRandPos = newPos(randPos.x, 0, randPos.y);
        Transform newAnimal = Instantiate(animal, newRandPos, Quaternion.identity, parent);
        EntityBase newAnimalEntity = newAnimal.GetComponent<EntityBase>();
        newAnimalEntity.adult = true;
        newAnimalEntity.pregnancyTime = gestationTime;
        newAnimalEntity.matureTime = GameData.matureTime;
        if (deer)
        {
            newAnimalEntity.Mutations(deerStats);
        }
        else
        {
            newAnimalEntity.Mutations(bearStats);
        }       
        var angle = newAnimal.rotation.eulerAngles;
        angle.y = Random.Range(-180, 181);
        newAnimal.rotation = Quaternion.Euler(angle);
        newAnimal.name = animal.name;      
    }
}
