using System;
using UnityEngine;

// Token: 0x02000D84 RID: 3460
public class CameraSelector : MonoBehaviour
{
	// Token: 0x06006C29 RID: 27689 RVA: 0x001FC6A8 File Offset: 0x001FA8A8
	private void Start()
	{
		if (this.activateOnStart)
		{
			Camera.main.transform.rotation = Quaternion.Euler(this.cameraRotation);
			Camera.main.transform.position = this.cameraPosition;
		}
	}

	// Token: 0x06006C2A RID: 27690 RVA: 0x001FC6F0 File Offset: 0x001FA8F0
	private void OnMouseDown()
	{
		Camera.main.transform.rotation = Quaternion.Euler(this.cameraRotation);
		Camera.main.transform.position = this.cameraPosition;
	}

	// Token: 0x040054A8 RID: 21672
	public Vector3 cameraPosition;

	// Token: 0x040054A9 RID: 21673
	public Vector3 cameraRotation;

	// Token: 0x040054AA RID: 21674
	public bool activateOnStart;
}
