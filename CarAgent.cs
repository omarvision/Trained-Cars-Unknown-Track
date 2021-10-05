using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DecisionRequester))]
public class CarAgent : Agent
{
    private float Movespeed = 30;
    private float Turnspeed = 120;
    private Vector3 recall_position;
    private Quaternion recall_rotation;
    private Rigidbody rb = null;
    private Checkpoints CheckpointScript = null;
    private string CurrentWall;

    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        recall_position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        recall_rotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        CheckpointScript = GameObject.Find("Checkpoints").GetComponent<Checkpoints>();
    }
    public override void OnEpisodeBegin()
    {
        this.transform.position = recall_position;
        this.transform.rotation = recall_rotation;
        rb.velocity = Vector3.zero;
        CurrentWall = "wall (0)";
    }
    public override void Heuristic(float[] actionsOut)
    {
        float move = Input.GetAxis("Vertical");     //-1..0..1
        float turn = Input.GetAxis("Horizontal");

        if (move < 0)
            actionsOut[0] = 0;
        else if (move == 0)
            actionsOut[0] = 1;
        else if (move > 0)
            actionsOut[0] = 2;

        if (turn < 0)
            actionsOut[1] = 0;
        else if (turn == 0)
            actionsOut[1] = 1;
        else if (turn > 0)
            actionsOut[1] = 2;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        switch (vectorAction[0])
        {
            case 0: //back
                rb.AddRelativeForce(Vector3.back * Movespeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(-0.001f);
                break;
            case 1: //noaction
                AddReward(-0.1f);
                break;
            case 2: //forward
                rb.AddRelativeForce(Vector3.forward * Movespeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(0.00005f);
                break;
        }

        switch (vectorAction[1])
        {
            case 0: //left
                this.transform.Rotate(Vector3.up, -Turnspeed * Time.deltaTime);
                break;
            case 1: //noaction
                break;
            case 2: //right
                this.transform.Rotate(Vector3.up, Turnspeed * Time.deltaTime);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier") == true)
        {
            AddReward(-2.0f);
            EndEpisode();
        }

        if (other.CompareTag("Checkpoint") == true)
        {
            if (other.name == CurrentWall)
            {
                AddReward(1.0f);
                CurrentWall = CheckpointScript.GetNextCheckpointName(CurrentWall);
            }
        }
    }
}
