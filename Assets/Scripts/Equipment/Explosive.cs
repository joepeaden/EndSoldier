using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public ExplosiveData data;
    private AudioSource audioSource;
    private Collider collision;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collision = GetComponent<Collider>();

        StartCoroutine("Explode");
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(3f);

        // generate an explosive force
        Vector3 explosionPos = transform.position;

        // apply it to the rigidbody
        Collider[] colliders = Physics.OverlapSphere(explosionPos, data.explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.GetComponent<Shatter>())
            {
                hit.GetComponent<Shatter>().ShatterObject();
            }
        }

        colliders = Physics.OverlapSphere(explosionPos, data.explosionRadius);
        foreach (Collider hit in colliders)
        {

            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(data.explosionPower, explosionPos, data.explosionRadius, data.upwardsForce);
        }

        Instantiate(data.explosionPrefab, transform.position, transform.rotation);

        audioSource.Play();
        GetComponent<MeshRenderer>().enabled = false;

        StartCoroutine("DestroyObject");

    }

    // wait 3 seconds before destroying object
    private IEnumerator DestroyObject()
    {
        GameManager.Instance.StartSlowMotion(.25f);

        collision.enabled = false;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
