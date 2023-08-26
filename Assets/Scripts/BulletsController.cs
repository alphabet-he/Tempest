using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 0.5f;
    List<Tuple<GameObject, Vector3>> bullets = new List<Tuple<GameObject, Vector3>>(); // bullet object - destination

    public static BulletsController bc;


    private void Awake()
    {

        if (bc == null)
        {
            DontDestroyOnLoad(gameObject);
            bc = this;
        }
        else if (bc != null)
        {
            Destroy(gameObject);
        }
    }

    public void NewBullets(Vector3 pos, Vector3 endpoint)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullets.Add( new Tuple<GameObject, Vector3>(bullet, endpoint));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bullets.Count > 0)
        {
            List<Tuple<GameObject, Vector3>> toRemove = new List<Tuple<GameObject, Vector3>>();
            foreach (var bullet in bullets) //for each bullet, move
            {
                Vector3 pos = bullet.Item1.transform.position;
                float ratio = Time.deltaTime * bulletSpeed / Vector3.Distance(pos, bullet.Item2);
                if (ratio >= 1) // reach the end
                {
                    bullet.Item1.transform.position = bullet.Item2; // directly show at the end point
                    toRemove.Add(bullet);
                    Destroy(bullet.Item1, 1); // destroy bullet
                }
                else // move bullet
                {
                    pos.x += ratio * (bullet.Item2.x - pos.x);
                    pos.y += ratio * (bullet.Item2.y - pos.y);
                    bullet.Item1.transform.position = pos;
                }
             
            }
            bullets.RemoveAll(x => toRemove.Contains(x));
        }
    }
}
