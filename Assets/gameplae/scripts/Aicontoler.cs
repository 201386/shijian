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

        InvokeRepeating("FireControl", 1, 3);//�ڵȴ�1s֮���ٵ��÷���FireControl����������ÿ��3s��ȥ���á�
    }


    void FireControl()
    {
        character.Attack();
    }
    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;//����Ѳ·
        transform.LookAt(player.transform); 
    }
}
