using UnityEngine;
using VRTK;
using System.Collections;

public class Generator : MonoBehaviour {
    public GameObject prefab;
    public int numObjs = 125;

    GameObject[] orbitals;
    int counter = 0;
    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        print("here");
        GetComponentInParent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
        orbitals = new GameObject[numObjs];
        for (int i = 0; i < numObjs; i++)
        {
            var orb = GameObject.Instantiate(prefab);
            orb.SetActive(false);
            var move = orb.GetComponent<Move>();
            move.indexNum = i + 1;
            move.orbitCenter = GameObject.Find("Camera (eye)");
            orbitals[i] = orb;
        }
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
        orbitals[counter].SetActive(true);
        orbitals[counter].transform.position = gameObject.transform.position;
        orbitals[counter].transform.rotation = gameObject.transform.rotation;
        var orbital = orbitals[counter].GetComponent<Move>();
        orbital.Launch();
        orbital.initialTangent = gameObject.transform.up;

        counter++;
        if(counter > numObjs - 1)
        {
            counter = 0;
            for (int i = 0; i < numObjs; i++)
            {
                var orb = GameObject.Instantiate(prefab);
                orb.SetActive(false);
                var move = orb.GetComponent<Move>();
                move.indexNum = i + 1;
                move.orbitCenter = GameObject.Find("Camera (eye)");
                orbitals[i] = orb;
            }
        }
    }
}
