using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour {

    public static BallThrower _Instance;

    public GameObject ball;
    public GameObject pivot;
    public GameObject pointOfFire;



    public void ThrowABall()
    {
        Vector3 newAngles = new Vector3(Random.Range(20, -20), Random.Range(20, -20), Random.Range(20, -20));
        //Vector3 newAngles = new Vector3(10, 20, 50);
        pivot.transform.rotation = Quaternion.Euler(newAngles);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = pointOfFire.transform.position;
        ball.transform.rotation = pivot.transform.rotation;
        ball.GetComponent<Rigidbody>().AddForce(pointOfFire.transform.up * 800);

    }

}
