using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]//½ÇÉ«¿ØÖÆÆ÷½Å±¾

public class Playercontroler : MonoBehaviour
{
    Rigidbody rig;
    Vector3 velocity;
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rig.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
    }
    public void Move(Vector3 velocity)
    {
        this.velocity = velocity; 
    }
}
