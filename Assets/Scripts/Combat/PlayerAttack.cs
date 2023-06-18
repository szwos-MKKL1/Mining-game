using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //TODO: instead take this from Inventory.CurrentWeapon or something like that
    public GameObject WeaponStrikePrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //TODO: instantiate in correct position
            Instantiate(WeaponStrikePrefab, transform);

        }
    }
}
