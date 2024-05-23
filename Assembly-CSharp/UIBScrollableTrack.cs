using System;
using UnityEngine;

// Token: 0x02000F67 RID: 3943
[RequireComponent(typeof(BoxCollider), typeof(PegUIElement))]
[CustomEditClass]
public class UIBScrollableTrack : MonoBehaviour
{
	// Token: 0x06007509 RID: 29961 RVA: 0x00228D2B File Offset: 0x00226F2B
	private void Awake()
	{
		if (this.m_parentScrollbar == null)
		{
			Debug.LogError("Parent scroll bar not set!");
			return;
		}
		this.m_parentScrollbar.AddEnableScrollListener(new UIBScrollable.EnableScroll(this.OnScrollEnabled));
	}

	// Token: 0x0600750A RID: 29962 RVA: 0x00228D60 File Offset: 0x00226F60
	private void OnEnable()
	{
		if (this.m_scrollTrack != null)
		{
			this.m_lastEnabled = this.m_parentScrollbar.IsEnabled();
			this.m_scrollTrack.transform.localEulerAngles = ((!this.m_lastEnabled) ? this.m_hideRotation : this.m_showRotation);
		}
	}

	// Token: 0x0600750B RID: 29963 RVA: 0x00228DBC File Offset: 0x00226FBC
	private void OnScrollEnabled(bool enabled)
	{
		if (this.m_scrollTrack == null || !this.m_scrollTrack.activeInHierarchy || this.m_lastEnabled == enabled)
		{
			return;
		}
		this.m_lastEnabled = enabled;
		Vector3 localEulerAngles;
		Vector3 vector;
		if (enabled)
		{
			localEulerAngles = this.m_hideRotation;
			vector = this.m_showRotation;
		}
		else
		{
			localEulerAngles = this.m_showRotation;
			vector = this.m_hideRotation;
		}
		this.m_scrollTrack.transform.localEulerAngles = localEulerAngles;
		iTween.StopByName(this.m_scrollTrack, "rotate");
		iTween.RotateTo(this.m_scrollTrack, iTween.Hash(new object[]
		{
			"rotation",
			vector,
			"islocal",
			true,
			"time",
			this.m_rotateAnimationTime,
			"name",
			"rotate"
		}));
	}

	// Token: 0x04005F97 RID: 24471
	public UIBScrollable m_parentScrollbar;

	// Token: 0x04005F98 RID: 24472
	public GameObject m_scrollTrack;

	// Token: 0x04005F99 RID: 24473
	public Vector3 m_showRotation = Vector3.zero;

	// Token: 0x04005F9A RID: 24474
	public Vector3 m_hideRotation = new Vector3(0f, 0f, 180f);

	// Token: 0x04005F9B RID: 24475
	public float m_rotateAnimationTime = 0.5f;

	// Token: 0x04005F9C RID: 24476
	private bool m_lastEnabled;
}
