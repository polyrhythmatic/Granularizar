using UnityEngine;
using System.Collections;
using OscJack;
using Kvant;

public class Orbit : MonoBehaviour {
    public GameObject orbitCenter;

    Rigidbody rb;

    public string route = "unassigned";

    Spray spray1;
    Spray spray2;

    Vector3 direction;
    Vector3 lastPos;
    Vector3 pos;
    Vector3 trail;
    public Vector3 initialTangent;
    Vector3 binormal;
	// Use this for initialization
	void Start () {
        GameObject Spray1 = transform.Find("Spray 1").gameObject;
        GameObject Spray2 = transform.Find("Spray 2").gameObject;
        spray1 = Spray1.GetComponent("Spray") as Spray;
        spray2 = Spray2.GetComponent("Spray") as Spray;
        rb = GetComponent<Rigidbody>();
        direction = orbitCenter.transform.position - transform.position;

        initialTangent.Normalize();
        binormal = Vector3.Cross(initialTangent, direction);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        direction = orbitCenter.transform.position - transform.position;
        //direction.Normalize();
        rb.AddForce(direction);

        Vector3 tangentForce = Vector3.Cross(direction, binormal);
        rb.AddForce(tangentForce * 30f);
        pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        if (pos != lastPos)
        {
            OscMaster.SendData("/" + route + "/x " + pos.x);
            OscMaster.SendData("/" + route + "/y " + pos.y);
            OscMaster.SendData("/" + route + "/z " + pos.z);
        }
        trail = lastPos - pos;
        trail.Normalize();
        trail = trail * 10f;
        spray1.initialVelocity = trail;
        spray2.initialVelocity = trail;

        lastPos = pos;
    }
    void OnDestroy()
    {
        OscMaster.SendData("/" + route + "/off 1");
    }
}
