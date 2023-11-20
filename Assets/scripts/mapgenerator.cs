using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapgenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    Map currentMap;
    public Transform obsGameobject;
    public float Tilesize;

    public Transform Quad;

    public Transform tilePresf;
    public Transform mapHolder;
    public Transform obsPresf;
   [Range(0, 1)]
    public float outlinePercent;
  
 
     List<Coord> coordslist;
    Queue<Coord> coordQueue;
    public Vector2 mapmaxSize;
    Transform[,] tilemap;
    Queue<Coord> queueTilemap;
    public void GenerateMap()
    {
      
        currentMap = maps[mapIndex];
        tilemap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random rand = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x, 1, currentMap.mapSize.y);
          coordslist = new List<Coord>();
        List<Coord> tileMaplist = new List<Coord>(); 
        if (mapHolder != null)
        {
            DestroyImmediate(mapHolder.gameObject);
            mapHolder = new GameObject().transform;
            mapHolder.parent = transform;
        }
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Transform newtile = Instantiate(tilePresf);
                newtile.position = new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f+ y + 0.5f);
                newtile.parent = mapHolder; 
                newtile.localScale *= (1-outlinePercent)* Tilesize;
                coordslist.Add(new Coord(x, y));
                tileMaplist.Add(new Coord(x, y));
                tilemap[x, y] = newtile.transform;

            }
        }//������Ƭ
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obsPercent);//�ϰ�������
        int currntObstacaleCount = 0;//��ǰ�ϰ�������
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];//���ڼ�¼�����ǿ�ͨ������
        coordQueue = new Queue<Coord>(utilities.Shuffle<Coord>(coordslist.ToArray(), currentMap.seed));//����List����Ԫ��
        for (int i = 0; i < obstacleCount; i++)
        {
            currntObstacaleCount++;
           
            Coord tile = getRandomCoord();//�õ�һ���������ҵ�Ԫ��
            obstacleMap[tile.x, tile.y] = true;//���費��ͬ��
            if (tile != currentMap.mapCenter && MapIsfullyAccessble(obstacleMap,currntObstacaleCount))
            {
                float Height = Mathf.Lerp(currentMap.minObsHeight, currentMap.maxObsHeight, (float)rand.NextDouble());
                Transform newobs = Instantiate(obsPresf);
                newobs.position = new Vector3(-currentMap.mapSize.x / 2f + 0.5f + tile.x, Height/2, -currentMap.mapSize.y / 2f + tile.y + 0.5f)*Tilesize;
                newobs.localScale *= (1 - outlinePercent)* Tilesize;
                newobs.localScale = new Vector3(newobs.localScale.x, Height, newobs.localScale.z);
                newobs.parent = mapHolder;
                float obsPercent = tile.y / (float)currentMap.mapSize.y;
                Renderer obsRendeerer = newobs.GetComponent<Renderer>();
                Material obsMaterial = new Material(obsRendeerer.sharedMaterial);
                obsMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, obsPercent);
                obsRendeerer.sharedMaterial = obsMaterial;
                tileMaplist.Remove(tile);

            }
            else
            {
                currntObstacaleCount--;
                obstacleMap[tile.x, tile.y] = false;
            }
        }
        queueTilemap = new Queue<Coord>(utilities.Shuffle<Coord>(tileMaplist.ToArray(), currentMap.seed));
        Quad.localScale = new Vector3(mapmaxSize.x, mapmaxSize.y, 1) * Tilesize;
         
        Transform navMeshobsfoward=Instantiate(obsGameobject,Vector3.forward*((mapmaxSize.y+currentMap.mapSize.y)/4f)*Tilesize,Quaternion.identity);
        navMeshobsfoward.localScale = new Vector3(currentMap.mapSize.x, 0.05f, mapmaxSize.y / 2f - currentMap.mapSize.y / 2f) * Tilesize;
        navMeshobsfoward.parent = mapHolder;

        Transform navMeshobsback = Instantiate(obsGameobject, Vector3.back * ((mapmaxSize.y + currentMap.mapSize.y) / 4f) * Tilesize, Quaternion.identity);
        navMeshobsback.localScale = new Vector3(currentMap.mapSize.x, 0.05f, mapmaxSize.y / 2f - currentMap.mapSize.y / 2f) * Tilesize;
        navMeshobsback.parent = mapHolder;
     
        Transform navMeshobsLeft = Instantiate(obsGameobject, Vector3.left * ((mapmaxSize.x+ currentMap.mapSize.x) / 4f) * Tilesize, Quaternion.identity);
        navMeshobsLeft.localScale = new Vector3(mapmaxSize.x/2f-currentMap.mapSize.x/2f, 0.05f, currentMap.mapSize.y ) * Tilesize;
        navMeshobsLeft.parent = mapHolder;

        Transform navMeshobsRight = Instantiate(obsGameobject, Vector3.right * ((mapmaxSize.x + currentMap.mapSize.x) / 4f) * Tilesize, Quaternion.identity);
        navMeshobsRight.localScale = new Vector3(mapmaxSize.x / 2f - currentMap.mapSize.x / 2f, 0.05f, currentMap.mapSize.y) * Tilesize;
        navMeshobsRight.parent = mapHolder;


    }

    Coord getRandomCoord()
    {
        Coord coord = coordQueue.Dequeue();
        coordQueue.Enqueue(coord);
        return coord;
    }
    public Transform Getrandomtransform()
    {
        Coord coord = queueTilemap.Dequeue();
        queueTilemap.Enqueue(coord);
        return tilemap[coord.x, coord.y];
    }
    public Transform GetTileFornPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / Tilesize + (currentMap.mapSize.x - 1) / 2);
        int y = Mathf.RoundToInt(position.z / Tilesize + (currentMap.mapSize.y - 1 )/ 2);
        x = Mathf.Clamp(x, 0, currentMap.mapSize.x - 1);
        y = Mathf.Clamp(y, 0, currentMap.mapSize.y - 1);
        return tilemap[x, y];
    }
    bool MapIsfullyAccessble(bool[,] obstaqleMap, int currentObstacleCount)
    {
        bool[,] mapLogs = new bool[obstaqleMap.GetLength(0), obstaqleMap.GetLength(1)];
        mapLogs[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;//һ��ʼ�Ͱ����ĵ���Ϊ�Ѽ���
        int accessibleTilecount = 1;//��ͨ������
        Queue<Coord> queue = new Queue<Coord>();//���ڼ�¼����������Ƭ
        queue.Enqueue(new Coord(currentMap.mapCenter.x, currentMap.mapCenter.y));
        while (queue.Count > 0)
        {
            Coord coord = queue.Dequeue();
            for(int x =-1; x <2; x++)
            {
                for (int y =-1; y < 2; y++)
                { int neighbourX = coord.x + x;
                    int neighbourY = coord.y + y;
                    if (x == 0 || y == 0)
                    {
                        //�жϵ�ͼ�߽磬��֤��������Χ
                        if (neighbourX >= 0 && neighbourX < currentMap.mapSize.x && neighbourY >= 0 && neighbourY < currentMap.mapSize.y) 
                        {
                            //�ж��Ƿ�һ����
                            if (!mapLogs[neighbourX, neighbourY] && !obstaqleMap[neighbourX, neighbourY])
                            {
                                queue.Enqueue(new Coord(neighbourX, neighbourY));//������п�ʼ��һ�ֱ���
                                mapLogs[neighbourX, neighbourY] = true;//���Ϊ�Ѿ�����
                                accessibleTilecount++;//��ͨ������++
                            }
                        }
                    }
                }
            }
        }
        int s= (int)(currentMap.mapSize.x * currentMap.mapSize.y) - currentObstacleCount;//�õ�ʵ�ʿ�ͨ�����������

        return accessibleTilecount==s;

    }
    private void Start()
    {
        GenerateMap();
    }
}


[System.Serializable]//���л�
public struct Coord
{
    public int x;
    public int y;
    public Coord(int x,int y)
    {
        this.x = x;
        this.y = y;
    }
    public static bool operator !=(Coord c1,Coord c2)
    {
        return !(c1 == c2); 
    }
    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.x==c2.x&& c1.y == c2.y;
    }
}
 
[System.Serializable]
public  class Map
{
    public int seed;
    public Coord mapSize;
    [Range(0, 1)]
    public float obsPercent;
    public float minObsHeight,maxObsHeight;
    public Color foregroundColor, backgroundColor;
    public Coord mapCenter
    {
        get => new Coord(mapSize.x / 2, mapSize.y / 2);
    }

}

