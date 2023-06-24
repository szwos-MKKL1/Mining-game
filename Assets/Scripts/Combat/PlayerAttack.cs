using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //TODO: instead take this from Inventory.CurrentWeapon or something like that
    public GameObject WeaponStrikePrefab;

    private bool isStriking;
    private float attackSpeed;

    private void Start()
    {
        attackSpeed = GetComponent<PlayerStats>().AttackSpeed;
        GetComponent<PlayerStats>().Subscribe(() => { attackSpeed = GetComponent<PlayerStats>().AttackSpeed; });
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            if(!isStriking)
            {
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
