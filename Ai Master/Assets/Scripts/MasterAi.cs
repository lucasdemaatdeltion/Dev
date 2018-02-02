using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//this is the master ai script with every move the ai can do.
public class MasterAi : MonoBehaviour
{
    //These are all the variables for all the ai.
    public GameObject closest;

    public int hp;
    public int picaxeDamage;
    public int axeDamage;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    public float afkTimer;
    const float tijd = 7f;

    private bool timerRunning;

    public float wanderRadius = 40;
    public float wanderTimer = 10;

    public RaycastHit hit;
    public float reach = 2;
    private Vector3 fwd;
    public float distance = 2;
    public float largeDistance = 15;
    float destinationReachedTreshold = 2;

    private bool destinationReached;

    public GameObject player;
    public int offset;

    public int walkingspeed;
    public int chasingSpeed;

    private bool randomAiMovement;
    public bool idle, walking, attack;
    public Animator anim;

    public enum Actions
    {
        RandomWalking,
        Chasing,
        Working,
        Idle,
        Combat
    }

    public Actions actions;

    //This searches for the closest wood for the working state.
    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Wood");
        closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    //This starts when you start the game.
    void Start()
    {
        afkTimer += Time.deltaTime;
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        if (!player)
        {
            Debug.Log("Make sure your player is tagged!!");
        }
    }

    //this get enabled when you start the scene.
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        //this is the direction the raycast will go.
        fwd = transform.TransformDirection(Vector3.forward);

        //this says that the ai will walk randomly until it sees you.
        if (actions == Actions.RandomWalking)
        {
            randomAiMovement = true;
            walking = true;
            attack = false;
            GetComponent<NavMeshAgent>().speed = walkingspeed;

            Debug.DrawRay(transform.position, fwd * largeDistance, Color.green);
            if (Physics.Raycast(transform.position, transform.forward, out hit, largeDistance) && hit.collider.gameObject.CompareTag("Player"))
            {
                actions = Actions.Chasing;
            }
        }

        //When the ai has seen you he will start chasing if he can't get you within 7 seconds he will start walking randomly.
        if (actions == Actions.Chasing)
        {
            transform.LookAt(player.transform.position);
            attack = false;
            GetComponent<NavMeshAgent>().destination = new Vector3(player.transform.position.x + offset, player.transform.position.y + offset, player.transform.position.z+ offset);
            GetComponent<NavMeshAgent>().speed = chasingSpeed;
            if(destinationReached == true)
            {
                walking = false;
                actions = Actions.Combat;
            }

            if (destinationReached == false)
            {
                walking = true;
                attack = false;
                CheckDestinationReached();
                timerRunning = true;
                if(timerRunning == true)
                {
                    Timer();
                }
            }
        }

        //this says dat it is working and is searching for the closest resource.
        if (actions == Actions.Working)
        {
            FindClosestEnemy();
            GetComponent<NavMeshAgent>().destination = new Vector3 (closest.transform.position.x, closest.transform.position.x, closest.transform.position.x);
        }

        //The ai will play an animation that is looking around and if it sees you it will start chasing you.
        if (actions == Actions.Idle)
        {
            //anim.enabled = true;
            Debug.DrawRay(transform.position, fwd * largeDistance, Color.green);
            if (Physics.Raycast(transform.position, transform.forward, out hit, largeDistance) && hit.collider.gameObject.CompareTag("Player"))
            {
                anim.enabled = false;
                actions = Actions.Chasing;
            }
        }

        //when it has caught up with you it will start fighting you and when you are out of range of the ai he will start chasing you again.
        if (actions == Actions.Combat)
        {
            walking = false;
            transform.LookAt(player.transform.position);
            GetComponent<NavMeshAgent>().destination = new Vector3(player.transform.position.x + offset, player.transform.position.y + offset, player.transform.position.z + offset);
            Debug.DrawRay(transform.position, fwd * distance, Color.blue);
            if (Physics.Raycast(transform.position, transform.forward,out hit, distance) && hit.collider.gameObject.CompareTag("Player"))
            {
                actions = Actions.Combat;
                attack = true;
            }
            else
            {
                actions = Actions.Chasing;
                attack = false;
                walking = true;
                Timer();
            }
        }
    }

    //this says when it can use the random walking.
    void Update()
    {
        if(randomAiMovement == true)
        {
             timer += Time.deltaTime;

             if (timer >= wanderTimer)
             {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
             }
        }
    }

    //this is the timer that the ai uses for chasing you.
    void Timer()
    {
        afkTimer += Time.deltaTime;
        if(afkTimer > tijd)
        {
            actions = Actions.RandomWalking;
            afkTimer = 0f;
            timerRunning = false;
        }
    }

    //this generates a sphere around the ai to see where it can walk to.
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    //this checks if the ai can combat you.
    void CheckDestinationReached()
    {
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToTarget < destinationReachedTreshold)
        {
            destinationReached = true;
        }
        if (distanceToTarget > destinationReachedTreshold)
        {
            destinationReached = false;
        }
    }

    //these will set the bools for the animations.
    void SetIdle()
    {
        idle = true;
        //set anim to idles
        if (idle && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            anim.SetBool("Idle", true);
            anim.SetBool("walking", false);
        }
        if (!idle && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            anim.SetBool("Idle", false);
        }
    }
    void SetWalking()
    {
        if(walking == true && walking && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("walking"))
        {
            anim.SetBool("walking", true);
            anim.SetBool("Idle", false);
        }
        if(walking == false && !walking && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("walking"))
        {
            anim.SetBool("walking", false);
        }
    }
    void SetAttacking()
    {
        if(attack == true && attack && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("attacking"))
        {
            anim.SetBool("attack", true);
            anim.SetBool("walking", false);
        }
        if (attack == true && !attack && !this.anim.GetCurrentAnimatorStateInfo(0).IsName("attacking"))
        {
            anim.SetBool("attack", false);
            anim.SetBool("walking", true);
        }
    }
}

