using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MasterResources
{
    public GameObject wood;
    public int numberOfWood;
    public int min, max;
    public float interpolationPeriod = 0.1f;

    private void Start()
    {
        PlaceCubes();
        Hp();
    }


    //This is the Hp of wood;
    public override void Hp()
    {
        health = 120;
    }

    //this places wood at random places.
    void PlaceCubes()
    {
        for (int i = 0; i < numberOfWood; i++)
        {
            Instantiate(wood, GeneratedPosition(), Quaternion.identity);
        }
    }

    //this generates the places for the wood.
    Vector3 GeneratedPosition()
    {
        int x, y, z;
        x = Random.Range(min, max);
        y = 0;
        z = Random.Range(min, max);
        return new Vector3(x, y, z);
    }

    //when is activates -10 Hp goes of health.
    public void Damage()
    {
        health -= 10;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
