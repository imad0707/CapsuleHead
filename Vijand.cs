using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vijand : Wezens {

    public enum State { Niks, Volgen, Aanvallen};
    State huidigeState;
    NavMeshAgent navigatie;
    Transform doelwit;
    float aanvalAfstand = 1.25f;
    float aanvalTussentijd = 1;
    float nextAanvalTijd;
    Material skinMaterial;
    Color originalColor;
    Wezens doelwitWezen;
    float damage = 1;
    bool heeftDoelwit;
    public static event System.Action OnDeathStatic; // static versie voor de score
    
public GameObject doodEffect;

    void Awake()
    {
        navigatie = GetComponent<NavMeshAgent>();
        if (GameObject.FindWithTag("Player") != null)
        {
            heeftDoelwit = true;
            doelwit = GameObject.FindWithTag("Player").transform;
            doelwitWezen = doelwit.GetComponent<Wezens>();
        }
    }

    protected override void Start()
    {          // protected geeft waarschuwing?
        base.Start();

        if (heeftDoelwit)
        {
            huidigeState = State.Volgen;
            doelwitWezen.OnDeath += OnDoelwitDeath;

            //StartCoroutine(Updatenavigatie());
        }
    }

    public void SetKenmerk(float vijandsnelheid, int vijandDamange, float vijandHealth, Color vijandKleur)
    {
        navigatie.speed = vijandsnelheid;

        if (heeftDoelwit)
        {
            damage = Mathf.Ceil(doelwitWezen.startHealth/ vijandDamange);
        }
        startHealth = vijandHealth;

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = vijandKleur;
        originalColor = skinMaterial.color;
    }

    public override void Raak(float damage, Vector3 raakPunt, Vector3 raakRichting)
    {
        if (damage >= health)
        {
            // static voor de score
            if (OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            //
            Destroy(Instantiate(doodEffect, raakPunt, Quaternion.FromToRotation(Vector3.forward, raakRichting)) as GameObject, 2f);
        }
        base.Raak(damage, raakPunt, raakRichting);
    }

    void OnDoelwitDeath()
    {
        heeftDoelwit = false;
        huidigeState = State.Niks;
    }

    void Update () {
        navigatie.SetDestination(doelwit.position);

        if (huidigeState == State.Volgen)
        {
            if (Time.time > nextAanvalTijd)
            {
                float sqrAfstandDoelwit = (doelwit.position - transform.position).sqrMagnitude;
                if (sqrAfstandDoelwit < Mathf.Pow(aanvalAfstand, 2))
                {
                    nextAanvalTijd = Time.time + aanvalTussentijd;
                    StartCoroutine(Aanval());
                }
            }
        }
	}

    IEnumerator Aanval()
    {
        //navigatie.enabled = false;        NavMeshAgent hoeft niet uitgesckaled te worden http://answers.unity3d.com/questions/596200/animator-component-prevents-navmeshagent-from-disa.html
        huidigeState = State.Aanvallen;
        skinMaterial.color = Color.red;
        bool playerGewond = false;

        Vector3 originalPosition = transform.position;
        Vector3 aanvalPosition = doelwit.position;
        float percentage = 0;
        float aanvalSnelheid = 3;

        while (percentage <= 1)
        {
            if (percentage >= 0.5f && !playerGewond)
            {
                playerGewond = true;
                doelwitWezen.Damage(damage);
            }
            percentage += Time.deltaTime * aanvalSnelheid;
            float aanvalPolation = (-Mathf.Pow(percentage, 2) + percentage) * 4;
            transform.position = Vector3.Lerp(originalPosition, aanvalPosition, aanvalPolation);
            yield return null;
        }

        skinMaterial.color = originalColor;
        huidigeState = State.Volgen;
        //navigatie.enabled = true;

    }

    //IEnumerator Updatenavigatie()         werkt nog niet https://coding.abel.nu/2011/12/return-ienumerable-with-yield-return/
    //{
    //    float refreshRate = 0.25f;
    //
    //    while (doelwit != null)
    //    {
    //        Vector3 doelwitPositie = new Vector3(doelwit.position.x, 0, doelwit.position.z);
    //        navigatie.SetDestination(doelwitPositie);
    //        yield return new WaitForSeconds(refreshRate);
    //    }
    //}
}
