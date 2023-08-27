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
    public int defeatedScore = 1; // the score gained by player if the enemy is defeated
    public float shootFreqMin = 0.3f;
    public float shootFreqMax = 0.3f;
    public float shootAfter = 0.2f;
    public static EnemyController ec;
    List<List<GameObject>> enemies = new List<List<GameObject>>() { new List<GameObject>(), new List<GameObject>(), new List<GameObject>()};

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
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("NewEnemy", generateAfter, generateFreq);
    }

    void NewEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, TempestController.tc.endpoint, Quaternion.identity);
        int loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        loc = TempestController.tc.rnd.Next(0, TempestController.tc.MaxLoc + 1);
        GameObject destLane = TempestController.tc.lanes[loc];
        Vector3 v0 = destLane.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 v1 = destLane.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 v = (v0 + v1) * 0.5f;
        enemy.transform.GetChild(0).GetComponent<Enemy>().EnemySpeed = enemySpeed;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Endpoint = v;
        enemy.transform.GetChild(0).GetComponent<Enemy>().Loc = loc;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMin = shootFreqMin;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ShootFreqMax = shootFreqMax;
        enemy.transform.GetChild(0).GetComponent<Enemy>().ShootAfter = shootAfter;

    }

    // Update is called once per frame
    void Update()
    {

    }

}
