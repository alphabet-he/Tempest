using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    int loc;
    float fade = 1f;
    float fadeSpeed;
    bool isDissolving = false;

    public int Loc { get => loc; set => loc = value; }
    public float FadeSpeed { get => fadeSpeed; set => fadeSpeed = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Ally";
    }

    // Update is called once per frame
    void Update()
    {
        if (isDissolving)
        {
            fade -= Time.deltaTime;
            if (fade <= 0f)
            {
                fade = 0f;
                isDissolving=false;
                gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
                Destroy(gameObject);
            }
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);

        }
    }

    public void heal()
    {
        fade = 1f;
        isDissolving = false;
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "EnemyBullet")
        {
            // explode
            isDissolving = true;
        }
        
    }


}
