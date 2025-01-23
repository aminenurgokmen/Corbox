using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    public List<Transform> stackA;  // A içindeki objeler
    public Transform stackBParent; // Stack B'nin parent GameObject'i
    public float moveDuration = 0.5f; // Hareket süresi
    public float flipSpeed = 720f; // Objelerin havada dönme hızı
    public float arcHeight = 2f; // Yay yüksekliği

    private Stack<Transform> stackB = new Stack<Transform>(); // Stack B içindeki objeler


    private void Awake()
    {
        Instance = this;
    }

    public void InitializeStackAWithChildren(Transform parentObject)
    {
        stackA.Clear();

        // Bütün çocuk objeleri stackA'ya ekleyelim
        foreach (Transform child in parentObject)
        {
            child.SetParent(transform, false);
            stackA.Add(child);
        }

        // Flip hareketini başlat
        StartCoroutine(MoveObjectsToB());
    }


    IEnumerator MoveObjectsToB()
    {
        List<Transform> tempList = new List<Transform>(stackA); // **Orijinal listeyi kopyala**
        stackA.Clear(); // **Hemen orijinal listeyi temizle**

        foreach (Transform obj in tempList)
        {
            Vector3 startPos = obj.position;
            Vector3 endPos = stackBParent.position + Vector3.up * stackB.Count * 0.1f;

            float elapsedTime = 0;
            Quaternion startRotation = obj.rotation;

            // **Flip için hedef dönüşü belirleyelim**
            Quaternion flipRotation = Quaternion.Euler(0, 0, 360) * obj.rotation;

            while (elapsedTime < moveDuration)
            {
                float t = elapsedTime / moveDuration;
                t = Mathf.SmoothStep(0, 1, t);

                // **Parabolik yay hareketi**
                float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
                Vector3 midPosition = Vector3.Lerp(startPos, endPos, t) + stackBParent.up * height;

                obj.position = midPosition;

                // **Flip dönüşünü düzgün bir şekilde uygula**
                obj.rotation = Quaternion.Lerp(startRotation, flipRotation, t * 2f); // Daha hızlı dönsün

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            obj.position = endPos;
            obj.rotation = flipRotation; // **Son dönüşü tamamla**
            obj.SetParent(stackBParent); // **Yeni parent'a ekle**

            Destroy(obj.gameObject); // **5 saniye sonra yok et**
        }
    }



}
