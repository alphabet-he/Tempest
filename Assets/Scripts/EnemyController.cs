using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour

{
    public GameObject enemyPrefab;
    public float enemyGenerateAfter = 3.0f;
    public float enemyGenerateFreq = 3.0f; // enemy generate frequency
    public float enemySpeed = 1.2f; // enemy moving speed
    public static EnemyController ec;
    List<Tuple<GameObject, Vector3>> enemies = new List<Tuple<GameObject, Vector3>>(); // bullet object - destination


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
        InvokeRepeating("NewEnemy", enemyGenerateAfter, enemyGenerateFreq);
    }

    void NewEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, TempestController.tp.endpoint, Quaternion.identity);
        int loc = TempestController.tp.rnd.Next(0, TempestController.tp.MaxLoc + 1);
        loc = TempestController.tp.rnd.Next(0, TempestController.tp.MaxLoc + 1);
        GameObject destLane = TempestController.tp.lanes[loc];
        Vector3 v0 = destLane.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 v1 = destLane.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 v = (v0 + v1) * 0.5f;
        enemies.Add(new Tuple<GameObject, Vector3>(enemy, v)); // enemy moves towards one lane
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count > 0)
        {
            foreach (var enemy in enemies) //for each bullet, move
            {
                Vector3 pos = enemy.Item1.transform.position;
                float ratio = Time.deltaTime * enemySpeed / Vector3.Distance(pos, enemy.Item2);
                if (ratio >= 1) // reach the end
                {
                    enemy.Item1.transform.position = enemy.Item2; // directly show at the end point
                }
                else // move bullet
                {
                    pos.x += ratio * (enemy.Item2.x - pos.x);
                    pos.y += ratio * (enemy.Item2.y - pos.y);
                    enemy.Item1.transform.position = pos;
                }

            }
        }
    }
}