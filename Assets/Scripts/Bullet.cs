using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int loc;
    float bulletSpeed;
    Vector3 endpoint;
    bool identity; // True - player, False - enemy 

    float accelerateFreq;

    public float BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }
    public Vector3 Endpoint { get => endpoint; set => endpoint = value; }
    public bool Identity { get => identity; set => identity = value; }
    public int Loc { get => loc; set => loc = value; }
    public float AccelerateFreq { get => accelerateFreq; set => accelerateFreq = value; }


    GameObject startLane;
    GameObject destLane;

    // Start is called before the first frame update
    void Start()
    {
        float angle = TempestController.tc.RotateToCenter(gameObject, endpoint);
        if (identity) 
        { 
            gameObject.tag = "PlayerBullet";
            
            if (endpoint.x>=0)
            {
                angle += 180;
            }

            startLane = TempestController.tc.PlayerLanes[loc];
            destLane = TempestController.tc.StartLanes[loc];
            Debug.Log("Player bullet initiated");


        }
        else 
        { 
            gameObject.tag = "EnemyBullet";
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            if (endpoint.x < 0)
            {
                angle += 180;
            }
            startLane = TempestController.tc.StartLanes[loc];
            destLane = TempestController.tc.EndLanes[loc];
        }
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Invoke("accelerate", 0);
    }

    void accelerate()
    {

        bulletSpeed = TempestController.tc.SpeedToScale(bulletSpeed, startLane, destLane, transform.position);
        Invoke("accelerate", AccelerateFreq);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = gameObject.transform.position;
        float ratio = Time.deltaTime * bulletSpeed / Vector3.Distance(pos, endpoint);
        if (ratio >= 1) // reach the end
        {
            gameObject.transform.position = endpoint; // directly show at the end point
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject); // destroy bullet
        }
        else // move bullet
        {
            pos.x += ratio * (endpoint.x - pos.x);
            pos.y += ratio * (endpoint.y - pos.y);
            gameObject.transform.position = pos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (identity) // if is player's bullets
        {
            if (other.tag == "Enemy" || other.tag == "EnemyBullet")
            {
                Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
            }
        }
        else // if is enemy's bullets
        {
            if(other.tag == "Tempest" || other.tag == "PlayerBullet" || other.tag == "Ally")
            {
                Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
