using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shell : MonoBehaviour
{
    public float explosionRadius;//�뾶 
    public LayerMask damageMask;//�˺���
    public float damage=20;//�˺�
    public AudioSource explosionAudioSource;//��Ƶ
    public ParticleSystem explosionEffect;//�ӵ���ЧЧ��


    public bool isRotate = false;

    private void Start()
    {
        Destroy(gameObject, 2);//δ����Ŀ��ʱ��5������
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
