using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class CardBackDisplay : MonoBehaviour
{
	// Token: 0x06002B4D RID: 11085 RVA: 0x000D754B File Offset: 0x000D574B
	private void Start()
	{
		this.m_CardBackManager = CardBackManager.Get();
		if (this.m_CardBackManager == null)
		{
			Debug.LogError("Failed to get CardBackManager!");
			base.enabled = false;
		}
		this.UpdateCardBack();
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000D7580 File Offset: 0x000D5780
	public void UpdateCardBack()
	{
		if (this.m_CardBackManager == null)
		{
			return;
		}
		base.StartCoroutine(this.SetCardBackDisplay());
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000D75A1 File Offset: 0x000D57A1
	public void SetCardBack(bool friendlySide)
	{
		if (this.m_CardBackManager == null)
		{
			this.m_CardBackManager = CardBackManager.Get();
		}
		this.m_CardBackManager.UpdateCardBack(base.gameObject, friendlySide);
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000D75D4 File Offset: 0x000D57D4
	private IEnumerator SetCardBackDisplay()
	{
		if (this.m_Actor == null)
		{
			this.m_CardBackManager.UpdateCardBack(base.gameObject, true);
			yield break;
		}
		if (this.m_Actor.GetCardbackUpdateIgnore())
		{
			yield break;
		}
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
		{
			while (this.m_Actor.GetEntity() == null)
			{
				yield return null;
			}
		}
		this.m_FriendlySide = true;
		Entity entity = this.m_Actor.GetEntity();
		if (entity != null)
		{
			Player controller = entity.GetController();
			if (controller != null && controller.GetSide() == Player.Side.OPPOSING)
			{
				this.m_FriendlySide = false;
			}
		}
		this.m_CardBackManager.UpdateCardBack(base.gameObject, this.m_FriendlySide);
		this.m_Actor.SeedMaterialEffects();
		yield break;
	}

	// Token: 0x04001A32 RID: 6706
	public Actor m_Actor;

	// Token: 0x04001A33 RID: 6707
	private CardBackManager m_CardBackManager;

	// Token: 0x04001A34 RID: 6708
	private bool m_FriendlySide = true;
}
