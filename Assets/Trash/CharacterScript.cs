using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public static CharacterScript Instance;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f;
    public float moveSpeed = 5f;
    private Vector3 moveDirection; // Bu internal bir değişken olarak kalacak
    public Vector3 MoveDirection => moveDirection; // Public bir getter ekledik
    public bool isMoving;
    public bool canSwipe;
    public string lastSwipeDirection;
    public AudioSource wind;
    public bool isFall, isFinish;
    public GameObject lastTile;
    private Rigidbody rb;
    public GameObject stack;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canSwipe = true;
        lastSwipeDirection = null;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isFinish)
        {
            moveSpeed = 0;
            //   transform.position = lastTile.transform.position + new Vector3(0, 0.2f, 0);

            if (StackManager.Instance != null)
            {
                StackManager.Instance.InitializeStackAWithChildren(transform);
            }
        }

        if (canSwipe && !isMoving)
        {

            if (Input.GetMouseButtonDown(0))
            {
                startTouchPosition = Input.mousePosition;


            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchPosition = Input.mousePosition;
                wind.Play();

                DetectSwipe();
            }
        }

        if (isMoving)
        {

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, transform.position + moveDirection) < 0.1f)
            {
                isMoving = false;
                canSwipe = true;
            }
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 100, 1 << 6))
        {
            rb.useGravity = false;
            isFall = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.red);
            isFinish = false;
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 100, 1 << 7))
        {
            rb.useGravity = false;
            isFall = false;
            isFinish = true;
            moveSpeed = 0;
            transform.position = lastTile.transform.position + new Vector3(0, 0.2f, 0);
        }
        else
        {
            GetComponent<BoxCollider>().isTrigger = false;
            moveSpeed = 2;
            rb.useGravity = true;
            isFall = true;
            isFinish = false;
        }
    }


    void DetectSwipe()
    {
        float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;
        float swipeDistanceY = endTouchPosition.y - startTouchPosition.y;

        if (Mathf.Abs(swipeDistanceX) > Mathf.Abs(swipeDistanceY) && Mathf.Abs(swipeDistanceX) > minSwipeDistance)
        {
            if (swipeDistanceX > 0)
            {
                if (lastSwipeDirection != "Right")
                {
                    isMoving = true;
                    canSwipe = false;
                    moveDirection = Vector3.right;
                    lastSwipeDirection = "Right";
                }
            }
            else if (swipeDistanceX < 0)
            {
                if (lastSwipeDirection != "Left")
                {
                    isMoving = true;
                    canSwipe = false;
                    moveDirection = Vector3.left;
                    lastSwipeDirection = "Left";
                }
            }
        }
        else if (Mathf.Abs(swipeDistanceY) > minSwipeDistance)
        {
            if (swipeDistanceY > 0)
            {
                if (lastSwipeDirection != "Up")
                {
                    isMoving = true;
                    canSwipe = false;
                    moveDirection = Vector3.forward;
                }
            }
            else if (swipeDistanceY < 0)
            {
                if (lastSwipeDirection != "Down")
                {
                    isMoving = true;
                    canSwipe = false;
                    moveDirection = Vector3.back;
                    lastSwipeDirection = "Down";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            isMoving = false;

            Vector3 roundedPosition = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                Mathf.Round(transform.position.z)
            );

            transform.position = roundedPosition;

            canSwipe = true;

            if (moveDirection == Vector3.right)
            {
                lastSwipeDirection = "Right";
            }
            else if (moveDirection == Vector3.left)
            {
                lastSwipeDirection = "Left";
            }
            else if (moveDirection == Vector3.forward)
            {
                lastSwipeDirection = "Up";
            }
            else if (moveDirection == Vector3.back)
            {
                lastSwipeDirection = "Down";
            }
        }
    }
}
