using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour

{
    public GameObject enemyPrefab;
    public GameObject shootEnemyPrefab;
    public float generateAfter = 3.0f;
    public float generateFreq = 3.0f; // enemy generate frequency
    public float shootEnemyGenerateAfter = 10.0f;
    public float shootEnemyProb = 0.3f;
    public float enemySpeed = 1.2f; // enemy moving speed
    public float shootFreqMin = 0.3f;
    public float shootFreqMax = 0.3f;
    //public float shootAfter = 0.2f;
    public float chaseFreq = 1.0f;
    //public float enemyAccelerateFreq = 0.25f;
    public float enemyAcc = 0.06f;
    public float rotateSpeed = 10.0f;
    public static EnemyController ec;
    List<List<GameObject>> enemies = new List<List<GameObject>>();
    bool shootEnemy = false;

    public List<List<GameObject>> Enemies { get => enemies; set => enemies = value; }

    private void Awake()
    {

        if (ec == null)
        {
            DontDestroyOnLoad(gameObject);
            ec = this;
        }
        else if (ec != null)
        {
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < TempestController.tc.MaxLoc + 1; i++)
        {
            enemies.Add(new List<GameObject>());
        }
        ResetEnemyShoot();
        InvokeRepeating("NewEnemy", generateAfter, generateFreq);

    }

    public void ResetEnemyShoot()
    {
        shootEnemy = false;
        Invoke("setShootEnemyTrue", shootEnemyGenerateAfter);
    }

    void setShootEnemyTrue()
    {
        shootEnemy = true;
    }

    void NewEnemy()
    {
        
        int loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        
        GameObject startLane = TempestController.tc.StartLanes[loc];
        Vector3 startv = TempestController.tc.GetMid(startLane);

        GameObject destLane = TempestController.tc.PlayerLanes[loc];
        Vector3 v = TempestController.tc.GetMid(destLane);

        bool isShootEnemy = false;
        if (shootEnemy) // it is after a certain time
        {
            // there is probability to generate shoot enemy
            if (TempestController.tc.rnd.NextDouble() < shootEnemyProb) isShootEnemy = true;
        }

        GameObject enemy;
        if (isShootEnemy)
        {
            enemy = Instantiate(shootEnemyPrefab, startv, Quaternion.identity);
            enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMin = shootFreqMin;
            enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMax = shootFreqMax;
        }
        else
        {
            enemy = Instantiate(enemyPrefab, startv, Quaternion.identity);
            enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMin = 100;
            enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMax = 100;
        }
        

        enemy.transform.GetChild(0).GetComponent<Enemy>().EnemySpeed = enemySpeed;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Endpoint = v;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Loc = loc;
        //enemy.transform.GetChild(0).GetComponent<Enemy>().ShootAfter = shootAfter;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ChaseFreq = chaseFreq;
        enemy.transform.GetChild(0).GetComponent<Enemy>().EnemyAcc = enemyAcc;
        enemy.transform.GetChild(0).GetComponent<Enemy>().RotateSpeed = rotateSpeed;
        //enemy.transform.GetChild(0).GetComponent<Enemy>().AccelerateFreq = enemyAccelerateFreq;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
