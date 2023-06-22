using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //TODO: instead take this from Inventory.CurrentWeapon or something like that
    public GameObject WeaponStrikePrefab;

    private bool isStriking; //TODO: if logic gets longer than ~100 lines of code, re-implement this using State Machine
    private float attackSpeed;

    private void Awake()
    {
        //TODO: handle this via Observer pattern for dynamic updates
        attackSpeed = GetComponentInParent<PlayerStats>().AttackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            if(!isStriking)
            {
                //TODO: instantiate in correct position
                Instantiate(WeaponStrikePrefab, transform);
                StartCoroutine(strikeCooldown());

            }
        }
    }

    private IEnumerator strikeCooldown()
    {
        isStriking = true;

        yield return new WaitForSeconds(1 / attackSpeed);

        isStriking = false;
    }
}
