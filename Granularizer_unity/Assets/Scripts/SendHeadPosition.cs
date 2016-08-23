using UnityEngine;
using OscJack;
using System.Collections;

public class SendHeadPosition : MonoBehaviour {
    Vector3 pos;
    Vector3 lastPos;

    Quaternion rot;
    Quaternion lastRot;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.frameCount % 3 == 0) {
            pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            rot = gameObject.transform.rotation;
            if (pos != lastPos || rot != lastRot)
            {
                //from http://answers.unity3d.com/questions/416169/finding-pitchrollyaw-from-quaternions.html
                var roll = Mathf.Atan2(2 * rot.y * rot.w - 2 * rot.x * rot.z, 1 - 2 * rot.y * rot.y - 2 * rot.z * rot.z) * Mathf.Rad2Deg;
                //var pitch = Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z) * Mathf.Rad2Deg;
                //var yaw = Mathf.Asin(2 * x * y + 2 * z * w) * Mathf.Rad2Deg;
                OscMaster.SendData("/HeadPosition " + pos.x + " " + pos.y + " " + pos.z + " " + roll);
            }
            lastPos = pos;
            lastRot = rot;
        }
    }
}
