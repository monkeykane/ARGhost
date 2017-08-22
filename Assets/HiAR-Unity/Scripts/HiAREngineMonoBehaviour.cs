
using UnityEngine;

public class HiAREngineMonoBehaviour : HiAREngine
{
    void LateUpdate()
    {
        // Player update.
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit(); // On Android, maps to "back" button.\
    }

    void DownLoadAssetsStart()
    {
        Debug.Log("start");
    }
    void DownLoadAssetsProgress(float progress)
    {
        Debug.Log(progress);
    }
    void DownLoadAssetsComplete()
    {
        Debug.Log("complete");
    }

}
