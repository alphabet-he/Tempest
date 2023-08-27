using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestController : MonoBehaviour
{
    public Vector3 endpoint;

    int score;
    public int hp = 2;
    int loc;
    int maxLoc;
    public List<GameObject> lanes = new List<GameObject>();

    public int Loc { get => loc; set => loc = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxLoc { get => maxLoc; set => maxLoc = value; }
    public int Score { get => score; set => score = value; }

    public static TempestController tc;
    
    public System.Random rnd = new System.Random();


    private void Awake()
    {

        if (tc == null)
        {
            DontDestroyOnLoad(gameObject);
            tc = this;
        }
        else if (tc != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Tempest";
        loc = 0;
        score = 0;
        GameObject padsParent = GameObject.Find("MovingPads").gameObject;
        foreach (Transform child in padsParent.transform)
        {
            lanes.Add(child.gameObject);
        }
        maxLoc = lanes.Count-1;
        Debug.Log(maxLoc);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X down");
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left arrow down");
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right arrow down");
            MoveRight();
        }
    }


    void Fire()
    {
        BulletController.bc.NewBullets(gameObject.transform.position, endpoint, true);
        // remove all enemies on neighbouring edges
        if (loc > 0)
        {
            foreach (var enemy in EnemyController.ec.Enemies[loc - 1])
            {
                Destroy(enemy);
            }
            EnemyController.ec.Enemies[loc - 1].Clear();
        }
        
        if (loc < maxLoc)
        {
            foreach (var enemy in EnemyController.ec.Enemies[loc + 1])
            {
                Destroy(enemy);
            }
            EnemyController.ec.Enemies[loc + 1].Clear();
        }
        

    }
    

    void MoveTempest()
    {
        GameObject pad = lanes [loc];
        Vector3 v0 = pad.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 v1 = pad.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 v = (v0 + v1) * 0.5f;
        gameObject.transform.position = v;
    }

    void MoveLeft()
    {
        if(loc > 0) // can go left
        {
            loc--;
            MoveTempest();
            Debug.Log($"Move left {loc}");
        }
        else
        {
            Debug.Log("Can't go left");
        }
    }

    void MoveRight()
    {
        if (loc < maxLoc) // can go right
        {
            loc++;
            MoveTempest();
            Debug.Log($"Move right {loc}");
        }
        else
        {
            Debug.Log("Can't go right");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "EnemyBullet")
        {
            hp--;
            Debug.Log(hp);
            if(hp <= 0) // the player dies
            {
                Destroy(gameObject);
            }
        }
    }

}
