using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AllyController : MonoBehaviour
{
    public Animator Animator;
    public AnimationClip explode;
    public AnimationClip worsening;
//    public AnimatorController explode;
//    public AnimatorController worsening;


    public float fadeSpeed = 0.5f;
    List<List<Ally>> allies = new List<List<Ally>>();

    public List<List<Ally>> Allies { get => allies; set => allies = value; }
    public float FadeSpeed { get => fadeSpeed; set => fadeSpeed = value; }

    public static AllyController ac;



    private void Awake()
    {

        if (ac == null)
        {
            DontDestroyOnLoad(gameObject);
            ac = this;

        }
        else if (ac != null)
        {
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAllies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAllies()
    {
        int i = -1;
        foreach (Transform childgroup in gameObject.transform)
        {
            i++;
            List<Ally> group = new List<Ally>();
            int j = 0;
            foreach (Transform child in childgroup.transform)
            {
                // add to list
                group.Add(child.gameObject.GetComponent<Ally>());
                child.gameObject.GetComponent<Ally>().Loc = i; // pass loc
                child.gameObject.GetComponent<Ally>().FadeSpeed = FadeSpeed; // pass fade time
                child.gameObject.GetComponent<Ally>().GroupLoc = j;
                child.gameObject.SetActive(true);
                // put game object
                if (j == 0)
                {
                    Vector3 v = TempestController.tc.GetMid(TempestController.tc.AllyLanes0[i]);
                    child.gameObject.transform.position = TempestController.tc.GetMid(TempestController.tc.AllyLanes0[i]);
                }
                else if (j == 1)
                {
                    child.gameObject.transform.position = TempestController.tc.GetMid(TempestController.tc.AllyLanes1[i]);
                }
                else
                {
                    Debug.Log("j count wrong");
                }
                j++;
            }
            allies.Add(group);
        }
    }


}
