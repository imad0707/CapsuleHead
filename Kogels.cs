using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kogels : MonoBehaviour {

    public LayerMask collisionMask;
    float speed = 10;
    float damage = 1;
    float Duur = 3;
    
    void Start()
    {
        Destroy(gameObject, Duur);

        Collider[] eersteBotsing = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (eersteBotsing.Length > 0)
        {
            HitObject(eersteBotsing[0], transform.position);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

	void Update () {
        float moveDistance = speed * Time.deltaTime;
        Checkbotsing (moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    void Checkbotsing (float moveDistance) // bron: https://www.youtube.com/watch?v=xcHNvWtPq6g
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide)) // QertyTriggerInteraction geeft error een teug
        {
            HitObject(hit.collider, hit.point);
        }
    }

    //void  HitObject(RaycastHit hit)  //informatie voor het object
    //{
    //    IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
    //    if (damageableObject != null)
    //    {
    //        damageableObject.Raak(damage, hit);
    //    }
    //    GameObject.Destroy(gameObject);
    //}

    void HitObject(Collider c, Vector3 raakPunt)  //informatie voor het object
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.Raak(damage, raakPunt, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
