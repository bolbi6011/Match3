using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void RestarScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void SceneLoad(string name)
    {
        SceneManager.LoadScene(name);
    }

}
