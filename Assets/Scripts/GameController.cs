using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gc;


    private void Awake()
    {

        if (gc == null)
        {
            DontDestroyOnLoad(gameObject);
            gc = this;
        }
        else if (gc != null)
        {
            Destroy(gameObject);
        }
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
