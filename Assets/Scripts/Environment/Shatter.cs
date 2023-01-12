using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    public GameObject shatterObject;

    public void ShatterObject()
    {
        Instantiate(shatterObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
