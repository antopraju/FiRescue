using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent
{
    public GameObject fire;
    private PlayerArea playerArea;
    private Animator animator;
    private RayPerception3D rayPerception;
    //private GameObject safePlane;
    List<Vector3> position = new List<Vector3>(); // empty now
    int i = 0;

    private bool hasReached;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Convert actions to axis values
        float forward = vectorAction[0];
        float leftOrRight = 0f;
        if (vectorAction[1] == 1f)
        {
            leftOrRight = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            leftOrRight = 1f;
        }

        // Set animator parameters
        animator.SetFloat("Vertical", forward);
        animator.SetFloat("Horizontal", leftOrRight);

        // Tiny negative reward every step
        //AddReward(-1f / agentParameters.maxStep);
    }

    public override void AgentReset()
    {
        hasReached = false;
        playerArea.ResetArea();
    }

    public override void CollectObservations()
    {
        // Has the penguin eaten
        AddVectorObs(hasReached);

        // Distance to the baby
        //AddVectorObs(Vector3.Distance(safePlane.transform.position, transform.position));

        // Direction to baby
        //AddVectorObs((safePlane.transform.position - transform.position).normalized);

        // Direction penguin is facing
        AddVectorObs(transform.forward);

        // RayPerception (sight)
        // ========================
        // rayDistance: How far to raycast
        // rayAngles: Angles to raycast (0 is right, 90 is forward, 180 is left)
        // detectableObjects: List of tags which correspond to object types agent can see
        // startOffset: Starting height offset of ray from center of agent
        // endOffset: Ending height offset of ray from center of agent
        float rayDistance = 20f;
        float[] rayAngles = { 30f, 60f, 90f, 120f, 150f };
        string[] detectableObjects = { "fire", "safe", "terrain" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
    }

    private void Start()
    {
        playerArea = GetComponentInParent<PlayerArea>();
        position.Add(new Vector3(1f, 1f, 1f));
        //fire = playerArea.fire;
        //safePlane = playerArea.safeArea;
        animator = GetComponent<Animator>();
        rayPerception = GetComponent<RayPerception3D>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("fire"))
        {
            AddReward(-0.3f);
        }
        else if (collision.transform.CompareTag("safe"))
        {
            hasReached = true;
            //playerArea.AddBabyPenguins();
            Destroy(collision.gameObject);
            AddReward(0.5f);
        }
        /*
        else if (collision.transform.CompareTag("wall"))
        {
            hasReached = true;
            //playerArea.AddBabyPenguins();
            AddReward(-0.02f);
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        AddReward(-0.1f);
    }
}
