using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float xInput;
    private float yInput;

    public Rigidbody2D rb;

    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        GetComponent<PlayerStats>().Subscribe(() => { speed = GetComponent<PlayerStats>().MovementSpeed; });
        speed = GetComponent<PlayerStats>().MovementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(xInput * speed, yInput * speed);
        
    }
}
