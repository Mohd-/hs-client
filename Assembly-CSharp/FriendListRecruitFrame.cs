using System;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public class FriendListRecruitFrame : MonoBehaviour
{
	// Token: 0x06003F76 RID: 16246 RVA: 0x00134530 File Offset: 0x00132730
	protected virtual void Awake()
	{
		this.m_RecruitUI = Object.Instantiate<FriendListRecruitUI>(this.m_Prefabs.recruitUI);
		this.m_RecruitUI.transform.parent = base.gameObject.transform;
		this.m_RecruitUI.gameObject.SetActive(false);
		this.m_rightComponentOrder = new Component[]
		{
			this.m_RecruitUI
		};
	}

	// Token: 0x06003F77 RID: 16247 RVA: 0x00134594 File Offset: 0x00132794
	private void OnEnable()
	{
		this.UpdateRecruit();
	}

	// Token: 0x06003F78 RID: 16248 RVA: 0x0013459C File Offset: 0x0013279C
	public void SetRecruitInfo(Network.RecruitInfo info)
	{
		this.m_recruitInfo = info;
		this.UpdateRecruit();
	}

	// Token: 0x06003F79 RID: 16249 RVA: 0x001345AB File Offset: 0x001327AB
	public Network.RecruitInfo GetRecruitInfo()
	{
		return this.m_recruitInfo;
	}

	// Token: 0x06003F7A RID: 16250 RVA: 0x001345B4 File Offset: 0x001327B4
	private void UpdateLayout()
	{
		Component component = this.m_Bones.m_RightComponent;
		for (int i = this.m_rightComponentOrder.Length - 1; i >= 0; i--)
		{
			Component component2 = this.m_rightComponentOrder[i];
			if (component2.gameObject.activeSelf)
			{
				TransformUtil.SetPoint(component2, Anchor.RIGHT, component, Anchor.LEFT, this.m_Offsets.m_RightComponent);
				component = component2;
			}
		}
		this.LayoutLeftText(this.m_PlayerNameText, this.m_Bones.m_PlayerNameText, this.m_Offsets.m_PlayerNameText, component);
		this.LayoutLeftText(this.m_StatusText, this.m_Bones.m_StatusText, this.m_Offsets.m_StatusText, component);
	}

	// Token: 0x06003F7B RID: 16251 RVA: 0x00134664 File Offset: 0x00132864
	private void LayoutLeftText(UberText text, Transform bone, Vector3 offset, Component rightComponent)
	{
		if (!text.gameObject.activeInHierarchy)
		{
			return;
		}
		text.Width = this.ComputeLeftComponentWidth(bone, offset, rightComponent);
		TransformUtil.SetPoint(text, Anchor.LEFT, bone, Anchor.RIGHT, offset);
	}

	// Token: 0x06003F7C RID: 16252 RVA: 0x0013469C File Offset: 0x0013289C
	private float ComputeLeftComponentWidth(Transform bone, Vector3 offset, Component rightComponent)
	{
		Vector3 vector = bone.position + offset;
		Bounds bounds = TransformUtil.ComputeSetPointBounds(rightComponent);
		float num = bounds.center.x - bounds.extents.x + this.m_Offsets.m_RightComponent.x;
		return num - vector.x;
	}

	// Token: 0x06003F7D RID: 16253 RVA: 0x001346F8 File Offset: 0x001328F8
	public virtual void UpdateRecruit()
	{
		if (base.gameObject == null || !base.gameObject.activeSelf)
		{
			return;
		}
		this.m_RecruitUI.SetInfo(this.m_recruitInfo);
		if (this.m_recruitInfo != null)
		{
			this.m_PlayerNameText.Text = this.m_recruitInfo.Nickname;
			switch (this.m_recruitInfo.Status)
			{
			case 1:
			{
				string requestElapsedTimeString = FriendUtils.GetRequestElapsedTimeString(this.m_recruitInfo.CreationTimeMicrosec);
				this.m_StatusText.Text = string.Format("Invintation sent {0}", requestElapsedTimeString);
				break;
			}
			case 2:
				this.m_StatusText.Text = "Account ineligible!";
				break;
			case 3:
				this.m_StatusText.Text = "Invitation declined!";
				break;
			case 4:
				this.m_StatusText.Text = "Accepted";
				break;
			}
		}
		this.UpdateLayout();
	}

	// Token: 0x040028AD RID: 10413
	public UberText m_PlayerNameText;

	// Token: 0x040028AE RID: 10414
	public UberText m_StatusText;

	// Token: 0x040028AF RID: 10415
	private FriendListRecruitUI m_RecruitUI;

	// Token: 0x040028B0 RID: 10416
	private Network.RecruitInfo m_recruitInfo;

	// Token: 0x040028B1 RID: 10417
	public FriendListRecruitFrameBones m_Bones;

	// Token: 0x040028B2 RID: 10418
	public FriendListRecruitFrameOffsets m_Offsets;

	// Token: 0x040028B3 RID: 10419
	public FriendListRecruitFramePrefabs m_Prefabs;

	// Token: 0x040028B4 RID: 10420
	private Component[] m_rightComponentOrder;
}
