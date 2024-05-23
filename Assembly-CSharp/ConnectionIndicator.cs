using System;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class ConnectionIndicator : MonoBehaviour
{
	// Token: 0x06003ABF RID: 15039 RVA: 0x0011C214 File Offset: 0x0011A414
	private void Awake()
	{
		ConnectionIndicator.s_instance = this;
		this.m_active = false;
		this.m_indicator.SetActive(false);
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x0011C22F File Offset: 0x0011A42F
	private void OnDestroy()
	{
		ConnectionIndicator.s_instance = null;
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x0011C237 File Offset: 0x0011A437
	public static ConnectionIndicator Get()
	{
		return ConnectionIndicator.s_instance;
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x0011C240 File Offset: 0x0011A440
	private void SetIndicator(bool val)
	{
		if (val == this.m_active)
		{
			return;
		}
		this.m_active = val;
		this.m_indicator.SetActive(val);
		BnetBar.Get().UpdateLayout();
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x0011C277 File Offset: 0x0011A477
	public bool IsVisible()
	{
		return this.m_active;
	}

	// Token: 0x06003AC4 RID: 15044 RVA: 0x0011C27F File Offset: 0x0011A47F
	private void Update()
	{
		this.SetIndicator(ConnectAPI.TimeSinceLastPong() > 3f);
	}

	// Token: 0x04002577 RID: 9591
	private const float LATENCY_TOLERANCE = 3f;

	// Token: 0x04002578 RID: 9592
	public GameObject m_indicator;

	// Token: 0x04002579 RID: 9593
	private static ConnectionIndicator s_instance;

	// Token: 0x0400257A RID: 9594
	private bool m_active;
}
