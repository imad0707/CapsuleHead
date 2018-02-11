using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tegelPrefab;
    //public Vector2 mapSize;
    public float randAfstand = 0.05f;
    List<Coord> TegelCoord;
    Queue<Coord> gemengdeTegelCoord;
    //public int seed = 10;
    public Transform obstakelPrefab;
    //public float obstakelPercentage;
    //Coord middenMap;

    // variabelen m.b.t. navigatie masking
    public Transform navigatieVloer;
    public Transform mapVloer;
    public Vector2 maxMapSize;

    public Transform lavaPrefab;
    // public Vector3 lavaToVector = new Vector3 (-1,0,0); → kan ook met Vector3.left

    // alles m.b.t. map editor
    public Map[] maps;
    public int mapIndex;
    Map huidigeMap;

    // Random vijanden spawnen
    Transform[,] tegelMap;
    Queue<Coord> gemengdeVrijeTegelCoord;

    void Awake()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveGetal)
    {
        mapIndex = waveGetal - 1;
        GenerateMap();
    }
    
    // Map editor eigenschappen
    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        public float obstakelPercentage;
        public int seed;
        public Coord middenMap
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
    public void GenerateMap()
    {
        huidigeMap = maps[mapIndex];
        tegelMap = new Transform[huidigeMap.mapSize.x, huidigeMap.mapSize.y];
        System.Random randomNumberGenerator = new System.Random(huidigeMap.seed);
        //NOG NIET GOED!!!
        //GetComponent<BoxCollider>().size = new Vector3(huidigeMap.mapSize.x, .05f, huidigeMap.mapSize.y);


        // Coordinaten genereren 
        TegelCoord = new List<Coord>();
        // Coord lijst vullen mapSize.x * mapSize.y
        for (int x = 0; x < huidigeMap.mapSize.x; x++)
        {
            for (int y = 0; y < huidigeMap.mapSize.y; y++)
            {
                TegelCoord.Add(new Coord(x, y));
            }
        }
        gemengdeTegelCoord = new Queue<Coord>(GameMananger.MengArray(TegelCoord.ToArray(), Random.Range(0, 1000000))); //uit gamemananger (T[] array, int seed)
        //gemengdeTegelCoord = new Queue<Coord>(GameMananger.MengArray(TegelCoord.ToArray(), huidigeMap.seed)); //uit gamemananger (T[] array, int seed)

        // Map holder (hierarchie van spawn objecten)
        string holderName = "Generated Map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Tegels genereren  
        for (int x = 0; x < huidigeMap.mapSize.x; x++)
        {
            for (int y = 0; y < huidigeMap.mapSize.y; y++)
            {
                //Vector3 tegelPosition = new Vector3(-mapSize.x/2 +0.5f + x , 0, -mapSize.y/2 + 0.5f + y);
                Vector3 tegelPosition = CoordToPosition(x, y);

                Transform newtegel = Instantiate(tegelPrefab, tegelPosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newtegel.localScale = Vector3.one * (1 - randAfstand);
                newtegel.parent = mapHolder;
                tegelMap[x, y] = newtegel;
            }
            //  bool[,] obstakelMap = new bool[(int)mapSize.x, (int)mapSize.y];
            //  mapSize vector is niet statisch geeft error terug
        }

        // Obstakels genereren
        int obstakelTel = (int)(huidigeMap.mapSize.x * huidigeMap.mapSize.y * huidigeMap.obstakelPercentage);
        int huidigeObstakelTel = 0;
        List<Coord> alleVrijeCoords = new List<Coord>(TegelCoord);

        bool[,] obstakelTegel = new bool[(int)huidigeMap.mapSize.x, (int)huidigeMap.mapSize.y];

        for (int i = 0; i < obstakelTel; i++)
        {
            Coord randomCoord = RandomCoord();
            obstakelTegel[randomCoord.x, randomCoord.y] = true;
            huidigeObstakelTel++;

            if (randomCoord != huidigeMap.middenMap && MapIsVrij(obstakelTegel, huidigeObstakelTel)) //Map is vrij d.m.v. 2d bool array
            {
                Vector3 obstakelPosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstakel = Instantiate(obstakelPrefab, obstakelPosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstakel.parent = mapHolder;
                //kan bij de Y 1 (hoogte?)
                //newObstakel.localScale = new Vector3((1 - randAfstand), 1, (1 - randAfstand));


                alleVrijeCoords.Remove(randomCoord);
            }
            else
            {
                obstakelTegel[randomCoord.x, randomCoord.y] = false;
                huidigeObstakelTel--;
            }
        }

        gemengdeVrijeTegelCoord = new Queue<Coord>(GameMananger.MengArray(alleVrijeCoords.ToArray(), huidigeMap.seed)); //uit gamemananger (T[] array, int seed)


        //masken
        navigatieVloer.localScale = new Vector3(maxMapSize.x, maxMapSize.y); // De z-axis is nu de y-axis (90 graden)

        // uiterste punt van de map, x/2 geeft x=0, (m-x)/4 geeft het uiterste punt dus (x+m)/4
        Transform lavaLinks = Instantiate(lavaPrefab, Vector3.left * (huidigeMap.mapSize.x + maxMapSize.x) / 4, Quaternion.identity) as Transform;
        lavaLinks.parent = mapHolder;
        //vullen van het gebied tussen mapSize en maxMapSize
        lavaLinks.localScale = new Vector3((maxMapSize.x - huidigeMap.mapSize.x) / 2, 1, huidigeMap.mapSize.y /* = eigelijk z-axis*/);

        Transform lavaRechts = Instantiate(lavaPrefab, Vector3.right * (huidigeMap.mapSize.x + maxMapSize.x) / 4, Quaternion.identity) as Transform;
        lavaRechts.parent = mapHolder;
        lavaRechts.localScale = new Vector3((maxMapSize.x - huidigeMap.mapSize.x) / 2, 1, huidigeMap.mapSize.y);

        Transform lavaBoven = Instantiate(lavaPrefab, Vector3.forward * (huidigeMap.mapSize.y + maxMapSize.y) / 4, Quaternion.identity) as Transform;
        lavaBoven.parent = mapHolder;
        lavaBoven.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - huidigeMap.mapSize.y) / 2);

        Transform lavaBeneden = Instantiate(lavaPrefab, Vector3.back * (huidigeMap.mapSize.y + maxMapSize.y) / 4, Quaternion.identity) as Transform;
        lavaBeneden.parent = mapHolder;
        lavaBeneden.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - huidigeMap.mapSize.y) / 2);

        navigatieVloer.localScale = new Vector3(maxMapSize.x, maxMapSize.y);
        mapVloer.localScale = new Vector3(huidigeMap.mapSize.x, huidigeMap.mapSize.y);
    }

    //flood fill
    bool MapIsVrij(bool[,] obstakelTegel, int huidigeObstakelTel)
    {
        bool[,] mapChecked = new bool[obstakelTegel.GetLength(0), obstakelTegel.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(huidigeMap.middenMap); // al gechecked
        mapChecked[huidigeMap.middenMap.x, huidigeMap.middenMap.y] = true;

        int vrijeTegelCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int buurman_x = tile.x + x; //alle 8 ook diagonaal
                    int buurman_y = tile.y + y;
                    if (x == 0 || y == 0) // if (!(x= -1 && y = 1|| x= -1 && y = -1|| x= 1 && y = 1|| x= 1 && y = -1))
                    {
                        if (buurman_x >= 0 && buurman_x < obstakelTegel.GetLength(0) && buurman_y >= 0 && buurman_y < obstakelTegel.GetLength(1))
                        {
                            if (!mapChecked[buurman_x, buurman_y] && !obstakelTegel[buurman_x, buurman_y])
                            {
                                mapChecked[buurman_x, buurman_y] = true;
                                queue.Enqueue(new Coord(buurman_x, buurman_y)); //nog niet gechecked buurman
                                vrijeTegelCount++;
                            }
                        }
                    }
                }
            }
        }

        int werkelijkeVrijeTegelTel = (int)(huidigeMap.mapSize.x * huidigeMap.mapSize.y - huidigeObstakelTel);
        return werkelijkeVrijeTegelTel == vrijeTegelCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-huidigeMap.mapSize.x / 2 + 0.5f + x, 0, -huidigeMap.mapSize.y / 2 + 0.5f + y); // (-mapSize.x / 2 + 0.5f + x)
    }

    public Transform GetTegelPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x + (huidigeMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z + (huidigeMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tegelMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tegelMap.GetLength(1) - 1);
        return tegelMap[x, y];
    }

    public Coord RandomCoord()
    {
        Coord randomCoord = gemengdeTegelCoord.Dequeue();
        gemengdeTegelCoord.Enqueue(randomCoord);
        return randomCoord;
    }
    public Transform RandomVrijeTegel()
    {
        Coord randomCoord = gemengdeVrijeTegelCoord.Dequeue();
        gemengdeVrijeTegelCoord.Enqueue(randomCoord);
        return tegelMap[randomCoord.x, randomCoord.y];
    }

    // Coord class voor middenMap, TegelCoord(list), gemengdeTegelCoord(queue) → mengarray
    [System.Serializable]
    public class Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    
    
    }

//inspiratie:
//https://www.youtube.com/watch?v=fbZ8UcNsmSU
//https://stackoverflow.com/questions/14633999/flood-fill-implementation
//https://gist.github.com/ProGM/22a615b812c5a9f1183d43b536d14a42