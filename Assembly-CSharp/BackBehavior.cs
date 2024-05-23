using System;
using UnityEngine;

// Token: 0x02000F65 RID: 3941
public class BackBehavior : MonoBehaviour
{
	// Token: 0x060074FF RID: 29951 RVA: 0x00228B2C File Offset: 0x00226D2C
	public void Awake()
	{
		PegUIElement component = base.gameObject.GetComponent<PegUIElement>();
		if (component != null)
		{
			component.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnRelease();
			});
		}
	}

	// Token: 0x06007500 RID: 29952 RVA: 0x00228B65 File Offset: 0x00226D65
	public void OnRelease()
	{
		Navigation.GoBack();
	}
}
