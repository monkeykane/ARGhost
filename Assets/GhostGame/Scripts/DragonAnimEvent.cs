using UnityEngine;
using System.Collections;

public class DragonAnimEvent : MonoBehaviour
{
	public void OnGhostAttack()
	{
		DragonGhostGame.Instance.OnGhostAttack ();
	}

}
