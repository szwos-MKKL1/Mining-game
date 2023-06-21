using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //this class is meant to provide public access to player stats like speed, speedAttack, baseDmg etc.
    //for every other class that might refer to it, if any of such stat is going to be shared between scripts, 
    //it should be stored and updated in this class (in future for smooth updating implement Observer pattern,
    //where each class might subscribe to this one)

    //unit is Hz - how many times per second player can strike
    [SerializeField]
    public float attackSpeed = 1.0f;

    public float AttackSpeed
    {
        get { return attackSpeed; }
        set { attackSpeed = value; } //TODO: exposing public setter is not desired here, leaving it now just for testing purposes
    }
    

}
