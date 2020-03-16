using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class DogAgent : Agent
{
    public GameObject heartPrefab;
    private ForestArea forestArea;
    private Animation animator;
    private GameObject safeZone;
    private AudioSource animalSound;
    private AudioSource dogSound;
    private float movementSpeed = 2.5f;
    private int saved_id = 0;
    private int jump_second;
    private bool jumped = false;
    private Rigidbody rb;
    private bool isFull; 
    private float health = 0f;

    public override void InitializeAgent(){
        base.InitializeAgent();
        forestArea = GetComponentInParent<ForestArea>();
        animator = GetComponent<Animation>();
        animalSound = forestArea.animalSound;
        dogSound = forestArea.dogSound;
        rb = GetComponent<Rigidbody>();
        safeZone = forestArea.safeZone;
        safeZone.GetComponent<Animation>().CrossFade("idle");
        saved_id = 0;
    }

    public override void AgentAction(float[] vectorAction)
    {
        float moveHorizontal = vectorAction[1];
        float moveVertical = vectorAction[0];
        Debug.Log(moveHorizontal);
        if (moveHorizontal == 2)
        {
            moveHorizontal = -1;
        }
        if (moveVertical == 2)
        {
            moveVertical = -1;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
        {
            if (moveVertical != -1)
            {
                animator.CrossFade("run");
                animator["run"].speed = 2f;
            }
            if (moveHorizontal != 0)
            {
                transform.Rotate(0, moveHorizontal * 2, 0, Space.World);
            }

        }
        else
        {
            animator.CrossFade("stand");
        }
        if (moveVertical == 1)
        {
            transform.Translate(movement * movementSpeed * Time.deltaTime);
        }
        else if (moveVertical == -1)
        {
            animator.CrossFade("stand");
        }
        // Tiny negative reward every step
        AddReward(-1f / maxStep);
    }

    public override void AgentReset()
    {
        isFull = false;
        forestArea.ResetArea();
    }

    public void updateHealth(){
        health -= 1 * 0.1f;
        if(health <= 0f){
            AddReward(-1f);
            Done();
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(isFull);
        AddVectorObs(Vector3.Distance(safeZone.transform.position, transform.position));
        AddVectorObs((safeZone.transform.position - transform.position).normalized);
        AddVectorObs(transform.forward);
    }

    //TODO
    public override float[] Heuristic(){
        Debug.Log("Heuristics mode");
        if(Input.GetKey(KeyCode.W)){
            return new float[2]{1,0};
        }
        else if(Input.GetKey(KeyCode.A)){
            return new float[2]{0,2};
        }
        else if(Input.GetKey(KeyCode.S)){
            return new float[2]{2,0};    
        }
        else if(Input.GetKey(KeyCode.D)){
            return new float[2]{0,1};
        }
        else
            return new float[2]{0,0};
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, safeZone.transform.position) < forestArea.feedRadius)
        {
            // Close enough, try to save the animal
            DropAnimal(saved_id);
        }
        if (jumped && System.DateTime.Now.Second - jump_second >= 2)
        {
            safeZone.GetComponent<Animation>().Play();
            jumped = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.collider.tag){
            case "saveRabbit":{
                saved_id = 0;
                PickAnimal(collision.gameObject);
                AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
                break;
            }

            case "saveSquirrel":{
                saved_id = 1;
                PickAnimal(collision.gameObject);
                AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
                break;
            }
            
            case "safeZone":{
                DropAnimal(saved_id);
                AddVectorObs(Vector3.Distance(collision.transform.position, transform.position));
                break;
            }

            case "wall":{
                //make it move and add small negative reward
                //Done();
                break;
            }
        }
    }

    private void PickAnimal(GameObject animalObject)
    {
        if (isFull) return; // Can't save another animal while full
        isFull = true;

        forestArea.RemoveSpecificAnimal(animalObject);
        dogSound.Play();
        AddReward(0.5f);
    }

    private void DropAnimal(int i)
    {
        if (!isFull)
        {
            return; // Nothing to save
        }
        jumped = true;
        isFull = false;
        // Spawn heart
        GameObject heart = Instantiate<GameObject>(heartPrefab);
        heart.transform.parent = transform.parent;
        heart.transform.position = safeZone.transform.position + Vector3.up;
        safeZone.GetComponent<Animation>()["jump"].speed = 0.3f;
        safeZone.GetComponent<Animation>().CrossFade("jump");
        jump_second = System.DateTime.Now.Second;
        Destroy(heart, 4f);
        animalSound.Play();
        forestArea.SaveAnimal(transform.position, i);
        AddReward(0.5f);
    }

    /**
        Enters fire
    */
    private void OnTriggerEnter(Collider other)
    {
        dogSound.Play();
        AddVectorObs(Vector3.Distance(other.transform.position, transform.position));
        AddReward(-0.5f);
    }

    public void variableRewards(){

    }
}
