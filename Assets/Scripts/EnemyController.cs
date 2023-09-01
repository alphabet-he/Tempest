using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour

{
    public GameObject enemyPrefab;
    public float generateAfter = 3.0f;
    public float generateFreq = 3.0f; // enemy generate frequency
    public float enemySpeed = 1.2f; // enemy moving speed
    public float shootFreqMin = 0.3f;
    public float shootFreqMax = 0.3f;
    //public float shootAfter = 0.2f;
    public float chaseFreq = 1.0f;
    public static EnemyController ec;
    List<List<GameObject>> enemies = new List<List<GameObject>>();

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
        InvokeRepeating("NewEnemy", generateAfter, generateFreq);
    }

    void NewEnemy()
    {
        
        int loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        
        GameObject startLane = TempestController.tc.StartLanes[loc];
        Vector3 startv = TempestController.tc.GetMid(startLane);

        GameObject destLane = TempestController.tc.PlayerLanes[loc];
        Vector3 v = TempestController.tc.GetMid(destLane);
        

        GameObject enemy = Instantiate(enemyPrefab, startv, Quaternion.identity);

        enemy.transform.GetChild(0).GetComponent<Enemy>().EnemySpeed = enemySpeed;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Endpoint = v;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Loc = loc;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMin = shootFreqMin;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMax = shootFreqMax;
        //enemy.transform.GetChild(0).GetComponent<Enemy>().ShootAfter = shootAfter;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ChaseFreq = chaseFreq;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
