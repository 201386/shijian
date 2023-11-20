using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shell : MonoBehaviour
{
    public float explosionRadius;//半径 
    public LayerMask damageMask;//伤害层
    public float damage=20;//伤害
    public AudioSource explosionAudioSource;//音频
    public ParticleSystem explosionEffect;//子弹特效效果


    public bool isRotate = false;

    private void Start()
    {
        Destroy(gameObject, 2);//未击中目标时候，5秒后清除
    if(isRotate)
        {
            GetComponent<Rigidbody>().AddTorque(transform.right * 1000);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var colliders=Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<player>();
            if (target)
            {
                target.TackDamage(damage);
            }
        }
        explosionAudioSource.Play();
        explosionEffect.transform.parent = null;
        explosionEffect.Play();


        ParticleSystem.MainModule mainmodule = explosionEffect.main;
        Destroy(explosionEffect.gameObject, mainmodule.duration);
        Destroy(gameObject);
    }

}
