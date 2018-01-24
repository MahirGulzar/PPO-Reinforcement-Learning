using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherAgent: Agent {

    public BallThrower bT;
    public float currentDistance;



    private int speed=10;
    private GameObject Goal;
    private int solved = 0;
    private Vector3 initPosition;


    private int progressChecker = 5;
    private float previous_best;
    private float sensor = 5;


    private void Awake()
    {
        initPosition = this.transform.position;
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

        //state.Add(goalVelocity.x);
        //state.Add(goalVelocity.y);
        //state.Add(goalVelocity.z);
        //state.Add((Goal.transform.position.x - gameObject.transform.position.x));
        //state.Add((Goal.transform.position.y - gameObject.transform.position.y));
        //state.Add((Goal.transform.position.z - gameObject.transform.position.z));
        //state.Add(previous_best);

        //state.Add(velocity.x);
        //state.Add(velocity.z);


        var ballLoc = Goal.transform.position - gameObject.transform.position;
        state.AddRange(new List<float>() { ballLoc.x, ballLoc.y, ballLoc.z });

        state.AddRange(new List<float>() { goalVelocity.x, goalVelocity.y, goalVelocity.z });

        state.AddRange(new List<float>() { velocity.x, velocity.z });

        //var paddleLoc = gameObject.transform.position - initPosition;
        //state.AddRange(getFloatsXy(paddleLoc, 2.5f));


        state.Add(previous_best);

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
                if (sensor != 0.005)
                {
                    sensor-=0.005f;
                }
                else
                {
                    sensor = 0.5f;
                    //print("sensing stoped");
                }
                //print("Yo......");
                reward += 0.2f;
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
            progressChecker = 5;
        }

        
        if (done == false)
        {
            if (done == false) reward += -0.005f;
        }



        if (currentDistance > 30)
        {
            reward = -10f;
            done = true;
            return;
        }

        if (currentDistance <= 0.8f)
        {
            reward = 10;
            done = true;
            solved++;
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
    }



    
}
