using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Raak(float damage, Vector3 raakPunt, Vector3 raakRichting);
    void Damage(float damage);

}
