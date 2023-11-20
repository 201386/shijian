using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class enemy : shanghai
{
    public enum state
    {
        Idle,//待机
        chasing,//追逐
        Attack//攻击
    }

    NavMeshAgent agent;
     Transform target;
    float attackDistanceThreshold=0.5f;//攻击距离阈值
    float nextAttackTime;//攻击时间间隔中间数
    float timebetweenAttacks=1f;//攻击时间间隔
    state currentState;
    float myCollisonRadius;//敌人碰撞器半径
    float tagrgetCollisonRadius;//玩家碰撞器半径
    Material material;
    Color color;
    bool hasTarget;
    shanghai targetenity;
    float damage =1;//每次打到扣1滴血
    protected override void Start()
    {
     
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<Renderer>().material;
        color = material.color;
        if (GameObject.Find("Player") != null)
        {
            hasTarget = true;
            target = GameObject.Find("Player").transform;
            targetenity = target.GetComponent<shanghai>();
            targetenity.onDath += OntargetDath;
            StartCoroutine(updatepath(0.25f, new Vector3()));
            myCollisonRadius = GetComponent<CapsuleCollider>().radius;
            tagrgetCollisonRadius = target.GetComponent<CapsuleCollider>().radius;
            currentState = state.chasing;
        }
       
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float distance = (target.position - transform.position).sqrMagnitude;
                if (distance < Mathf.Pow(attackDistanceThreshold + myCollisonRadius + tagrgetCollisonRadius, 2))
                {
                    nextAttackTime = Time.time + timebetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }
    #region 攻击动画
    IEnumerator Attack()
    {
        material.color = Color.red;
        currentState = state.Attack;
        agent.enabled = false;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 originalposition = transform.position;
        Vector3 attackposition = target.position - dirToTarget * (myCollisonRadius);
        float percent = 0;
        float attackspeed = 3;
        bool hasAppliedDamage=false;//伤害判断避免重复受到伤害
        while (percent <= 1)
        {
            if (percent > 0.5&&!hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetenity.GetComponent<Rigidbody>().AddForce(dirToTarget * 2f, ForceMode.Impulse);//玩家被攻击后形成被击退的效果
                targetenity.TashDamage(damage);
            }
            percent += Time.deltaTime * attackspeed;
            float t = 4 * (-Mathf.Pow(percent, 2) + percent);
            print(t);
            transform.position = Vector3.Lerp(originalposition, attackposition, t);
            yield return null;
        }
        material.color = color;
        currentState = state.chasing;
        agent.enabled = true;
    }
    #endregion
    #region 导航寻路
    IEnumerator updatepath(float refreshRate,Vector3 targeposition)
    {
        while (hasTarget)
        {
            if (currentState == state.chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                targeposition = target.position-dirToTarget*(myCollisonRadius+ tagrgetCollisonRadius+attackDistanceThreshold/2);
                if (!dead)
                    agent.SetDestination(targeposition);
            }
            yield return new WaitForSeconds(refreshRate);
         
        }
    }
    #endregion
    #region 目标死亡方法
    void OntargetDath()
    {
        hasTarget = false;
        currentState = state.Idle;
    }

    #endregion
}
