using UnityEngine;
using OscJack;
using System.Collections;

public class ListenerPosition : MonoBehaviour {
    Vector3 pos;
    Vector3 lastPos;

    Vector3 rot;
    Vector3 lastRot;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        rot = gameObject.transform.rotation.eulerAngles;
        if (pos != lastPos || rot != lastRot)
        {
            //rot.x = pitch
            //rot.y = yaw
            //rot.z = roll
            OscMaster.SendData("/listener/position " + pos.x + " " + pos.y + " " + pos.z + " " + rot.z + " " + rot.x + " " + rot.y);
        }
        lastPos = pos;
        lastRot = rot;
    }
}
