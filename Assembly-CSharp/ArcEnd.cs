using System;
using UnityEngine;

// Token: 0x02000D81 RID: 3457
public class ArcEnd : MonoBehaviour
{
	// Token: 0x06006C20 RID: 27680 RVA: 0x001FC520 File Offset: 0x001FA720
	private void Start()
	{
		this.s = base.transform.localScale;
	}

	// Token: 0x06006C21 RID: 27681 RVA: 0x001FC534 File Offset: 0x001FA734
	private void FixedUpdate()
	{
		Vector3 vector = Camera.main.transform.position - base.transform.position;
		Quaternion rotation = Quaternion.LookRotation(Vector3.up, vector);
		base.transform.rotation = rotation;
		base.transform.Rotate(Vector3.up, Random.value * 360f);
		if (Random.value > 0.8f)
		{
			base.transform.localScale = this.s * 1.5f;
			if (this.l != null)
			{
				this.l.range = 100f;
				this.l.intensity = 1.5f;
			}
		}
		else
		{
			base.transform.localScale = this.s;
			if (this.l != null)
			{
				this.l.range = 50f;
				this.l.intensity = 1f;
			}
		}
	}

	// Token: 0x040054A5 RID: 21669
	private Vector3 s;

	// Token: 0x040054A6 RID: 21670
	public Light l;
}
