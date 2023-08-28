using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float enemySpeed;
    Vector3 endpoint;
    int loc;
    float shootFreqMin;
    float shootFreqMax;
    float shootAfter;
    float chaseFreq;

    bool onEdge = false;

    public float EnemySpeed { get => enemySpeed; set => enemySpeed = value; }
    public Vector3 Endpoint { get => endpoint; set => endpoint = value; }
    public int Loc { get => loc; set => loc = value; }
    public float ShootAfter { get => shootAfter; set => shootAfter = value; }
    public float ShootFreqMin { get => shootFreqMin; set => shootFreqMin = value; }
    public float ShootFreqMax { get => shootFreqMax; set => shootFreqMax = value; }
    public float ChaseFreq { get => chaseFreq; set => chaseFreq = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Enemy";
        Invoke("Shoot", shootAfter);
    }

    void Shoot()
    {
        if (onEdge) { return; }
        BulletController.bc.NewBullets(gameObject.transform.position, endpoint, false);
        TempestController.tc.rnd.NextDouble();
        float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        Invoke("Shoot", randomTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (onEdge) { return; }
        Vector3 pos = gameObject.transform.position;
        float ratio = Time.deltaTime * enemySpeed / Vector3.Distance(pos, endpoint);
        if (ratio >= 1) // reach the end
        {
            gameObject.transform.position = endpoint; // directly show at the end point
            EnemyController.ec.Enemies[loc].Add(gameObject);
            onEdge = true;
            
            Invoke("Chase", chaseFreq);
        }
        else // move enemy
        {
            pos.x += ratio * (endpoint.x - pos.x);
            pos.y += ratio * (endpoint.y - pos.y);
            gameObject.transform.position = pos;
        }
    }

    void Chase()
    {
        EnemyController.ec.Enemies[loc].Remove(gameObject);
        if(TempestController.tc.Loc < loc) { loc--; } 
        else if(TempestController.tc.Loc > loc) { loc++; }
        else { Debug.Log("Should Trigger"); }
        MoveOnLanes();
        EnemyController.ec.Enemies[loc].Add(gameObject);
        Invoke("Chase", chaseFreq);
    }

    void MoveOnLanes()
    {
        GameObject pad = TempestController.tc.Lanes[loc];
        Vector3 v0 = pad.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 v1 = pad.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 v = (v0 + v1) * 0.5f;
        gameObject.transform.position = v;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerBullet")
        {
            Destroy(gameObject); // enemy defeated
            TempestController.tc.Score += EnemyController.ec.defeatedScore; // get score
        }
        else if(other.tag == "Tempest")
        {
            Destroy(gameObject); // enemy disappear
        }
    }
}
