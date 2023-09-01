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
    //float shootAfter;
    float chaseFreq;

    bool onEdge = false;

    public float EnemySpeed { get => enemySpeed; set => enemySpeed = value; }
    public Vector3 Endpoint { get => endpoint; set => endpoint = value; }
    public int Loc { get => loc; set => loc = value; }
    //public float ShootAfter { get => shootAfter; set => shootAfter = value; }
    public float ShootFreqMin { get => shootFreqMin; set => shootFreqMin = value; }
    public float ShootFreqMax { get => shootFreqMax; set => shootFreqMax = value; }
    public float ChaseFreq { get => chaseFreq; set => chaseFreq = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Enemy";
        float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        Invoke("Shoot", randomTime);
    }

    void Shoot()
    {
        //if (onEdge) return;
        BulletController.bc.NewBullets(gameObject.transform.position, TempestController.tc.GetMid(TempestController.tc.EndLanes[loc]), false);
        TempestController.tc.rnd.NextDouble();
        //float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        //Invoke("Shoot", randomTime);
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
        GameObject pad = TempestController.tc.PlayerLanes[loc];
        Vector3 v = TempestController.tc.GetMid(pad);
        gameObject.transform.position = v;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerBullet")
        {
            Debug.Log("Shoot Enemy!");
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject); // enemy defeated
            AudioManager.Instance.PlaySFX("enemy_explode");
            TempestController.tc.Score += TempestController.tc.shootEnemyScore;
            TempestController.tc.SetScore();
        }
        else if(other.tag == "Tempest")
        {
            Debug.Log("Catch Tempest!");
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject); // enemy disappear
        }
    }
}
