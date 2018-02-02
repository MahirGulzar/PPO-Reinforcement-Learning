using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherAgent: Agent {

    public BallThrower bT;
    public BreakoutAcademy b_academy;
    public GameObject area;
    public float currentDistance;
    


    private int speed=10;
    private GameObject Goal;
    private int solved = 0;
    private Vector3 initPosition;


    private int progressChecker = 5;
    private float previous_best;
    //private float rangeBest;
    private float sensor = 4;




    private void Awake()
    {
        initPosition = this.transform.position;
        b_academy = GameObject.FindObjectOfType<BreakoutAcademy>();
        area = this.transform.parent.gameObject;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward,sensor);
    }

    public List<float> getFloatsXy(Vector3 target, float normDivisor)
    {
        var result = new List<float>();
        result.Add(target.x / normDivisor);
        result.Add(target.y / normDivisor);
        return result;
    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        Vector3 goalVelocity = Goal.GetComponent<Rigidbody>().velocity;

        // state: ball location, agent location
        // alternative state: ball relative location
        // action: forward, backward, left, right
        // reward: 1 when catches the ball + episode ends, 0 when does not touch the ball + timeout after n steps
        // additional reward: -0.01 for each timestep


        //var ballLoc = Goal.transform.position - gameObject.transform.position;
        //state.AddRange(new List<float>() { ballLoc.x, ballLoc.y, ballLoc.z });

        state.Add((transform.position.x - area.transform.position.x));
        state.Add((transform.position.y - area.transform.position.y));
        state.Add((transform.position.z + 5 - area.transform.position.z));

        state.Add((Goal.transform.position.x - area.transform.position.x));
        state.Add((Goal.transform.position.y - area.transform.position.y));
        state.Add((Goal.transform.position.z + 5 - area.transform.position.z));

        state.Add((transform.position.x - area.transform.position.x));
        state.Add((transform.position.y - area.transform.position.y));
        state.Add((transform.position.z + 5 - area.transform.position.z));

        state.AddRange(new List<float>() { goalVelocity.x, goalVelocity.y, goalVelocity.z });

        state.AddRange(new List<float>() { velocity.x, velocity.z });



        //state.Add(rangeBest);
        state.Add(previous_best);
        //state.Add(currentDistance);
        state.Add(sensor);

        

        return state;
    }



    public void MoveAgent(float[] act)
    {
        float directionX = 0;
        float directionZ = 0;

        RaycastHit hit;
        //Debug.DrawRay(transform.position,transform.TransformDirection(Vector3.forward)*10, Color.green, Time.deltaTime, false);
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f))
        //{
            
        //    if (hit.collider.gameObject == Goal)
        //    {
        //        print("Yo......");
        //        reward += 0.1f;
        //    }
        //}


       

        if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            directionX = Mathf.Clamp(act[0], -1f, 1f);
            directionZ = Mathf.Clamp(act[1], -1f, 1f);
        }
        else
        {
            int movement = Mathf.FloorToInt(act[0]);
            if (movement == 0) { directionX = -1; }
            if (movement == 1) { directionX = 1; }
            if (movement == 2) { directionZ = 1; }

            if (movement == 3) { directionZ = -1; }

        }


        gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(directionX * 40f, 0, directionZ * 40f));
        if (GetComponent<Rigidbody>().velocity.sqrMagnitude > 25f)
        {
            GetComponent<Rigidbody>().velocity *= 0.95f;
        }

        if (Physics.SphereCast(transform.position, sensor, transform.forward, out hit))
        {
            if (hit.collider.gameObject == Goal)
            {
               
                
                if (sensor > 0.05)
                {
                    sensor-=0.05f;
                }
                else
                {
                    sensor = 0.05f;
                    //print("sensing stoped");
                }
                //print("Yo......");

                if (progressChecker == 5)
                {
                    reward += 0.2f;
                    
                }

            }
        }
    }

    public override void AgentStep(float[] action)
    {
        MoveAgent(action);

        currentDistance = Vector3.Distance(this.transform.position, Goal.transform.position);

        if(progressChecker==5)
        {
            if (previous_best > currentDistance)
            {
                previous_best = currentDistance;
            }
        }

        progressChecker--;
        //print(progressChecker);
        if(progressChecker==0)
        {
            
            if (previous_best>currentDistance)
            {
                reward += 0.1f;
                //print("yes");
            }
            //else
            //{
            //    reward -= 0.05f;
            //}
            progressChecker = 5;
        }


        if (done == false)
        {
            if (done == false) reward += -0.005f;
        }



        if (currentDistance > 30)
        {
            reward = -1000f;
            done = true;
            b_academy.failCount++;
            return;
        }

        if (currentDistance <= 0.8f)
        {
            reward = 1000;
            done = true;
            solved++;
            b_academy.successCount++;
            //print(solved);
            return;
        }
        

    }

    public override void AgentReset()
    {
        bT.ThrowABall();
        Goal = bT.ball;
        sensor = 4;
        progressChecker = 5;
        this.transform.position = initPosition;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentDistance = Vector3.Distance(this.transform.position,Goal.transform.position);
        previous_best = currentDistance;
        //rangeBest = currentDistance;
    }



    
}
