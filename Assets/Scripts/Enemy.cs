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
    float accelerateFreq;

    bool onEdge = false;

    public float EnemySpeed { get => enemySpeed; set => enemySpeed = value; }
    public Vector3 Endpoint { get => endpoint; set => endpoint = value; }
    public int Loc { get => loc; set => loc = value; }
    //public float ShootAfter { get => shootAfter; set => shootAfter = value; }
    public float ShootFreqMin { get => shootFreqMin; set => shootFreqMin = value; }
    public float ShootFreqMax { get => shootFreqMax; set => shootFreqMax = value; }
    public float ChaseFreq { get => chaseFreq; set => chaseFreq = value; }
    public float AccelerateFreq { get => accelerateFreq; set => accelerateFreq = value; }

    GameObject startLane;
    GameObject destLane;
    GameObject prefabParent;
    float destLaneLength;
    float totalTrip;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Enemy";
        startLane = TempestController.tc.StartLanes[loc];
        destLane = TempestController.tc.PlayerLanes[loc];
        destLaneLength = Vector3.Distance(
            destLane.GetComponent<LineRenderer>().GetPosition(0),
            destLane.GetComponent<LineRenderer>().GetPosition(1));
        totalTrip = Vector3.Distance(
            TempestController.tc.GetMid(startLane),
            TempestController.tc.GetMid(destLane));
        prefabParent = gameObject.transform.parent.gameObject;
        InitPos();
        float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        Invoke("Shoot", randomTime);
        Invoke("accelerate", 0);
    }

    void accelerate()
    {
        if (onEdge) return;
        enemySpeed = TempestController.tc.SpeedToScale(enemySpeed, startLane, destLane, prefabParent.transform.position);
        Invoke("accelerate", accelerateFreq);
    }

    void Shoot()
    {
        //if (onEdge) return;
        BulletController.bc.NewBullets(prefabParent.transform.position, TempestController.tc.GetMid(TempestController.tc.EndLanes[loc]), false);
        TempestController.tc.rnd.NextDouble();
        //float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        //Invoke("Shoot", randomTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (onEdge) { return; }
        Vector3 pos = prefabParent.transform.position;
        //Debug.Log(enemySpeed);
        float ratio = Time.deltaTime * enemySpeed / Vector3.Distance(pos, endpoint);
        if (ratio >= 1) // reach the end
        {
            prefabParent.transform.position = endpoint; // directly show at the end point
            prefabParent.transform.localScale = new Vector3(destLaneLength, destLaneLength, 1);

            EnemyController.ec.Enemies[loc].Add(gameObject);
            onEdge = true;
            
            Invoke("Chase", chaseFreq);
        }
        else // move enemy
        {
            pos.x += ratio * (endpoint.x - pos.x);
            pos.y += ratio * (endpoint.y - pos.y);
            prefabParent.transform.position = pos;
            float currTrip = Vector3.Distance(
                TempestController.tc.GetMid(startLane),
                prefabParent.transform.position);
            float scale = currTrip / totalTrip * destLaneLength;
            prefabParent.transform.localScale = new Vector3(scale, scale, 1);
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
        prefabParent.transform.position = v;
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

    void InitPos()
    {
        Vector3 leftp = startLane.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 rightp = startLane.GetComponent<LineRenderer>().GetPosition(1);
        float startLaneLength = Vector3.Distance(
            leftp,
            rightp);
        float angle = TempestController.tc.RotateToCenter(gameObject, endpoint);
        if (endpoint.x < 0)
        {
            angle += 180;
        }
        float scale = startLaneLength * Mathf.Sin(Mathf.Abs(angle) * Mathf.Deg2Rad);
        prefabParent.transform.position = TempestController.tc.GetMid(startLane);
        prefabParent.transform.localScale = new Vector3(scale, scale, 1);

        if(leftp.x == rightp.x)
        {
            prefabParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else
        {
            float rangle = Mathf.Atan((rightp.y - leftp.y)/(rightp.x - leftp.x)) * Mathf.Rad2Deg;
            prefabParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rangle));
        }

//        prefabParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
