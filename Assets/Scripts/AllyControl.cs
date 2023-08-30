using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyControl : MonoBehaviour
{
    public Animation explode;
    public float fadeSpeed = 1f;
    List<List<Ally>> allies = new List<List<Ally>>();

    public List<List<Ally>> Allies { get => allies; set => allies = value; }
    public static AllyControl ac;



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
                child.gameObject.GetComponent<Ally>().FadeSpeed = fadeSpeed; // pass fade time
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

    // Update is called once per frame
    void Update()
    {
        
    }


}
