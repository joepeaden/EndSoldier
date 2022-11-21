using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRotationTest : MonoBehaviour
{
    GameObject reticle;
    // Start is called before the first frame update
    void Start()
    {
        reticle = GameManager.Instance.GetReticleGO();   
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(reticle.transform);
    }
}
