using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    private float attackSpeed;
    private float t = 0;
    private float T;

    private float mouseAngle;
    private float angle;


    [SerializeField]
    public float radius = 1;

    [SerializeField, Range(0, Mathf.PI * 2)]
    public float SwingArc = Mathf.PI;

    [SerializeField, Tooltip("Should mouse be followed for whole duration of swing, or determined at beginning")]
    public bool DynamicMouseFollow = false;

    [SerializeField, Range(-180, 180)]
    public int SpriteAngleOffset = -135;


    public void Start()
    {
        //this should not get updated mid animation, hence no subscribing to PlayerStats event
        attackSpeed = GetComponentInParent<PlayerStats>().AttackSpeed;
        T = 1 / attackSpeed;
        StartCoroutine(EndAmination(T));

        Vector3 mousePos = Input.mousePosition;
        Vector3 center = Camera.main.WorldToScreenPoint(transform.parent.position);
        mousePos.x = mousePos.x - center.x;
        mousePos.y = mousePos.y - center.y;
        mouseAngle = Mathf.Atan2(mousePos.y, mousePos.x);
    }

    public void Update()
    {
        t += (Time.deltaTime / T) * SwingArc;

        if(DynamicMouseFollow)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 center = Camera.main.WorldToScreenPoint(transform.parent.position);
            mousePos.x = mousePos.x - center.x;
            mousePos.y = mousePos.y - center.y;
            mouseAngle = Mathf.Atan2(mousePos.y, mousePos.x);
        }

        //TODO: some math is wrong here, bcs this only works correctly for SwingArc ~ 4.2
        angle = t + mouseAngle + SwingArc / 2;


        if (Camera.main.ScreenToWorldPoint(transform.parent.position).x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(180f, 0f, -angle * Mathf.Rad2Deg + SpriteAngleOffset));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle * Mathf.Rad2Deg + SpriteAngleOffset));
        }


        //Debug.Log(angle);
        //Debug.Log((-Mathf.Sin(t) * radius).ToString() + ", " + (Mathf.Cos(t) * radius).ToString());
        //Debug.Log(parentTransform.position);
        transform.position = new Vector3(transform.parent.position.x + (- Mathf.Sin(angle) * radius), transform.parent.position.y + (Mathf.Cos(angle) * radius), transform.position.z);


    }

    private IEnumerator EndAmination(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

}
