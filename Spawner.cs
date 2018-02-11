using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devmode; // Werkt nog niet

    public Ronde[] waves;
    public Vijand vijand;

    Ronde huidigeWave;
    int huidigeWaveLevel;

    int vijandNogOver;
    float nextSpawnTime;
    int vijandNogLevend;

    Transform playerT;

    MapGenerator map;

    Wezens playerWezen;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerWezen = FindObjectOfType<Player>();
        playerT = playerWezen.transform;
        playerWezen.OnDeath += VijandDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if ((vijandNogOver > 0 || huidigeWave.infinite) && Time.time > nextSpawnTime)
        {
            vijandNogOver--;
            nextSpawnTime = Time.time + huidigeWave.SpawnTime;

            StartCoroutine("SpawnVijand");
            //Vijand spawnedVijand = Instantiate(vijand, Vector3.zero, Quaternion.identity) as Vijand;
            //spawnedVijand.OnDeath += VijandDeath;
        }
        if (devmode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnVijand");
                // foreach (Vijand vijand in FindObjectOfType<Vijand>())
                // {
                //    GameObject.Destroy(vijand.gameObject);
                // }
                NextWave();
            }
        }
    }

    IEnumerator SpawnVijand()
    {
        float spawnDelay = 1;
        float tegelFlitsSnelheid = 4;
        Transform randomtegel = map.RandomVrijeTegel();
        Material tegelMat = randomtegel.GetComponent<Renderer>().material;
        Color origineleKleur = tegelMat.color;
        Color flitsKleur = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tegelMat.color = Color.Lerp(origineleKleur, flitsKleur, Mathf.PingPong(spawnTimer * tegelFlitsSnelheid, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Vijand spawnedVijand = Instantiate(vijand, randomtegel.position + Vector3.up, Quaternion.identity) as Vijand;
        spawnedVijand.OnDeath += VijandDeath;
        spawnedVijand.SetKenmerk(huidigeWave.vijandSnelheid, huidigeWave.vijandDamage, huidigeWave.vijandHealth, huidigeWave.vijandKleur);
    }

    void VijandDeath()
    {
        vijandNogLevend--;
        if (vijandNogLevend == 0)
        {
            NextWave();
        }
    }

    void ResetplayerPosition()
    {
        playerT.position = map.GetTegelPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        huidigeWaveLevel++;
        //print("Wave: " + huidigeWaveLevel);
        if (huidigeWaveLevel - 1 < waves.Length)
        {
            huidigeWave = waves[huidigeWaveLevel - 1];
            vijandNogOver = huidigeWave.VijandTel;
            vijandNogLevend = vijandNogOver;

            if (OnNewWave != null)
            {
                OnNewWave(huidigeWaveLevel);
            }
            ResetplayerPosition();
        }
    }

    [System.Serializable]
    public class Ronde
    {
        public int VijandTel;
        public float SpawnTime;
        public float vijandSnelheid;
        public int vijandDamage;
        public float vijandHealth;
        public Color vijandKleur;

        public bool infinite;
    }


}