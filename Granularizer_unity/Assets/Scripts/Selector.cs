using UnityEngine;
using VRTK;
using System.Collections;

public class Selector : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GetComponentInParent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);

    }

    // Update is called once per frame
    void Update () {
	
	}

    void SelectZero()
    {
        print("Zero selected");
    }

    void SelectOne()
    {
        print("OneSelected");
    }

    void SelectTwo()
    {
        print("Two selected");
    }
    
    void DoTriggerClicked(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        print("trigger");
    }
}
