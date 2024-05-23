using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class CardBackSummon : MonoBehaviour
{
	// Token: 0x06004775 RID: 18293 RVA: 0x0015702F File Offset: 0x0015522F
	private void Start()
	{
		this.UpdateEffect();
	}

	// Token: 0x06004776 RID: 18294 RVA: 0x00157037 File Offset: 0x00155237
	public void UpdateEffect()
	{
		this.UpdateEchoTexture();
	}

	// Token: 0x06004777 RID: 18295 RVA: 0x00157040 File Offset: 0x00155240
	private void UpdateEchoTexture()
	{
		if (this.m_CardBackManager == null)
		{
			this.m_CardBackManager = CardBackManager.Get();
			if (this.m_CardBackManager == null)
			{
				Debug.LogError("CardBackSummonIn failed to get CardBackManager!");
				base.enabled = false;
			}
		}
		if (this.m_Actor == null)
		{
			this.m_Actor = SceneUtils.FindComponentInParents<Actor>(base.gameObject);
			if (this.m_Actor == null)
			{
				Debug.LogError("CardBackSummonIn failed to get Actor!");
			}
		}
		Texture texture = base.GetComponent<Renderer>().material.mainTexture;
		if (this.m_CardBackManager.IsActorFriendly(this.m_Actor))
		{
			CardBack friendlyCardBack = this.m_CardBackManager.GetFriendlyCardBack();
			if (friendlyCardBack != null)
			{
				texture = friendlyCardBack.m_HiddenCardEchoTexture;
			}
		}
		else
		{
			CardBack opponentCardBack = this.m_CardBackManager.GetOpponentCardBack();
			if (opponentCardBack != null)
			{
				texture = opponentCardBack.m_HiddenCardEchoTexture;
			}
		}
		if (texture != null)
		{
			base.GetComponent<Renderer>().material.mainTexture = texture;
		}
	}

	// Token: 0x04002EAB RID: 11947
	private CardBackManager m_CardBackManager;

	// Token: 0x04002EAC RID: 11948
	private Actor m_Actor;
}
