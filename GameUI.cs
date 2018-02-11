using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadeCanvas;
    public GameObject gameOverUI;

    public RectTransform WaveUI;
    public Text WaveNummer;
    public Text WaveVijandenTel;

    Spawner spawner;
    Player player;

    public Text scoreUI;
    public RectTransform healthBalk;

    void Start () {
        player = FindObjectOfType<Player>(); // ref naar player
        player.OnDeath += OnGameOver;
	}
	
    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += BijNieuweWave;
    }

    void Update()
    {
        scoreUI.text = Score.score.ToString("D5");
        // ref naar healthbalk
        float healthPercentage = player.health / player.startHealth;
        healthBalk.localScale = new Vector3(healthPercentage, 1, 1);
    }

    void BijNieuweWave (int waveNummer)
    {
        string[] nummer = { "1 makkelijk", "2 iets lastiger", "3 middelmatig", "4 moeilijk", "5 hahaha veel succes" };
        WaveNummer.text = "Wave" +" "+ nummer[waveNummer - 1];
        string vijandTelString = ((spawner.waves [waveNummer - 1].infinite) ? "Oneindig" : spawner.waves [waveNummer - 1].VijandTel+ ""); //Oneindige ronde bug fix
        WaveVijandenTel.text = "Vijanden: " + vijandTelString;

        //StopCoroutine(WaveUIAnimatie());
        //StartCoroutine(WaveUIAnimatie()); // method group naar IEnumerator error
        StopCoroutine("WaveUIAnimatie");
        StartCoroutine("WaveUIAnimatie");
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator WaveUIAnimatie()
    {
        float delay = 1f;
        float speed = 2.5f;
        float animatiePercentage = 0;
        int maxHoogte = 1;
        float stopDelay = Time.deltaTime + 1 / speed + delay;

        while (animatiePercentage >= 0)
        {
            animatiePercentage += Time.deltaTime * speed * maxHoogte;
            //if (maxHoogte >= 1)
            if (animatiePercentage >= 1)
            {
                animatiePercentage = 1;
                if(Time.deltaTime > stopDelay)
                {
                    maxHoogte = -1;
                }
            }
            WaveUI.anchoredPosition = Vector2.up * Mathf.Lerp(50, 200, animatiePercentage);
            yield return null;
        }
    }

    IEnumerator Fade(Color van, Color tot, float tijd)
    {
        float snelheid = 1 / tijd;
        float percentage = 0;

        while (percentage < 1)
        {
            percentage += Time.deltaTime * snelheid;
            fadeCanvas.color = Color.Lerp(van, tot, percentage);
            yield return null;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void TerugNaarMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
