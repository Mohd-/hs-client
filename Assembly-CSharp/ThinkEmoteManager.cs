using System;
using UnityEngine;

// Token: 0x0200085B RID: 2139
public class ThinkEmoteManager : MonoBehaviour
{
	// Token: 0x0600526E RID: 21102 RVA: 0x00189889 File Offset: 0x00187A89
	private void Awake()
	{
		ThinkEmoteManager.s_instance = this;
	}

	// Token: 0x0600526F RID: 21103 RVA: 0x00189891 File Offset: 0x00187A91
	private void OnDestroy()
	{
		ThinkEmoteManager.s_instance = null;
	}

	// Token: 0x06005270 RID: 21104 RVA: 0x00189899 File Offset: 0x00187A99
	public static ThinkEmoteManager Get()
	{
		return ThinkEmoteManager.s_instance;
	}

	// Token: 0x06005271 RID: 21105 RVA: 0x001898A0 File Offset: 0x00187AA0
	private void Update()
	{
		GameState gameState = GameState.Get();
		if (gameState == null)
		{
			return;
		}
		if (!gameState.IsMainPhase())
		{
			return;
		}
		this.m_secondsSinceAction += Time.deltaTime;
		if (this.m_secondsSinceAction > 20f)
		{
			this.PlayThinkEmote();
		}
	}

	// Token: 0x06005272 RID: 21106 RVA: 0x001898EE File Offset: 0x00187AEE
	private void PlayThinkEmote()
	{
		this.m_secondsSinceAction = 0f;
		GameState.Get().GetGameEntity().OnPlayThinkEmote();
	}

	// Token: 0x06005273 RID: 21107 RVA: 0x0018990A File Offset: 0x00187B0A
	public void NotifyOfActivity()
	{
		this.m_secondsSinceAction = 0f;
	}

	// Token: 0x040038A5 RID: 14501
	public const float SECONDS_BEFORE_EMOTE = 20f;

	// Token: 0x040038A6 RID: 14502
	private float m_secondsSinceAction;

	// Token: 0x040038A7 RID: 14503
	private static ThinkEmoteManager s_instance;
}
