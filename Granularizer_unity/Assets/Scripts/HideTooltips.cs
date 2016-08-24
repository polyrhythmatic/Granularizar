using UnityEngine;
using System.Collections;

public class HideTooltips : MonoBehaviour
{
    public float tipTime = 10f;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > tipTime)
        {
            gameObject.SetActive(false);
        }
    }
}
