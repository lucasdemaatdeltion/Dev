using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MasterResources : MonoBehaviour
{
    //these are the variables for the recources.
    public string color;
    public int health;
    public int damageInflicted;

    public virtual void Hp()
    {
        health = 100;
    }

    public virtual void Attack()
    {
        health -= damageInflicted;
    }
}
