using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WapenController : MonoBehaviour {

    public Transform handvat;
    Wapen equippedWapen;
    //public Wapen startWapen;
    public Wapen[] alleWapens;

    void Start()
    {
        
        //if (startWapen != null)
        //{
        //    EquipWapen(startWapen);
        //}
    }

	public void EquipWapen(Wapen wapen_Equip)
    {
        if (equippedWapen != null)
        {
            Destroy(equippedWapen.gameObject);
        }
        equippedWapen = Instantiate(wapen_Equip, handvat.position, handvat.rotation) as Wapen;
        equippedWapen.transform.parent = handvat;
    }

    public void EquipWapen(int wapenIndex)  // aparte versie voor de OnNewWave method
    {
        EquipWapen(alleWapens[wapenIndex]);
    }
	
    public void TrekkerVast()
    {
        if(equippedWapen != null)
        {
            equippedWapen.TrekkerVast();
        }
    }
    public void TrekkerLos()
    {
        if (equippedWapen != null)
        {
            equippedWapen.TrekkerLos();
        }
    }
    public float WapenHoogte
    {
        get
        {
            return handvat.position.y;
        }
    }
}
