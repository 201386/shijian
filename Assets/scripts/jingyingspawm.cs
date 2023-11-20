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
    Wave currentwave;//当前波
    int currentwavenumber;//当前波数，用于记录当前是第几波敌人 用于更新波数
    int enemiesremainingToSpawm;//敌人当前波数，未生成敌人数量
    float nextSpawmtime;//当前波数内下一个敌人生成的时间间隔
    int enemiesRemainingAlive;//当前存活敌人数量

    

    float tmeBetweenCampingChecks = 2;//玩家带的时间超过两秒
    float nextcampCheckTime;//记录时间
    float campThreholdDistance = 1.5f;//距离
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
    IEnumerator SpawnEnemy()//敌人生成
    {
        Transform tileTransform = map.Getrandomtransform();
        if (isCamping)
            tileTransform = map.GetTileFornPosition(playerT.position);
        Material tilemat = tileTransform.GetComponent<Renderer>().material;
        float spawnDelay = 1;//生成时间
        float spawnTime = 0;//记录时间
        float tileFlashSpeed = 4;//瓦片闪烁速度
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
            print("当前是第" + currentwavenumber + "波");
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
    #region 波数类
    [Serializable]
    public class Wave
    {
        public int enemycount;//敌人数量
        public float timeBetweenspans;//敌人生成间隔
    }
    #endregion
}
