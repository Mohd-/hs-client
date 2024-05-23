using System;
using UnityEngine;

// Token: 0x02000A64 RID: 2660
public class AspectRatioDependentPosition : MonoBehaviour
{
	// Token: 0x06005D51 RID: 23889 RVA: 0x001C031C File Offset: 0x001BE51C
	private void Awake()
	{
		base.transform.localPosition = TransformUtil.GetAspectRatioDependentPosition(this.m_minLocalPosition, base.transform.localPosition);
	}

	// Token: 0x040044FD RID: 17661
	public Vector3 m_minLocalPosition;
}
