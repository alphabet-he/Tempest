using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;
    //List<Tuple<GameObject, Vector3>> bullets = new List<Tuple<GameObject, Vector3>>(); // bullet object - destination

    public static BulletController bc;


    private void Awake()
    {

        if (bc == null)
        {
            DontDestroyOnLoad(gameObject);
            bc = this;
        }
        else if (bc != null)
        {
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
        }
    }

    public void NewBullets(Vector3 pos, Vector3 endpoint, bool identity)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.transform.GetChild(0).GetComponent<Bullet>().BulletSpeed = bulletSpeed;
        bullet.transform.GetChild(0).GetComponent<Bullet>().Endpoint = endpoint;
        bullet.transform.GetChild(0).GetComponent<Bullet>().Identity = identity;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
