using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wapen : MonoBehaviour {

    public enum WapenMode {Single, Burst, Auto };
    public WapenMode wapenMode;
    bool trekkerLos;
    public int burstTel;
    int schotenOverInBurst;

    public Transform uitgang;
    public Kogels kogel;
    public float kogelRate = 100;
    public float uitgangSpeed = 35;

    float volgendSchot;

    void Start()
    {
        schotenOverInBurst = burstTel;
    }

    public void Schiet()
    {
        if (Time.time > volgendSchot)
        {
            if (wapenMode== WapenMode.Burst)
            {
                if (schotenOverInBurst == 0)
                {
                    return;
                }
                schotenOverInBurst--;
            }
            else if (wapenMode == WapenMode.Single)
            {
                if (!trekkerLos)
                {
                    return;
                }
            }


            volgendSchot = Time.time + kogelRate / 1000;
            Kogels newKogels = Instantiate(kogel, uitgang.position, uitgang.rotation) as Kogels;
            newKogels.SetSpeed(uitgangSpeed);
        }
    }

    public void TrekkerVast()
    {
        Schiet();
        trekkerLos = false;
    }
    public void TrekkerLos()
    {
        trekkerLos = true;
        schotenOverInBurst = burstTel;
    }   // bron: https://answers.unity.com/questions/227807/single-shot-to-full-auto-and-vice-versa.html
}
