using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Aicontoler : MonoBehaviour
{
    NavMeshAgent agent;
    player character;
    Transform player;

    void Start()
    { 
        character = GetComponent<player>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;

        InvokeRepeating("FireControl", 1, 3);//在等待1s之后，再调用方法FireControl方法，并且每隔3s再去调用。
    }


    void FireControl()
    {
        character.Attack();
    }
    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;//导航巡路
        transform.LookAt(player.transform); 
    }
}
