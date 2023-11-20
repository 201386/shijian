using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class player : MonoBehaviour
{

    public event Action onDath;

    public float speed;
    CharacterController cc;

    Animator animator;
    bool isAlive = true;

   public float turnspeed;

    public Rigidbody shell;//��ҩ
    public Transform muzzle;
    public float launchFoece =0.1f;//�ӵ�����

    public AudioSource shootAudioSource;//�������

    bool attacking = false;//�ж��Ƿ��ڹ�����

    public float attackTime;//����ʱ��

    float hp ;
    public float hpMAX = 100; 


    public Slider hpSlider;//Ѫ��
    public Image hpFillImage;//Ѫ��ui
    public Color hpColorFull = Color.green;//Ѫ����ɫ
    public Color hpColorNull = Color.red;

    public ParticleSystem explosionEffect;
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        hp = hpMAX;
        RefreshHealthHLD();//ˢ��Ѫ��
    }
    public void Move(Vector3 v)
    {
        if (!isAlive) return;
        if (attacking) return;

        Vector3 movement = v * speed;
        cc.SimpleMove(movement);

        if (animator)
        {
            animator.SetFloat("Speed", cc.velocity.magnitude);//��������
        }
    }  

    public void Attack()
    {
        if (!isAlive) return;
        if (attacking) return;


        var shellnstance = Instantiate(shell, muzzle.position,muzzle.rotation) as Rigidbody;
        shellnstance.velocity = launchFoece * muzzle.forward;//�ӵ���ǰ��
        if (animator)
        {
            animator.SetTrigger("Attack");//���Ŷ��� 
        }
        attacking = true; 
        shootAudioSource.Play();

        Invoke("RefreshAttack", attackTime);

    } 
    void RefreshAttack()
    {
        attacking = false;

    }
    public void Rotate(Vector3 lookDir)
    {
        var targetpos = transform.position + lookDir;
        var charcterpos = transform.position;

        charcterpos.y = 0;
        targetpos.y = 0;

        var faceTotargetDir = targetpos - charcterpos;
        var faceToquat = Quaternion.LookRotation(faceTotargetDir);
        Quaternion slerp = Quaternion.Slerp(transform.rotation, faceToquat, turnspeed * Time.deltaTime);

        transform.rotation = slerp;
    }

    public void Death()
    {
        isAlive = false;

        explosionEffect.transform.parent = null;
        explosionEffect.gameObject.SetActive(true);


        onDath();

        ParticleSystem.MainModule mainmodule = explosionEffect.main;
        Destroy(explosionEffect.gameObject, mainmodule.duration);
         
        gameObject.SetActive(false);//ɾ��

    }
     
    public void TackDamage(float amount)
    {
        hp -= amount;
        RefreshHealthHLD();
        if (hp <= 0 && isAlive) { Death(); }
      
    }
    public void RefreshHealthHLD()//ˢ��Ѫ��
    {
        hpSlider.value = hp;
        hpFillImage.color = Color.Lerp(hpColorNull, hpColorFull, hp / hpMAX);//Ѫ��ͼƬ����ɫ��ֵΪ��ɫ�Ĳ�ֵ

    }
}
