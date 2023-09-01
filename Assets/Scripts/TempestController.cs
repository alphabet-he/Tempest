using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TempestController : MonoBehaviour
{
    public float healingEffectLasting = 1f;
    public GameObject lifeCountPrefab;
    public int shootEnemyScore = 50;
    public int allyRemainingScore = 100;

    int score;
    public int hp = 100;
    int loc;
    int maxLoc;


    List<GameObject> startLanes = new List<GameObject>();
    List<GameObject> playerLanes = new List<GameObject>();
    List<GameObject> allyLanes0 = new List<GameObject>();
    List<GameObject> allyLanes1 = new List<GameObject>();
    List<GameObject> endLanes = new List<GameObject>();
    List<GameObject> healingEffect = new List<GameObject>();
    List<GameObject> lifeCounts = new List<GameObject>();
    GameObject scoreText;
    GameObject endPanel;


    // control 
    bool canShoot = true;
    bool canMove = true;

    public float minimumShootHeldDuration = 0.2f; // press for how long to consider it a hold
    public float pressShootCD = 0.25f;
    public float holdShootCD = 0.4f;
    public float holdShootInterval = 0.15f;
    public int maxContinuousShoot = 5;

    public float minimumMoveHeldDuration = 0.5f; // press for how long to consider it a hold
    public float pressMoveCD = 0.2f;
    public float holdMoveCD = 0.2f;
    public float holdMoveInterval = 0.1f;

    private float _movePressedTime = 0;
    private bool _moveHeld = false;
    private float _xPressedTime = 0;
    private bool _xHeld = false;
    private int _continuousShootCnt = 0;


    public int Loc { get => loc; set => loc = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxLoc { get => maxLoc; set => maxLoc = value; }
    public int Score { get => score; set => score = value; }
    public List<GameObject> StartLanes { get => startLanes; set => startLanes = value; }
    public List<GameObject> PlayerLanes { get => playerLanes; set => playerLanes = value; }
    public List<GameObject> AllyLanes0 { get => allyLanes0; set => allyLanes0 = value; }
    public List<GameObject> AllyLanes1 { get => allyLanes1; set => allyLanes1 = value; }
    public List<GameObject> EndLanes { get => endLanes; set => endLanes = value; }

    public static TempestController tc;
    
    public System.Random rnd = new System.Random();


    private void Awake()
    {

        if (tc == null)
        {
            DontDestroyOnLoad(gameObject);
            tc = this;
        }
        else if (tc != null)
        {
            Destroy(gameObject); Destroy(gameObject.transform.parent.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Tempest";
        loc = 0;
        score = 0;
        GameObject padsParent = GameObject.Find("MovingPads").gameObject;
        foreach (Transform child in padsParent.transform)
        {
            PlayerLanes.Add(child.gameObject);
        }
        GameObject startpadsParent = GameObject.Find("StartPads").gameObject;
        foreach (Transform child in startpadsParent.transform)
        {
            StartLanes.Add(child.gameObject);
        }
        GameObject endpadsParent = GameObject.Find("EndPads").gameObject;
        foreach (Transform child in endpadsParent.transform)
        {
            EndLanes.Add(child.gameObject);
        }
        GameObject allypadsParent0 = GameObject.Find("AllyPads0").gameObject;
        foreach (Transform child in allypadsParent0.transform)
        {
            AllyLanes0.Add(child.gameObject);
        }
        GameObject allypadsParent1 = GameObject.Find("AllyPads1").gameObject;
        foreach (Transform child in allypadsParent1.transform)
        {
            AllyLanes1.Add(child.gameObject);
        }
        GameObject healingParent = GameObject.Find("HealingEffect").gameObject;
        foreach (Transform child in healingParent.transform)
        {
            healingEffect.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        // put life count
        GameObject lifeCount0 = GameObject.Find("Canvas/Panel/LifeCount").gameObject;
        lifeCounts.Add(lifeCount0);
        Vector3 parentPos = lifeCount0.transform.position;
        GameObject img = lifeCount0.transform.GetChild(0).transform.GetChild(0).gameObject;
        Vector3 relativePos = img.GetComponent<RectTransform>().position; // image position
        float interval = img.GetComponent<RectTransform>().rect.width* 1.1f ;
        for(int i = 0; i < hp -1; i++)
        {
            GameObject life = Instantiate(lifeCountPrefab, parentPos, Quaternion.identity);
            life.transform.parent = lifeCount0.transform.parent;
            lifeCounts.Add(life);
            relativePos.x += interval;
            life.transform.GetChild(0).transform.GetChild(0).position = relativePos;
        }

        // find score text
        scoreText = GameObject.Find("Canvas/Panel/Score").gameObject;
        // set score to 0
        score = 0;
        SetScore();

        // find end panel
        GameObject endParent = GameObject.Find("GameEndCanvas");
        endPanel = endParent.transform.GetChild(0).gameObject;
        endPanel.SetActive(false);

        maxLoc = PlayerLanes.Count-1;
        Debug.Log(maxLoc);
        MoveTempest();

    }

    // Update is called once per frame
    void Update()
    {
        // Fire
        if(canShoot) CheckFireKey(); 

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down arrow down");
            StartCoroutine(Heal());
        }

        if (canMove)
        {
            CheckMoveKey(true);
            CheckMoveKey(false);
        }

    }


    void CheckFireKey()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Use has pressed the x key. We don't know if they'll release or hold it, so keep track of when they started holding it.
            _xPressedTime = Time.timeSinceLevelLoad;
            _xHeld = false;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            if (!_xHeld)
            {
                Debug.Log(canShoot);
                // Player has released the x key without holding it.
                // TODO: Perform the action for when x is pressed.
                Fire();
                StartCoroutine(StartCD((i) => { canShoot = i; }, pressShootCD));
                Debug.Log(canShoot);
            }
            else
            {
                StartCoroutine(StartCD((i) => { canShoot = i; }, holdShootCD));
                Debug.Log(canShoot);
            }
            _xHeld = false;
            _continuousShootCnt = 0;
        }

        if (Input.GetKey(KeyCode.X))
        {
            if (Time.timeSinceLevelLoad - _xPressedTime > minimumShootHeldDuration)
            {
                // Player has held the x key for .25 seconds. Consider it "held"
                _xHeld = true;
                if (canShoot && _continuousShootCnt < maxContinuousShoot)
                {
                    StartCoroutine(HoldToFire());
                }

                if (canShoot && _continuousShootCnt >= maxContinuousShoot)
                {
                    StartCoroutine(StartCD((i) => { canShoot = i; }, holdShootCD));
                    _continuousShootCnt = 0;
                }

            }
        }
    }

    void CheckMoveKey(bool moveLeft)
    {
        KeyCode key = moveLeft ? KeyCode.LeftArrow : KeyCode.RightArrow;
        if (Input.GetKeyDown(key))
        {
            // Use has pressed the left key. We don't know if they'll release or hold it, so keep track of when they started holding it.
            _movePressedTime = Time.timeSinceLevelLoad;
            _moveHeld = false;
            if (moveLeft) MoveLeft();
            else MoveRight();
        }
        else if (Input.GetKeyUp(key))
        {
            if (!_moveHeld)
            {
                // Player has released the left key without holding it.
                // TODO: Perform the action for when left is pressed.
                StartCoroutine(StartCD((i) => { canMove = i; }, pressMoveCD));
            }
            else
            {
                StartCoroutine(StartCD((i) => { canMove = i; }, holdMoveCD));
            }
            _moveHeld = false;
        }

        if (Input.GetKey(key))
        {
            if (Time.timeSinceLevelLoad - _movePressedTime > minimumMoveHeldDuration)
            {
                // Player has held the left key for .25 seconds. Consider it "held"
                _moveHeld = true;
                if (canMove)
                {
                    StartCoroutine(HoldToMove(moveLeft));
                }

            }
        }
    }

    void Fire()
    {
        AudioManager.Instance.PlaySFX("player_shoot");
        BulletController.bc.NewBullets(gameObject.transform.position, GetMid(StartLanes[loc]), true);
        // remove all enemies on neighbouring edges
        if (loc > 0)
        {
            foreach (var enemy in EnemyController.ec.Enemies[loc - 1])
            {
                if (!enemy.IsDestroyed())
                {
                    Destroy(enemy);
                    Destroy(enemy.transform.parent.gameObject);
                    score += shootEnemyScore;
                    Debug.Log("Shoot enemy!");
                }
            }
            EnemyController.ec.Enemies[loc - 1].Clear();
        }
        
        if (loc < maxLoc)
        {
            foreach (var enemy in EnemyController.ec.Enemies[loc + 1])
            {
                Destroy(enemy);
                Destroy(enemy.transform.parent.gameObject);
                score += shootEnemyScore;
                Debug.Log("Shoot enemy!");
            }
            EnemyController.ec.Enemies[loc + 1].Clear();
        }

        SetScore();

    }

    IEnumerator StartCD(Action<bool> value, float cdTime)
    {
        value(false);
        yield return new WaitForSeconds(cdTime);
        value(true);
    }

    IEnumerator HoldToFire()
    {
        Fire();
        canShoot = false;
        _continuousShootCnt++;
        yield return new WaitForSeconds(holdShootInterval);
        canShoot = true;
    }

    IEnumerator HoldToMove(bool moveLeft)
    {
        if(moveLeft) { MoveLeft(); }
        else { MoveRight(); }
        canMove = false;
        yield return new WaitForSeconds(holdMoveInterval);
        canMove = true;
    }

    IEnumerator Heal()
    {
        int num = loc;
        healingEffect[num].SetActive(true);
        Debug.Log(healingEffect.Count);
        foreach(var ally in AllyController.ac.Allies[num])
        {
            if (!ally.IsDestroyed())
            {
                ally.heal();
            }
            
        }
        yield return new WaitForSeconds(healingEffectLasting);
        healingEffect[num].SetActive(false);
    }

    void MoveTempest()
    {
        GameObject pad = PlayerLanes [loc];
        Vector3 v = GetMid(pad);
        gameObject.transform.position = v;
        float angle = RotateToCenter(gameObject, GetMid(startLanes[loc]));
        if(loc >= maxLoc / 2)
        {
            angle += 180;
        }
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //RotateToCenter(gameObject, StartLanes[loc]);
    }

    void MoveLeft()
    {
        if(loc > 0) // can go left
        {
            AudioManager.Instance.PlaySFX("player_move");
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
            AudioManager.Instance.PlaySFX("player_move");
            loc++;
            MoveTempest();
            Debug.Log($"Move right {loc}");
        }
        else
        {
            Debug.Log("Can't go right");
        }
    }

     public void SetScore()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "EnemyBullet")
        {
            hp--;
            lifeCounts[hp].SetActive(false);

            Debug.Log(hp);
            if(hp <= 0) // the player dies
            {
                Destroy(gameObject);
                AudioManager.Instance.PlaySFX("player_explode");
                Debug.Log("Tempest died");
                EndGame();
            }
            else
            {
                AudioManager.Instance.PlaySFX("player_hurt");
            }
        }
    }

    public void EndGame()
    {
        Time.timeScale = 0; // pause game
        foreach(List<Ally> group in AllyController.ac.Allies)
        {
            score += group.Count * allyRemainingScore; // calculate score
        }
        endPanel.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = score.ToString() ;
        endPanel.SetActive(true);
    }

    public Vector3 GetMid(GameObject lane)
    {
        Vector3 v0 = lane.GetComponent<LineRenderer>().GetPosition(0);
        Vector3 v1 = lane.GetComponent<LineRenderer>().GetPosition(1);
        Vector3 v = (v0 + v1) * 0.5f;
        return v;
    }

    public float RotateToCenter(GameObject toRotate, Vector3 targ)
    {
        targ.z = 0f;

        Vector3 objectPos = toRotate.transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.x, targ.y) * Mathf.Rad2Deg;
        //Debug.Log(angle);
        if (angle < 0) { angle += 180; }
        return angle * (-1);

    }

}
