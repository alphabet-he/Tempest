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
    public bool IsDissolving { get => isDissolving; set => isDissolving = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Ally";
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDissolving)
        {
            fade -= Time.deltaTime * AllyController.ac.FadeSpeed;
            if (fade <= 0f)
            {
                fade = 0f;
                IsDissolving = false;
                gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
                AllyController.ac.Allies[loc].Remove(this);
                Destroy(gameObject);
            }
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);

        }
    }

    public void heal()
    {
        fade = 1f;
        IsDissolving = false;
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "EnemyBullet")
        {
            // explode
            StartCoroutine(Explode());
            // infect nearby cells
            AllyController.ac.Allies[loc].Remove(this);
            foreach(Ally a in AllyController.ac.Allies[loc]) { if (!a.IsDissolving) a.IsDissolving = true; }
            if(loc > 0)
            {
                foreach (Ally a in AllyController.ac.Allies[loc-1]) { if (!a.IsDissolving) a.IsDissolving = true; }
            }
            if(loc < TempestController.tc.MaxLoc)
            {
                foreach (Ally a in AllyController.ac.Allies[loc+1]) { if (!a.IsDissolving) a.IsDissolving = true; }
            }

        }
        
    }

    IEnumerator Explode()
    {
        // visual effect
        gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        yield return new WaitForSeconds(1.0f);
        
        Destroy(gameObject);
    }


}
