using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
    
    public static int score { get; private set; }
    float tijdSindsLaatsteKill;
    int comboTel;
    float comboEindTijd = 0.5f;

    // het is niet handig dat het telkens naar de onDeath event moet luisteren
    void Start()
    {
        score = 0;
        Vijand.OnDeathStatic += OnVijandKilled; // probleem met unsubscribe als je dood gaat → "Error Static Member cannot be accessed"
        // bug : je krijgt 2x zoveel punten telkens als je dood gaat
        FindObjectOfType<Player>().OnDeath += IsSpelerDood; 
    }

    void OnVijandKilled()
    {
        if (Time.time < tijdSindsLaatsteKill + comboEindTijd)
        {
            comboTel++;
        }
        else
        {
            comboTel = 0;
        }

        tijdSindsLaatsteKill = Time.time;

        score += 2 + (int)Mathf.Pow(2, comboTel); // casten naar int → float
    }

    // Unsubscriben van de static event → bug: score x2
    void IsSpelerDood()
    {
        Vijand.OnDeathStatic -= OnVijandKilled;
    }
}
