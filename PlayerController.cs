using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidbody;

	void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();
	}

    public void LookAt(Vector3 Lookpoint)
    {
        Vector3 HoogteCorrect = new Vector3(Lookpoint.x, transform.position.y, Lookpoint.z); // player roteerd voorover bij kijken
        transform.LookAt(HoogteCorrect); // Lookpoint 
    }

	public void Move (Vector3 _velocity)
    {
        velocity = _velocity;
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}
