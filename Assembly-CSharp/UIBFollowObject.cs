using System;
using UnityEngine;

// Token: 0x02000556 RID: 1366
[CustomEditClass]
[ExecuteInEditMode]
public class UIBFollowObject : MonoBehaviour
{
	// Token: 0x06003E8D RID: 16013 RVA: 0x0012E89C File Offset: 0x0012CA9C
	public void UpdateFollowPosition()
	{
		if (this.m_rootObject == null || this.m_objectToFollow == null)
		{
			return;
		}
		Vector3 vector = this.m_objectToFollow.transform.position;
		if (this.m_offset.sqrMagnitude > 0f)
		{
			vector += this.m_objectToFollow.transform.localToWorldMatrix * this.m_offset;
		}
		this.m_rootObject.transform.position = vector;
	}

	// Token: 0x04002819 RID: 10265
	public GameObject m_rootObject;

	// Token: 0x0400281A RID: 10266
	public GameObject m_objectToFollow;

	// Token: 0x0400281B RID: 10267
	public Vector3 m_offset;

	// Token: 0x0400281C RID: 10268
	public bool m_useWorldOffset;
}
