using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DragonLoadingManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("LoadGameScene");
	}

    IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(4);
        yield break;
    }
}
