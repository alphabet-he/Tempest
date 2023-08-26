using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempestController : MonoBehaviour
{
    public Vector3 endpoint;

    public GameObject enemyPrefab;


    int hp;
    int loc;
    int maxLoc;
    List<GameObject> movingPads = new List<GameObject>();

    public int Loc { get => loc; set => loc = value; }
    public int Hp { get => hp; set => hp = value; }


    // Start is called before the first frame update
    void Start()
    {
        loc = 0;
        hp = 0;
        GameObject padsParent = GameObject.Find("MovingPads").gameObject;
        foreach (Transform child in padsParent.transform)
        {
            movingPads.Add(child.gameObject);
        }
        maxLoc = movingPads.Count-1;
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
        BulletsController.bc.NewBullets(gameObject.transform.position, endpoint);

    }
    

    void MoveTempest()
    {
        GameObject pad = movingPads [loc];
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

    void BulletMove()
    {

    }

}
