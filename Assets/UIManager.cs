using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject succesPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowSuccesPanel2()
    {
        StartCoroutine(ShowSuccesPanel());
    }
    IEnumerator ShowSuccesPanel()
    {
        yield return new WaitForSeconds(5f);
        succesPanel.SetActive(true);
    }

    public void LoadLevel()
    {
        // load the nextlevel
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
