using System;
using UnityEngine;

// Token: 0x02000ECB RID: 3787
public class GvG_Promotion : MonoBehaviour
{
	// Token: 0x060071A7 RID: 29095 RVA: 0x00216984 File Offset: 0x00214B84
	private void Start()
	{
		this.m_arenaObj = Box.Get().m_ForgeButton.gameObject;
		base.transform.parent = this.m_arenaObj.transform;
		Animation component = this.m_arenaObj.GetComponent<Animation>();
		this.m_AnimExisted = true;
		if (component == null)
		{
			this.m_arenaObj.AddComponent<Animation>();
			this.m_AnimExisted = false;
		}
		base.GetComponent<Spell>().Activate();
	}

	// Token: 0x060071A8 RID: 29096 RVA: 0x002169FC File Offset: 0x00214BFC
	private void OnDestroy()
	{
		this.m_arenaObj.transform.localPosition = new Vector3(-0.004992113f, 1.260711f, 0.4331615f);
		this.m_arenaObj.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_arenaObj.transform.localRotation = new Quaternion(0f, -180f, 0f, 0f);
		if (!this.m_AnimExisted)
		{
			Animation component = this.m_arenaObj.GetComponent<Animation>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	// Token: 0x04005BE0 RID: 23520
	private GameObject m_arenaObj;

	// Token: 0x04005BE1 RID: 23521
	private bool m_AnimExisted;
}
