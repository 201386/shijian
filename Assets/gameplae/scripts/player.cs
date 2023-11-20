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

    public Rigidbody shell;//弹药
    public Transform muzzle;
    public float launchFoece =0.1f;//子弹冲力

    public AudioSource shootAudioSource;//添加声音

    bool attacking = false;//判断是否在攻击中

    public float attackTime;//攻击时间

    float hp ;
    public float hpMAX = 100; 


    public Slider hpSlider;//血条
    public Image hpFillImage;//血条ui
    public Color hpColorFull = Color.green;//血条颜色
    public Color hpColorNull = Color.red;

    public ParticleSystem explosionEffect;
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        hp = hpMAX;
        RefreshHealthHLD();//刷新血条
    }
    public void Move(Vector3 v)
    {
        if (!isAlive) return;
        if (attacking) return;

        Vector3 movement = v * speed;
        cc.SimpleMove(movement);

        if (animator)
        {
            animator.SetFloat("Speed", cc.velocity.magnitude);//过渡条件
        }
    }  

    public void Attack()
    {
        if (!isAlive) return;
        if (attacking) return;


        var shellnstance = Instantiate(shell, muzzle.position,muzzle.rotation) as Rigidbody;
        shellnstance.velocity = launchFoece * muzzle.forward;//子弹向前飞
        if (animator)
        {
            animator.SetTrigger("Attack");//播放动画 
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
         
        gameObject.SetActive(false);//删除

    }
     
    public void TackDamage(float amount)
    {
        hp -= amount;
        RefreshHealthHLD();
        if (hp <= 0 && isAlive) { Death(); }
      
    }
    public void RefreshHealthHLD()//刷新血条
    {
        hpSlider.value = hp;
        hpFillImage.color = Color.Lerp(hpColorNull, hpColorFull, hp / hpMAX);//血条图片的颜色赋值为颜色的插值

    }
}
