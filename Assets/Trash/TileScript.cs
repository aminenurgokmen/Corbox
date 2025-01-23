using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public float rayDistance = 1000f;
    bool sawChar;
    CharacterScript cs;
    private void Start()
    {
        cs = CharacterScript.Instance;
    }

    void Update()
    {
        CheckRaycast(Vector3.right);
        CheckRaycast(Vector3.left);
        CheckRaycast(Vector3.forward);
        CheckRaycast(Vector3.back);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, rayDistance, 1 << 6))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.red);
            Debug.Log(gameObject.name + "girdi");
            sawChar = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * rayDistance, Color.white);
        }
    }



    void CheckRaycast(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, rayDistance, 1 << 7))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * rayDistance, Color.white);


        }
    }
}
