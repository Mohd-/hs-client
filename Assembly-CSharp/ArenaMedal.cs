using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
public class ArenaMedal : PegUIElement
{
	// Token: 0x06003DBC RID: 15804 RVA: 0x0012AB74 File Offset: 0x00128D74
	protected override void Awake()
	{
		base.Awake();
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MedalOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MedalOut));
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x0012ABAF File Offset: 0x00128DAF
	public void SetMedal(int medal)
	{
		this.m_medal = medal;
		AssetLoader.Get().LoadTexture("Medal_Key_" + (medal + 1), new AssetLoader.ObjectCallback(this.OnTextureLoaded), null, false);
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x0012ABE4 File Offset: 0x00128DE4
	private void MedalOver(UIEvent e)
	{
		bool flag = SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB);
		string headline;
		string bodytext;
		if (Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE) || flag)
		{
			headline = this.GetMedalName();
			bodytext = GameStrings.Format("GLOBAL_MEDAL_ARENA_TOOLTIP_BODY", new object[]
			{
				this.m_medal
			});
		}
		else
		{
			headline = GameStrings.Get("GLUE_TOURNAMENT_UNRANKED_MODE");
			bodytext = GameStrings.Get("GLUE_TOURNAMENT_UNRANKED_DESC");
		}
		base.gameObject.GetComponent<TooltipZone>().ShowLayerTooltip(headline, bodytext);
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x0012AC68 File Offset: 0x00128E68
	private void MedalOut(UIEvent e)
	{
		base.gameObject.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x0012AC7C File Offset: 0x00128E7C
	private void OnTextureLoaded(string assetName, Object asset, object callbackData)
	{
		if (asset == null)
		{
			Debug.LogWarning(string.Format("ArenaMedal.OnTextureLoaded(): asset for {0} is null!", assetName));
			return;
		}
		Texture texture = asset as Texture;
		if (texture == null)
		{
			Debug.LogWarning(string.Format("ArenaMedal.OnTextureLoaded(): medalTexture for {0} is null (asset is not a texture)!", assetName));
			return;
		}
		this.m_rankMedal.GetComponent<Renderer>().material.mainTexture = texture;
	}

	// Token: 0x06003DC1 RID: 15809 RVA: 0x0012ACE0 File Offset: 0x00128EE0
	private string GetMedalName()
	{
		return GameStrings.Get("GLOBAL_ARENA_MEDAL_" + this.m_medal);
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x0012ACFC File Offset: 0x00128EFC
	private string GetNextMedalName()
	{
		string text = "GLOBAL_ARENA_MEDAL_" + (this.m_medal + 1);
		string text2 = GameStrings.Get(text);
		if (text2 == text)
		{
			return string.Empty;
		}
		return text2;
	}

	// Token: 0x04002751 RID: 10065
	public const string MEDAL_TEXTURE_PREFIX = "Medal_Key_";

	// Token: 0x04002752 RID: 10066
	public const string MEDAL_NAME_PREFIX = "GLOBAL_ARENA_MEDAL_";

	// Token: 0x04002753 RID: 10067
	public GameObject m_rankMedal;

	// Token: 0x04002754 RID: 10068
	private int m_medal;
}
