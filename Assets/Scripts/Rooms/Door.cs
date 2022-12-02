using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public override void Interact()
    {
        gameObject.SetActive(false);
        //StartCoroutine("OpenAnimation");
    }

    //private IEnumerator OpenAnimation()
    //{
    //    yield return null;
    //}
}
