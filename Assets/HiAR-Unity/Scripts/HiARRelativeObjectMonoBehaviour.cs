using UnityEngine;

[RequireComponent(typeof(HiARObjectMovement))]
public class HiARRelativeObjectMonoBehaviour : HiARBaseObjectMonoBehaviour {

    private HiARObjectMovement _movement;

    protected override void Start() {
        base.Start();
        _movement = GetComponent<HiARObjectMovement>();
    }

    protected override void recieveTransform(Matrix4x4 pose, float NFTShowRatio) {
        _movement.setTransform(pose, NFTShowRatio);
    }
    void OnTargetFound(string targetId)
    {
        for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(true);

    }

    void OnTargetTracked(string targetId)
    {

    }

    void OnTargetLost(string targetId)
    {
        for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(false);

    }
}