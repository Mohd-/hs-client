using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000D8C RID: 3468
public class LightningCtrl : MonoBehaviour
{
	// Token: 0x06006C39 RID: 27705 RVA: 0x001FD887 File Offset: 0x001FBA87
	private void Start()
	{
	}

	// Token: 0x06006C3A RID: 27706 RVA: 0x001FD88C File Offset: 0x001FBA8C
	private void Update()
	{
		if (UniversalInputManager.Get().GetMouseButtonDown(0))
		{
			this.Spawn(this.target.transform, this.destination.transform);
		}
	}

	// Token: 0x06006C3B RID: 27707 RVA: 0x001FD8C8 File Offset: 0x001FBAC8
	public void Spawn(Transform targetTransform, Transform destinationTransform)
	{
		this.lightningObj = (GameObject)Object.Instantiate(this.mylightning, new Vector3(this.position_X, this.position_Y, this.position_Z), new Quaternion(0f, 0f, 0f, 0f));
		this.lightningObj.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
		ElectroScript component = this.lightningObj.GetComponent<ElectroScript>();
		component.timers.timeToPowerUp = this.speed;
		component.prefabs.target.position = targetTransform.position;
		component.prefabs.destination.position = destinationTransform.position;
		base.StartCoroutine(this.DestroyLightning());
	}

	// Token: 0x06006C3C RID: 27708 RVA: 0x001FD998 File Offset: 0x001FBB98
	private IEnumerator DestroyLightning()
	{
		yield return new WaitForSeconds(this.lifetime);
		Object.Destroy(this.lightningObj);
		yield break;
	}

	// Token: 0x040054CD RID: 21709
	public GameObject mylightning;

	// Token: 0x040054CE RID: 21710
	private GameObject lightningObj;

	// Token: 0x040054CF RID: 21711
	public float lifetime = 1f;

	// Token: 0x040054D0 RID: 21712
	public float position_X;

	// Token: 0x040054D1 RID: 21713
	public float position_Y;

	// Token: 0x040054D2 RID: 21714
	public float position_Z;

	// Token: 0x040054D3 RID: 21715
	public float scale = 0.1f;

	// Token: 0x040054D4 RID: 21716
	public float speed = 1f;

	// Token: 0x040054D5 RID: 21717
	public GameObject target;

	// Token: 0x040054D6 RID: 21718
	public GameObject destination;
}
