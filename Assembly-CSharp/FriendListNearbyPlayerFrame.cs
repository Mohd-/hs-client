using System;
using UnityEngine;

// Token: 0x02000568 RID: 1384
public class FriendListNearbyPlayerFrame : MonoBehaviour
{
	// Token: 0x06003F6C RID: 16236 RVA: 0x001342DC File Offset: 0x001324DC
	protected virtual void Awake()
	{
		this.m_ChallengeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChallengeButtonReleased));
		this.m_rightComponentOrder = new Component[]
		{
			this.m_ChallengeButton
		};
	}

	// Token: 0x06003F6D RID: 16237 RVA: 0x0013430C File Offset: 0x0013250C
	private void OnEnable()
	{
		this.UpdateNearbyPlayer();
	}

	// Token: 0x06003F6E RID: 16238 RVA: 0x00134314 File Offset: 0x00132514
	private void OnChallengeButtonReleased(UIEvent e)
	{
		if (!this.m_ChallengeButton.CanChallenge())
		{
			FriendListFriendFrame.OnPlayerChallengeButtonPressed(this.m_ChallengeButton, this.m_player);
		}
	}

	// Token: 0x06003F6F RID: 16239 RVA: 0x00134338 File Offset: 0x00132538
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
	}

	// Token: 0x06003F70 RID: 16240 RVA: 0x001343C4 File Offset: 0x001325C4
	private void LayoutLeftText(UberText text, Transform bone, Vector3 offset, Component rightComponent)
	{
		if (!text.gameObject.activeInHierarchy)
		{
			return;
		}
		text.Width = this.ComputeLeftComponentWidth(bone, offset, rightComponent);
		TransformUtil.SetPoint(text, Anchor.LEFT, bone, Anchor.RIGHT, offset);
	}

	// Token: 0x06003F71 RID: 16241 RVA: 0x001343FC File Offset: 0x001325FC
	private float ComputeLeftComponentWidth(Transform bone, Vector3 offset, Component rightComponent)
	{
		Vector3 vector = bone.position + offset;
		Bounds bounds = TransformUtil.ComputeSetPointBounds(rightComponent);
		float num = bounds.center.x - bounds.extents.x + this.m_Offsets.m_RightComponent.x;
		return num - vector.x;
	}

	// Token: 0x06003F72 RID: 16242 RVA: 0x00134458 File Offset: 0x00132658
	public BnetPlayer GetNearbyPlayer()
	{
		return this.m_player;
	}

	// Token: 0x06003F73 RID: 16243 RVA: 0x00134460 File Offset: 0x00132660
	public virtual bool SetNearbyPlayer(BnetPlayer player)
	{
		if (this.m_player == player)
		{
			return false;
		}
		this.m_player = player;
		this.m_ChallengeButton.SetPlayer(player);
		this.UpdateNearbyPlayer();
		return true;
	}

	// Token: 0x06003F74 RID: 16244 RVA: 0x0013448C File Offset: 0x0013268C
	public virtual void UpdateNearbyPlayer()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.m_player == null)
		{
			this.m_PlayerNameText.Text = string.Empty;
		}
		else
		{
			BnetPlayer bnetPlayer = BnetFriendMgr.Get().FindFriend(this.m_player.GetAccountId());
			if (bnetPlayer != null)
			{
				this.m_PlayerNameText.Text = FriendUtils.GetFriendListName(bnetPlayer, true);
			}
			else
			{
				this.m_PlayerNameText.Text = FriendUtils.GetFriendListName(this.m_player, true);
			}
		}
		this.m_ChallengeButton.UpdateButton();
		this.UpdateLayout();
	}

	// Token: 0x040028A7 RID: 10407
	public UberText m_PlayerNameText;

	// Token: 0x040028A8 RID: 10408
	public FriendListChallengeButton m_ChallengeButton;

	// Token: 0x040028A9 RID: 10409
	public FriendListNearbyPlayerFrameBones m_Bones;

	// Token: 0x040028AA RID: 10410
	public FriendListNearbyPlayerFrameOffsets m_Offsets;

	// Token: 0x040028AB RID: 10411
	private Component[] m_rightComponentOrder;

	// Token: 0x040028AC RID: 10412
	protected BnetPlayer m_player;
}
