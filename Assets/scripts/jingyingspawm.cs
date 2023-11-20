using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class jingyingspawm : MonoBehaviour
{
    shanghai player;
    Transform playerT;
    public mapgenerator map;
    public Wave[] waves;
    public player enemy;
    Wave currentwave;//��ǰ��
    int currentwavenumber;//��ǰ���������ڼ�¼��ǰ�ǵڼ������� ���ڸ��²���
    int enemiesremainingToSpawm;//���˵�ǰ������δ���ɵ�������
    float nextSpawmtime;//��ǰ��������һ���������ɵ�ʱ����
    int enemiesRemainingAlive;//��ǰ����������

    

    float tmeBetweenCampingChecks = 2;//��Ҵ���ʱ�䳬������
    float nextcampCheckTime;//��¼ʱ��
    float campThreholdDistance = 1.5f;//����
    Vector3 campPositionold;
    bool isCamping;
    bool isDisabled;
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
        playerT = player.transform;
        player.onDath += onPlayerDath;
        campPositionold = playerT.position;
        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextcampCheckTime)
            {
                nextcampCheckTime = Time.time + tmeBetweenCampingChecks;
                isCamping = (playerT.position - campPositionold).sqrMagnitude <= Mathf.Pow(campThreholdDistance, 2);
                campPositionold = playerT.position;


            }
            if (enemiesremainingToSpawm > 0 && Time.time > nextSpawmtime)
            {
                enemiesremainingToSpawm--;
                nextSpawmtime = Time.time + currentwave.timeBetweenspans;
                StartCoroutine(SpawnEnemy());
            }
        }

    }
    IEnumerator SpawnEnemy()//��������
    {
        Transform tileTransform = map.Getrandomtransform();
        if (isCamping)
            tileTransform = map.GetTileFornPosition(playerT.position);
        Material tilemat = tileTransform.GetComponent<Renderer>().material;
        float spawnDelay = 1;//����ʱ��
        float spawnTime = 0;//��¼ʱ��
        float tileFlashSpeed = 4;//��Ƭ��˸�ٶ�
        Color initalColor = tilemat.color;
        Color flashColor = Color.red;
        while (spawnTime < spawnDelay)
        {
            spawnTime += Time.deltaTime;
            tilemat.color = Color.Lerp(initalColor, flashColor, Mathf.PingPong(spawnTime * tileFlashSpeed, 1));
            yield return null;
        }
        player spawnEnemy = Instantiate(enemy, tileTransform.position + Vector3.up, Quaternion.identity);
        spawnEnemy.onDath += onEnemyDath;
    }

    void NextWave()
    {
        currentwavenumber++;
        if (currentwavenumber - 1 < waves.Length)
        {
            print("��ǰ�ǵ�" + currentwavenumber + "��");
            currentwave = waves[currentwavenumber - 1];
            enemiesremainingToSpawm = currentwave.enemycount;
            enemiesRemainingAlive = enemiesremainingToSpawm;
        }

    }
    void onEnemyDath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive <= 0)
        {
            NextWave();
        }
    }
    void onPlayerDath() => isDisabled = true;
    #region ������
    [Serializable]
    public class Wave
    {
        public int enemycount;//��������
        public float timeBetweenspans;//�������ɼ��
    }
    #endregion
}
