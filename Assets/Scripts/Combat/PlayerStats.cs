using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    //this class is meant to provide public access to player stats like speed, speedAttack, baseDmg etc.
    //for every other class that might refer to it, if any of such stat is going to be shared between scripts, 
    //it should be stored and updated in this class (in future for smooth updating implement Observer pattern,
    //where each class might subscribe to this one)

    private UnityEvent UpdateStats;

    //unit is Hz - how many times per second player can strike
    [SerializeField]
    private float attackSpeed = 1.0f;

    public float AttackSpeed
    {
        get { return attackSpeed; }
        set {
            attackSpeed = value;
            UpdateStats.Invoke();
        } 
    }

    [SerializeField]
    private float movementSpeed = 5.0f;
    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            movementSpeed = value;
            UpdateStats.Invoke();
        }
    }

    public void Subscribe(UnityAction call)
    {
        if(UpdateStats == null)
        {
            UpdateStats = new UnityEvent();
        }

        UpdateStats.AddListener(call); 
    }

}
