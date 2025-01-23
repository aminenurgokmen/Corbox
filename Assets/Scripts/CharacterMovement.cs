using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement Instance;
    private Vector2 startTouchPosition, endTouchPosition;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private float moveSpeed = 10f;
    public LayerMask tileLayer;
    public LayerMask obstacleLayer;
    public LayerMask deadlyObstacleLayer;
    public Transform stackPoint;

    private List<GameObject> collectedCoins = new List<GameObject>();
    private List<Vector3> pathTiles = new List<Vector3>();
    private Vector3 moveDirection;
    public Vector3 MoveDirection => moveDirection;
    private bool isGameOver = false;

    private Stack<Transform> stackB = new Stack<Transform>();
    public Transform stackBParent;
    public float moveDuration = 0.1f;
    public float flipSpeed = 1080 * 2;
    public float arcHeight = 2f;
    List<GameObject> toMove;

    private void Awake()
    {
        Instance = this;
        HideAllTiles();
    }

    void Update()
    {
        if (!isGameOver)
        {
            HandleInput();
            MoveCharacter();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude > 50)
        {
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                if (swipeDelta.x > 0)
                {
                    moveDirection = Vector3.right;
                    TryMove(Vector3.right);
                }
                else if (swipeDelta.x < 0)
                {
                    moveDirection = Vector3.left;
                    TryMove(Vector3.left);
                }
            }
            else
            {
                if (swipeDelta.y > 0)
                {
                    moveDirection = Vector3.forward;
                    TryMove(Vector3.forward);
                }
                else if (swipeDelta.y < 0)
                {
                    moveDirection = Vector3.back;
                    TryMove(Vector3.back);
                }
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        if (isMoving) return;

        Vector3 currentPos = transform.position;
        Vector3 nextPos = currentPos + direction;

        pathTiles.Clear();

        while (CanMove(nextPos))
        {
            pathTiles.Add(nextPos);
            currentPos = nextPos;
            nextPos += direction;
        }

        if (pathTiles.Count > 0)
        {
            targetPosition = pathTiles[pathTiles.Count - 1];
            isMoving = true;
        }
    }

    bool CanMove(Vector3 checkPosition)
    {
        Collider[] tileCheck = Physics.OverlapSphere(checkPosition, 0.1f, tileLayer);
        Collider[] obstacleCheck = Physics.OverlapSphere(checkPosition, 0.1f, obstacleLayer);

        return tileCheck.Length > 0 && obstacleCheck.Length == 0;
    }

    void MoveCharacter()
    {
        if (isMoving && pathTiles.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, pathTiles[0], moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, pathTiles[0]) < 0.01f)
            {
                transform.position = pathTiles[0];
                ActivateCurrentTile(transform.position);
                pathTiles.RemoveAt(0);

                if (CheckForDeadlyObstacle(transform.position))
                {
                    GameOver();
                    return;
                }

                if (pathTiles.Count == 0)
                {
                    isMoving = false;
                }
            }
        }
    }

    bool CheckForDeadlyObstacle(Vector3 position)
    {
        Collider[] deadlyObstacleCheck = Physics.OverlapSphere(position, 0.1f, deadlyObstacleLayer);
        return deadlyObstacleCheck.Length > 0;
    }

    void HideAllTiles()
    {
        Collider[] allTiles = Physics.OverlapSphere(transform.position, 100f, tileLayer);
        foreach (Collider tile in allTiles)
        {
            MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }
        }
    }

    void ActivateCurrentTile(Vector3 position)
    {
        Collider[] tileCheck = Physics.OverlapSphere(position, 0.1f, tileLayer);
        foreach (Collider tile in tileCheck)
        {
            MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.enabled = true;
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");
        gameObject.SetActive(false);
    }
    public void Succes()
    {
        Debug.Log("Succes!");
        toMove = StackController.Instance.stackedObjects;
        StartCoroutine(MoveObjectsToB());
        UIManager.Instance.ShowSuccesPanel2();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Succes();
        }
    }

    public IEnumerator MoveObjectsToB()
    {

        for (int i = toMove.Count - 1; i >= 0; i--)
        {
            GameObject obj = toMove[i];
            if (obj == null) continue;

            Transform objTransform = obj.transform;
            Vector3 startPos = objTransform.position;
            Vector3 endPos = stackBParent.position + Vector3.up * stackB.Count * objTransform.localScale.y * 0.007f;


            float elapsedTime = 0;
            Quaternion startRotation = objTransform.rotation;
            Quaternion endRotation = Quaternion.Euler(0, 0, -180) * objTransform.rotation;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                t = Mathf.SmoothStep(0, 1, t);

                float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
                Vector3 midPosition = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

                objTransform.position = midPosition;
                objTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objTransform.position = new Vector3(endPos.x, endPos.y - 1, endPos.z); // End pos -1 verirsek objTransform.localScale yüzünden liste eksi olup sapıtıyor. Ne yapacağımı bulamadığımdan burayı -1 verdim. Ehehe
            objTransform.rotation = Quaternion.Euler(0, 0, 180);
            objTransform.SetParent(stackBParent);
            stackB.Push(objTransform);
        }

        StackController.Instance.stackedObjects.Clear();
    }
}
