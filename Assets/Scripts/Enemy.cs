using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    float enemyAcc;
    float accelerateFreq;
    float rotateSpeed;
    float rotateUpdate = 0.02f;

    bool onEdge = false;
    bool isExploding = false;

    public float EnemySpeed { get => enemySpeed; set => enemySpeed = value; }
    public Vector3 Endpoint { get => endpoint; set => endpoint = value; }
    public int Loc { get => loc; set => loc = value; }
    //public float ShootAfter { get => shootAfter; set => shootAfter = value; }
    public float ShootFreqMin { get => shootFreqMin; set => shootFreqMin = value; }
    public float ShootFreqMax { get => shootFreqMax; set => shootFreqMax = value; }
    public float ChaseFreq { get => chaseFreq; set => chaseFreq = value; }
    public float AccelerateFreq { get => accelerateFreq; set => accelerateFreq = value; }
    public float EnemyAcc { get => enemyAcc; set => enemyAcc = value; }
    public float RotateSpeed { get => rotateSpeed; set => rotateSpeed = value; }

    GameObject startLane;
    GameObject destLane;
    GameObject prefabParent;
    float destLaneLength;
    float startLaneLength;
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
        startLaneLength = Vector3.Distance(
            startLane.GetComponent<LineRenderer>().GetPosition(0),
            startLane.GetComponent<LineRenderer>().GetPosition(1));
        totalTrip = Vector3.Distance(
            TempestController.tc.GetMid(startLane),
            TempestController.tc.GetMid(destLane));
        prefabParent = gameObject.transform.parent.gameObject;
        InitPos();
        float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        Invoke("Shoot", randomTime);
        //Invoke("accelerate", 0);
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
        BulletController.bc.NewBullets(prefabParent.transform.position, TempestController.tc.GetMid(TempestController.tc.EndLanes[loc]), false, enemySpeed);
        TempestController.tc.rnd.NextDouble();
        //float randomTime = (float)(TempestController.tc.rnd.NextDouble() * (shootFreqMax - shootFreqMin) + shootFreqMin);
        //Invoke("Shoot", randomTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (onEdge)  return;
        if (isExploding) return;
        enemySpeed += enemyAcc;
        Vector3 pos = prefabParent.transform.position;
        //Debug.Log(enemySpeed);
        float ratio = Time.deltaTime * enemySpeed / Vector3.Distance(pos, endpoint);
        if (ratio >= 1) // reach the end
        {
            prefabParent.transform.position = endpoint; // directly show at the end point
            prefabParent.transform.localScale = new Vector3(destLaneLength, destLaneLength, 1);

            EnemyController.ec.Enemies[loc].Add(gameObject);
            onEdge = true;

            Invoke("Chase", 0.3f);
        }
        else // move enemy
        {
            pos.x += ratio * (endpoint.x - pos.x);
            pos.y += ratio * (endpoint.y - pos.y);
            prefabParent.transform.position = pos;
            float currTrip = Vector3.Distance(
                TempestController.tc.GetMid(startLane),
                prefabParent.transform.position);
            //float scale = currTrip / totalTrip * destLaneLength;
            float scale = startLaneLength + currTrip / totalTrip * (destLaneLength - startLaneLength);
            prefabParent.transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    void Chase()
    {
        if (isExploding) return;
        EnemyController.ec.Enemies[loc].Remove(gameObject);
        if (TempestController.tc.Loc < loc) { MoveOnLanes(true); }
        else if (TempestController.tc.Loc > loc) { MoveOnLanes(false); }
        else { Debug.Log("Should Trigger"); }

        
        Invoke("Chase", chaseFreq);
    }

    void MoveOnLanes(bool toLeft)
    {
        GameObject originalPad = TempestController.tc.PlayerLanes[loc];
        Vector3 startp0 = originalPad.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 startp1 = originalPad.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 pivot = Vector3.zero;
        if (toLeft) 
        { 
            loc--;
            if (startp0.x < startp1.x) pivot = startp0;
            else pivot = startp1;
        }
        else 
        { 
            loc++;
            if (startp0.x < startp1.x) pivot = startp1;
            else pivot = startp0;
        }
        GameObject destPad = TempestController.tc.PlayerLanes[loc];
        Vector3 v = TempestController.tc.GetMid(destPad);
        
        Vector3 destleftp = destPad.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 destrightp = destPad.GetComponent<LineRenderer>().GetPosition(1);
        float rangle;
        if (destleftp.x == destrightp.x)
        {
            rangle = 90f;
        }
        else
        {
            rangle = Mathf.Atan((destrightp.y - destleftp.y) / (destrightp.x - destleftp.x)) * Mathf.Rad2Deg;
        }
        float scale = Vector3.Distance(destleftp, destrightp); // destinated scale

        float currRangle = prefabParent.transform.eulerAngles.z;
        float t = Mathf.Abs(Mathf.Abs(prefabParent.transform.eulerAngles.z - rangle) - 180) / rotateSpeed;
        float scaleSpeed = (scale - prefabParent.transform.localScale.x) / t;

        StartCoroutine(RotateOnLane(toLeft, rangle, scale, scaleSpeed, pivot, v, t));
    }

   

    IEnumerator RotateOnLane(bool toLeft, float rangle, float scale, float scaleSpeed, Vector3 pivot, Vector3 destv, float t)
    {
        int cnt = (int)(t / rotateUpdate);

        //for(int i=0; i<cnt; i++)
        while(Mathf.Abs(Mathf.Abs(prefabParent.transform.eulerAngles.z - rangle) - 180) > rotateSpeed)
        {
            //Debug.Log(prefabParent.transform.eulerAngles.z);
            Debug.Log(Mathf.Abs(rangle - prefabParent.transform.eulerAngles.z));
            if (toLeft)
            {
                prefabParent.transform.RotateAround(pivot, Vector3.forward, rotateSpeed);
            }
            else
            {
                prefabParent.transform.RotateAround(pivot, Vector3.forward, (-1) * rotateSpeed);
            }
            float originalScale = prefabParent.transform.localScale.x;
            prefabParent.transform.localScale = new Vector3(originalScale + scaleSpeed, originalScale + scaleSpeed, 1);

            yield return new WaitForSeconds(rotateUpdate);
        }

        prefabParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rangle));
        prefabParent.transform.localScale = new Vector3(scale, scale, 1);
        prefabParent.transform.position = destv;
        EnemyController.ec.Enemies[loc].Add(gameObject);
        yield break;



    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerBullet")
        {
            Debug.Log("Shoot Enemy!");
            EnemyExplode();
        }
        else if(other.tag == "Tempest")
        {
            Debug.Log("Catch Tempest!");
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject); // enemy disappear
        }
    }

    public void EnemyExplode()
    {
        isExploding = true;
        StartCoroutine(explode());
        AudioManager.Instance.PlaySFX("enemy_explode");
        TempestController.tc.Score += TempestController.tc.shootEnemyScore;
        TempestController.tc.SetScore();
    }

    IEnumerator explode()
    {
        Animator animator = GetComponent<Animator>();
        
        float waitTime = 0;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "explodeEnemyS" || clip.name == "explodeEnemy")
            {
                waitTime = clip.length;
                break;
            }
        }
        animator.SetBool("IsExplode", true);
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("IsExplode", false);
        Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject); // enemy defeated
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
