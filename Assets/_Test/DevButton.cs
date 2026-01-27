using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevButton : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ChangeSceneAfterDelay(string scene)
    {
        StartCoroutine(DelayedChange(scene));
    }

    public IEnumerator DelayedChange(string scene)
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(scene);
    }


}
