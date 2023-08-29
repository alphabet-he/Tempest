using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    int loc;
    int infectionState = 0;

    bool worsening = false;

    public int Loc { get => loc; set => loc = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Ally";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeInfection(int newState)
    {
        infectionState = newState;
        // add infection state render code
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // explode

        if(other.tag == "EnemyBullet")
        {
            Destroy(gameObject);
        }
        
    }


}
