using System;
using UnityEngine;

// Token: 0x02000800 RID: 2048
public class RewardCardBack : MonoBehaviour
{
	// Token: 0x06004F6C RID: 20332 RVA: 0x001798C4 File Offset: 0x00177AC4
	private void Awake()
	{
	}

	// Token: 0x06004F6D RID: 20333 RVA: 0x001798C6 File Offset: 0x00177AC6
	private void OnDestroy()
	{
		this.m_Ready = false;
	}

	// Token: 0x06004F6E RID: 20334 RVA: 0x001798CF File Offset: 0x00177ACF
	public bool IsReady()
	{
		return this.m_Ready;
	}

	// Token: 0x06004F6F RID: 20335 RVA: 0x001798D7 File Offset: 0x00177AD7
	public void LoadCardBack(CardBackRewardData cardbackData, GameLayer layer = GameLayer.IgnoreFullScreenEffects)
	{
		this.m_layer = layer;
		this.m_CardBackID = cardbackData.CardBackID;
		CardBackManager.Get().LoadCardBackByIndex(this.m_CardBackID, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnCardBackLoaded), "Card_Hidden");
	}

	// Token: 0x06004F70 RID: 20336 RVA: 0x0017990E File Offset: 0x00177B0E
	public void Death()
	{
		this.m_actor.ActivateSpell(SpellType.DEATH);
	}

	// Token: 0x06004F71 RID: 20337 RVA: 0x00179920 File Offset: 0x00177B20
	private void OnCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
	{
		GameObject gameObject = cardbackData.m_GameObject;
		gameObject.transform.parent = this.m_cardbackBone.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.one;
		SceneUtils.SetLayer(gameObject, this.m_layer);
		this.m_actor = gameObject.GetComponent<Actor>();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_cardbackTitle.Text = "GLOBAL_SEASON_END_NEW_CARDBACK_TITLE_PHONE";
		}
		this.m_cardbackName.Text = cardbackData.m_Name;
		this.m_Ready = true;
	}

	// Token: 0x04003649 RID: 13897
	public GameObject m_cardbackBone;

	// Token: 0x0400364A RID: 13898
	public UberText m_cardbackTitle;

	// Token: 0x0400364B RID: 13899
	public UberText m_cardbackName;

	// Token: 0x0400364C RID: 13900
	public int m_CardBackID = -1;

	// Token: 0x0400364D RID: 13901
	private bool m_Ready;

	// Token: 0x0400364E RID: 13902
	private Actor m_actor;

	// Token: 0x0400364F RID: 13903
	private GameLayer m_layer = GameLayer.IgnoreFullScreenEffects;
}
