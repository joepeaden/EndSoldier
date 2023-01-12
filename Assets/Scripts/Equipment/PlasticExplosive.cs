using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticExplosive : MonoBehaviour
{
    public float explosionRadius;
    public float explosionPower;
    public float upwardsForce;
    public GameObject explosionPrefab;

    private AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine("Explode");
    }

    private IEnumerator Explode() {
        yield return new WaitForSeconds(3f);

        // generate an explosive force
        Vector3 explosionPos = transform.position;

        // apply it to the rigidbody
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders) {
            if (hit.GetComponent<Shatter>())
            {
                hit.GetComponent<Shatter>().ShatterObject();
            }
        }

        colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders) {
        
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius, upwardsForce);
        }

        Instantiate(explosionPrefab, transform.position, transform.rotation);

        audioSource.Play();
        GetComponent<MeshRenderer>().enabled = false;
        
        StartCoroutine("DestroyObject");
        
    }

    // wait 3 seconds before destroying object
    private IEnumerator DestroyObject() {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}

