using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelBehavior : MonoBehaviour {

    public GameObject explosionFX;
    public float explosionRadius;
    public float explosionDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBall")
        {
            if (explosionFX != null)
            {
                GameObject go =  CFX_SpawnSystem.GetNextObject(explosionFX, false);
                go.transform.position = transform.position;
                go.transform.rotation = Quaternion.identity;
                go.SetActive(true);
            }    

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Attackable"));
            foreach(Collider c in hitColliders)
            {
                if (c.gameObject != gameObject) c.gameObject.SetActive(false);
            }

            Destroy(this.gameObject);
        }
    }
}
