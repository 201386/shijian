using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class enemy : shanghai
{
    public enum state
    {
        Idle,//����
        chasing,//׷��
        Attack//����
    }

    NavMeshAgent agent;
     Transform target;
    float attackDistanceThreshold=0.5f;//����������ֵ
    float nextAttackTime;//����ʱ�����м���
    float timebetweenAttacks=1f;//����ʱ����
    state currentState;
    float myCollisonRadius;//������ײ���뾶
    float tagrgetCollisonRadius;//�����ײ���뾶
    Material material;
    Color color;
    bool hasTarget;
    shanghai targetenity;
    float damage =1;//ÿ�δ򵽿�1��Ѫ
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
    #region ��������
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
        bool hasAppliedDamage=false;//�˺��жϱ����ظ��ܵ��˺�
        while (percent <= 1)
        {
            if (percent > 0.5&&!hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetenity.GetComponent<Rigidbody>().AddForce(dirToTarget * 2f, ForceMode.Impulse);//��ұ��������γɱ����˵�Ч��
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
    #region ����Ѱ·
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
    #region Ŀ����������
    void OntargetDath()
    {
        hasTarget = false;
        currentState = state.Idle;
    }

    #endregion
}
