using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))] // belangrijk
//[RequireComponent (typeof(WapenController))]
public class Player : Wezens {

    public float moveSpeed = 5;
    PlayerController controller;
    Camera viewCamera; // referentie naar de main camera
    WapenController wapenController;

    public Transform crosshairs;

	protected override void Start () {         // moet dit protected zijn?
        base.Start();
	}

    void Awake() // Moet eerder geroepen worden dan Spawner.start, de eerste wave moet een wapen worden toegewezen
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main; // uitzoeken wat hier mis mee gaat
        wapenController = GetComponent<WapenController>();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveLevel)
    {
        health = startHealth;
        wapenController.EquipWapen(waveLevel - 1); // Object reference error → static field
    }

	void Update () {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw ("Vertical")); 
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // geeft een ray terug vanuit de viewCamera door de muis positie
        Plane vloerPlane = new Plane(Vector3.up, Vector3.up * wapenController.WapenHoogte);
        float rayAfstand; // nog toewijzen

        if (vloerPlane.Raycast(ray, out rayAfstand))
        {
            Vector3 point = ray.GetPoint(rayAfstand);
            Debug.DrawLine(ray.origin,point, Color.red);
            controller.LookAt(point); // nog toewijzen
            crosshairs.position = point;
        }

        if (Input.GetMouseButton(0))
        {
            wapenController.TrekkerVast();
        }
        if (Input.GetMouseButtonUp(0))
        {
            wapenController.TrekkerLos();
        }

        if (transform.position.y < -1)
        {
            Damage(health);
        }
    }
}
