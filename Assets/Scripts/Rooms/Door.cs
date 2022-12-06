using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public float openForce;

    private bool isOpen;

    public override void Interact()
    {
        //eventually really should replace this with an animation. It's probably not very good for performance to use physics for something unnecessary like this.
        GetComponent<Rigidbody>().AddForce(isOpen ? openForce * -transform.right : openForce * transform.right);
        isOpen = !isOpen;
        //gameObject.SetActive(false);
        //StartCoroutine("OpenAnimation");
    }

    //private IEnumerator OpenAnimation()
    //{
    //    yield return null;
    //}
}
