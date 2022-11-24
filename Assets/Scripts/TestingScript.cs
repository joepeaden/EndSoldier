using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just use this script any time you wanna test something.
/// </summary>
public class TestingScript : MonoBehaviour
{
    public GameObject room;
    public GameObject roomCut;
    public void ToggleWalls()
    {
        room.SetActive(!room.activeInHierarchy);
        roomCut.SetActive(!roomCut.activeInHierarchy);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
