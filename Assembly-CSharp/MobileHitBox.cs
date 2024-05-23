using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class MobileHitBox : MonoBehaviour
{
	// Token: 0x06001D22 RID: 7458 RVA: 0x00088E20 File Offset: 0x00087020
	private void Start()
	{
		if (this.m_boxCollider != null && this.m_isMobile && (!this.m_phoneOnly || UniversalInputManager.UsePhoneUI))
		{
			Vector3 size = default(Vector3);
			size.x = this.m_boxCollider.size.x * this.m_scaleX;
			size.y = this.m_boxCollider.size.y * this.m_scaleY;
			if (this.m_scaleZ == 0f)
			{
				size.z = this.m_boxCollider.size.z * this.m_scaleY;
			}
			else
			{
				size.z = this.m_boxCollider.size.z * this.m_scaleZ;
			}
			this.m_boxCollider.size = size;
			this.m_boxCollider.center += this.m_offset;
			this.m_hasExecuted = true;
		}
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x00088F39 File Offset: 0x00087139
	public bool HasExecuted()
	{
		return this.m_hasExecuted;
	}

	// Token: 0x04000F94 RID: 3988
	public BoxCollider m_boxCollider;

	// Token: 0x04000F95 RID: 3989
	public float m_scaleX = 1f;

	// Token: 0x04000F96 RID: 3990
	public float m_scaleY = 1f;

	// Token: 0x04000F97 RID: 3991
	public float m_scaleZ;

	// Token: 0x04000F98 RID: 3992
	public Vector3 m_offset;

	// Token: 0x04000F99 RID: 3993
	public bool m_phoneOnly;

	// Token: 0x04000F9A RID: 3994
	private bool m_hasExecuted;

	// Token: 0x04000F9B RID: 3995
	private PlatformDependentValue<bool> m_isMobile = new PlatformDependentValue<bool>(PlatformCategory.Screen)
	{
		Tablet = true,
		MiniTablet = true,
		Phone = true,
		PC = false
	};
}
