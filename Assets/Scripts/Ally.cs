using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    int loc;
    float fade = 1f;
    float fadeSpeed;
    bool isDissolving = false;
    int groupLoc;
    public Animator Animator;

    public int Loc { get => loc; set => loc = value; }
    public float FadeSpeed { get => fadeSpeed; set => fadeSpeed = value; }
    public bool IsDissolving { get => isDissolving; set => isDissolving = value; }
    public int GroupLoc { get => groupLoc; set => groupLoc = value; }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Ally";
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDissolving)
        {
            StartCoroutine(Worsening());


/*            fade -= Time.deltaTime * AllyController.ac.FadeSpeed;
            Animator.SetBool("IsDissolve", true);
            //Animator.SetFloat("fade", fade);
            if (fade <= 0f)
            {
                fade = 0f;
                IsDissolving = false;
                gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
                AllyController.ac.Allies[loc].Remove(this);
                gameObject.SetActive(false);
            }
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);*/

        }
    }

    public void heal()
    {
        ;
        if (IsDissolving)
        {
            fade = 0.6f;
            Animator.SetBool("IsDissolve", false);
            IsDissolving = false;
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
            AudioManager.Instance.PlaySFX("ally_heal");
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "EnemyBullet")
        {
            // explode
            if(!IsDissolving) 
            {
                StartCoroutine(Explode());
            }

        }
        
        
    }

    void infect()
    {
        // infect nearby cells
        foreach (Ally a in AllyController.ac.Allies[loc]) { if (!a.IsDissolving && a.GroupLoc == groupLoc) a.IsDissolving = true; }
        if (loc > 0)
        {
            if (AllyController.ac.Allies[loc - 1].Count > 0)
            {
                foreach (Ally a in AllyController.ac.Allies[loc - 1]) { if (!a.IsDissolving && a.GroupLoc == groupLoc) a.IsDissolving = true; }

            }

        }
        if (loc < TempestController.tc.MaxLoc)
        {
            if (AllyController.ac.Allies[loc + 1].Count > 0)
            {
                foreach (Ally a in AllyController.ac.Allies[loc + 1]) { if (!a.IsDissolving && a.GroupLoc == groupLoc) a.IsDissolving = true; }
            }

        }
    }

    IEnumerator Explode()
    {
        // explode animation
        Animator.SetBool("explode", true);
        float waitTime = 0;
        foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "explode")
            {
                waitTime = clip.length; 
                break;
            }
        }

        yield return new WaitForSeconds(waitTime);

        Animator.SetBool("explode", false);
        gameObject.SetActive(false);
        AllyController.ac.Allies[loc].Remove(this);
        infect();
    }

    IEnumerator Worsening()
    {
        // explode animation
        Animator.SetBool("IsDissolve", true);
        float waitTime = 0;
        foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
        {
            waitTime += clip.length;
        }

        yield return new WaitForSeconds(waitTime);

        Animator.SetBool("IsDissolve", false);
        gameObject.SetActive(false);
        AllyController.ac.Allies[loc].Remove(this);
        infect();
    }


}
