using UnityEngine;
using OscJack;
using System.Collections;

public class Move : MonoBehaviour {

    Rigidbody rb;

    Vector3 direction;
    Vector3 lastPos;
    Vector3 pos;
    Vector3 trail;
    Vector3 binormal;
    public GameObject orbitCenter;

    public Vector3 initialTangent;
    public int indexNum;

	void Start () {
        orbitCenter = GameObject.Find("Camera (eye)");
        StartCoroutine("SetCenter");
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    void FixedUpdate ()
    {
        if (gameObject.activeSelf)
        {
            direction = orbitCenter.transform.position - gameObject.transform.position;
            rb.AddForce(direction);

            Vector3 tangentForce = Vector3.Cross(direction, binormal);
            rb.AddForce(tangentForce * 100f);
            pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            if(pos != lastPos && Time.frameCount % 3 == 0)
            {
                OscMaster.SendData("/source/position " + indexNum + " " + pos.x + " " + pos.y + " " + pos.z);
            }
            lastPos = pos;
        }
    }

    public void Launch()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // orbitCenter = GameObject.Find("Camera (eye)");
        print(indexNum);
        direction = orbitCenter.transform.position - gameObject.transform.position;
        initialTangent.Normalize();
        binormal = Vector3.Cross(initialTangent, direction);
        rb.AddForce(gameObject.transform.forward * 1000);
        object[] parms = new object[2] { 10f, Time.time };
        StartCoroutine("Deactivate", parms);
        OscMaster.SendData("/source/start " + indexNum);
    }

    IEnumerator Deactivate(object[] parms)
    {
        while(Time.time < (float)parms[0] + (float)parms[1])
        {
            yield return null;
        }
        gameObject.SetActive(false);
        OscMaster.SendData("/source/stop " + indexNum); //here we tell maxmsp to turn off the sound
    }

    IEnumerator SetCenter()
    {
        while (Time.time < 2f)
        {
            yield return null;
        }
        print(orbitCenter);
    }
}
