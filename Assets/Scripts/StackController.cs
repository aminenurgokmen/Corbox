using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StackController : MonoBehaviour
{
    public static StackController Instance;
    public Transform stackParent;
    public float stackHeight = 0.5f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 360f;
    public float liftHeight = 3f;
    public AudioSource coin;

    public List<GameObject> stackedObjects = new List<GameObject>();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        stackParent = gameObject.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            AddToStack(other.gameObject);
        }
    }
    void Update()
    {
        if (CharacterScript.Instance.isFall)
        {
            foreach (GameObject obj in stackedObjects)
            {
                obj.transform.SetParent(null);
                if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = false;
                    rb.AddForce(Vector3.up * UnityEngine.Random.Range(2f, 6f), ForceMode.Impulse);
                    rb.AddTorque(new Vector3(UnityEngine.Random.Range(0, 2f), UnityEngine.Random.Range(0, 2f), UnityEngine.Random.Range(0, 2f)), ForceMode.Impulse);
                    obj.GetComponent<BoxCollider>().isTrigger = false;
                }
            }
            stackedObjects.Clear();
        }
    }

    private Vector3 Random(int v1, int v2)
    {
        throw new NotImplementedException();
    }


    private void AddToStack(GameObject obj)
    {
        if (stackedObjects.Contains(obj)) return;

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        stackedObjects.Add(obj);
        Vector3 targetLocalPosition = new Vector3(0, stackedObjects.Count * stackHeight, 0);
        obj.transform.SetParent(stackParent, true);
        StartCoroutine(MoveWithLiftAndRotate(obj, targetLocalPosition));
    }

    private IEnumerator MoveWithLiftAndRotate(GameObject obj, Vector3 targetLocalPosition)
    {
        Vector3 startLocalPosition = obj.transform.localPosition;
        Vector3 liftLocalPosition = startLocalPosition + new Vector3(0, liftHeight, 0);
        Vector3 finalLocalPosition = targetLocalPosition;

        float duration = 1f / moveSpeed;
        float elapsed = 0f;

        Vector3 rotationAxis = GetCharacterMovementAxis();
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            obj.transform.localPosition = Vector3.Lerp(startLocalPosition, liftLocalPosition, elapsed / (duration / 2));
            float rotationAmount = (rotationSpeed / duration) * Time.deltaTime;
            obj.transform.Rotate(rotationAxis, rotationAmount, Space.World);

            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            obj.transform.localPosition = Vector3.Lerp(liftLocalPosition, finalLocalPosition, elapsed / (duration / 2));
            float rotationAmount = (rotationSpeed / duration) * Time.deltaTime;
            obj.transform.Rotate(rotationAxis, rotationAmount, Space.World);

            yield return null;
        }

        obj.transform.localPosition = finalLocalPosition;
        coin.Play();
        obj.transform.rotation = Quaternion.identity;
    }

    private Vector3 GetCharacterMovementAxis()
    {
        Vector3 movementDirection = CharacterMovement.Instance.MoveDirection;

        if (movementDirection.x != 0)
        {
            return Vector3.forward;
        }
        if (-movementDirection.x != 0)
        {
            return Vector3.right;
        }
        if (movementDirection.z != 0)
        {
            return Vector3.left;
        }
        return Vector3.up;

    }


}
