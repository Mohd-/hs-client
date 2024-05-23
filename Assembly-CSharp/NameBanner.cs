using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000889 RID: 2185
public class NameBanner : MonoBehaviour
{
	// Token: 0x06005357 RID: 21335 RVA: 0x0018DD64 File Offset: 0x0018BF64
	private void Awake()
	{
		this.m_className.gameObject.SetActive(false);
		this.m_playerName.Text = string.Empty;
		this.m_nameBoneToUse = this.m_nameBone;
	}

	// Token: 0x06005358 RID: 21336 RVA: 0x0018DD9E File Offset: 0x0018BF9E
	private void Update()
	{
		this.UpdateAnchor();
	}

	// Token: 0x06005359 RID: 21337 RVA: 0x0018DDA8 File Offset: 0x0018BFA8
	public void SetName(string name)
	{
		this.m_playerName.Text = name;
		if (this.m_alphaBannerSkinned != null)
		{
			this.AdjustSkinnedBanner();
		}
		else
		{
			this.AdjustBanner();
		}
	}

	// Token: 0x0600535A RID: 21338 RVA: 0x0018DDE4 File Offset: 0x0018BFE4
	private void AdjustBanner()
	{
		Vector3 vector = TransformUtil.ComputeWorldScale(this.m_playerName.gameObject);
		float num = this.FUDGE_FACTOR * vector.x * this.m_playerName.GetTextWorldSpaceBounds().size.x;
		float num2 = this.m_playerName.GetTextWorldSpaceBounds().size.x * vector.x + num;
		float x = this.m_alphaBannerMiddle.GetComponent<Renderer>().bounds.size.x;
		float x2 = this.m_playerName.GetTextBounds().size.x;
		MeshRenderer meshRenderer = this.m_medalAlphaBannerMiddle.GetComponentsInChildren<MeshRenderer>(true)[0];
		float x3 = meshRenderer.bounds.size.x;
		if (num2 > x)
		{
			if (GameUtils.ShouldShowRankedMedals())
			{
				TransformUtil.SetLocalScaleX(this.m_medalAlphaBannerMiddle, x2 / x3);
				TransformUtil.SetPoint(this.m_medalAlphaBannerRight, Anchor.LEFT, meshRenderer.gameObject, Anchor.RIGHT, new Vector3(0f, 0f, 0f));
			}
			else
			{
				TransformUtil.SetLocalScaleX(this.m_alphaBannerMiddle, num2 / x);
				TransformUtil.SetPoint(this.m_alphaBannerRight, Anchor.LEFT, this.m_alphaBannerMiddle, Anchor.RIGHT, new Vector3(-num, 0f, 0f));
			}
		}
	}

	// Token: 0x0600535B RID: 21339 RVA: 0x0018DF44 File Offset: 0x0018C144
	private void AdjustSkinnedBanner()
	{
		if (GameUtils.ShouldShowRankedMedals())
		{
			float num = -this.m_playerName.GetTextBounds().size.x - 10f;
			if (num > -17f)
			{
				num = -17f;
			}
			Vector3 localPosition = this.m_medalBannerBone.transform.localPosition;
			this.m_medalBannerBone.transform.localPosition = new Vector3(num, localPosition.y, localPosition.z);
		}
		else
		{
			float num = -this.m_playerName.GetTextBounds().size.x - 1f;
			if (num > -12f)
			{
				num = -12f;
			}
			Vector3 localPosition2 = this.m_alphaBannerBone.transform.localPosition;
			this.m_alphaBannerBone.transform.localPosition = new Vector3(num, localPosition2.y, localPosition2.z);
		}
	}

	// Token: 0x0600535C RID: 21340 RVA: 0x0018E038 File Offset: 0x0018C238
	public void SetClass(string className, bool isAI)
	{
		this.m_className.gameObject.SetActive(true);
		this.m_className.Text = className;
		if (isAI)
		{
			this.m_playerName.transform.localPosition = this.m_classBoneToUse.localPosition;
		}
		else
		{
			this.m_playerName.transform.localPosition = this.m_classBoneToUse.localPosition;
		}
	}

	// Token: 0x0600535D RID: 21341 RVA: 0x0018E0A4 File Offset: 0x0018C2A4
	public void FadeClass()
	{
		if (this.m_playerSide == Player.Side.FRIENDLY && !string.IsNullOrEmpty(GameState.Get().GetGameEntity().GetAlternatePlayerName()))
		{
			iTween.FadeTo(base.gameObject, 0f, 1f);
			return;
		}
		iTween.FadeTo(this.m_className.gameObject, 0f, 1f);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_nameBoneToUse.localPosition,
			"isLocal",
			true,
			"time",
			1f
		});
		iTween.MoveTo(this.m_playerName.gameObject, args);
	}

	// Token: 0x0600535E RID: 21342 RVA: 0x0018E164 File Offset: 0x0018C364
	public void FadeIn()
	{
		if (this.m_alphaBannerSkinned != null)
		{
			iTween.FadeTo(this.m_alphaBannerSkinned.gameObject, 1f, 1f);
		}
		else
		{
			iTween.FadeTo(this.m_alphaBanner.gameObject, 1f, 1f);
		}
		iTween.FadeTo(this.m_playerName.gameObject, 1f, 1f);
	}

	// Token: 0x0600535F RID: 21343 RVA: 0x0018E1D8 File Offset: 0x0018C3D8
	public void SetPlayerSide(Player.Side side)
	{
		this.m_playerSide = side;
		this.UpdateAnchor();
		base.StartCoroutine(this.UpdateName());
		base.StartCoroutine(this.UpdateUnknownName());
	}

	// Token: 0x06005360 RID: 21344 RVA: 0x0018E20C File Offset: 0x0018C40C
	public Player.Side GetPlayerSide()
	{
		return this.m_playerSide;
	}

	// Token: 0x06005361 RID: 21345 RVA: 0x0018E214 File Offset: 0x0018C414
	public void Unload()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06005362 RID: 21346 RVA: 0x0018E224 File Offset: 0x0018C424
	private void UpdateAnchor()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			if (this.m_playerSide == Player.Side.FRIENDLY)
			{
				OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.BOTTOM_RIGHT, false, CanvasScaleMode.HEIGHT);
			}
			else
			{
				OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.BOTTOM_LEFT, false, CanvasScaleMode.HEIGHT);
			}
		}
		else
		{
			Vector3 localPosition;
			if (this.m_playerSide == Player.Side.FRIENDLY)
			{
				OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.BOTTOM_LEFT, false, CanvasScaleMode.HEIGHT);
				localPosition..ctor(0f, 5f, 22f);
			}
			else
			{
				OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.TOP_LEFT, false, CanvasScaleMode.HEIGHT);
				localPosition..ctor(0f, 5f, -10f);
			}
			base.transform.localPosition = localPosition;
		}
	}

	// Token: 0x06005363 RID: 21347 RVA: 0x0018E2EB File Offset: 0x0018C4EB
	private bool IsReady(Player p)
	{
		return p != null && p.GetName() != null && p.GetRank() != null;
	}

	// Token: 0x06005364 RID: 21348 RVA: 0x0018E310 File Offset: 0x0018C510
	private IEnumerator UpdateName()
	{
		while (GameState.Get().GetPlayerMap().Count == 0)
		{
			yield return null;
		}
		Player p = null;
		while (p == null)
		{
			Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
			foreach (Player player in playerMap.Values)
			{
				if (player.GetSide() == this.m_playerSide)
				{
					p = player;
					break;
				}
			}
			yield return null;
		}
		while (p.GetName() == null)
		{
			yield return null;
		}
		string nameToDisplay = p.GetName();
		if (p.IsLocalUser())
		{
			string altName = GameState.Get().GetGameEntity().GetAlternatePlayerName();
			if (!string.IsNullOrEmpty(altName))
			{
				nameToDisplay = altName;
			}
		}
		this.SetName(nameToDisplay);
		this.m_nameBoneToUse = this.m_nameBone;
		this.m_classBoneToUse = this.m_classBone;
		if (this.m_alphaBannerSkinned == null)
		{
			this.m_alphaBanner.SetActive(true);
		}
		else
		{
			this.m_alphaBannerSkinned.SetActive(true);
			if (this.m_alphaBanner != null)
			{
				this.m_alphaBanner.SetActive(false);
			}
			this.m_medalBannerSkinned.SetActive(false);
		}
		if (this.m_medalAlphaBanner != null)
		{
			this.m_medalAlphaBanner.SetActive(false);
		}
		if (this.m_medalBannerSkinned != null)
		{
			this.m_medalBannerSkinned.SetActive(false);
		}
		this.m_medal.gameObject.SetActive(false);
		if (GameUtils.ShouldShowRankedMedals())
		{
			while (p.GetRank() == null)
			{
				yield return null;
			}
			MedalInfoTranslator medalInfoTranslator = p.GetRank();
			bool isWild = Options.Get().GetBool(Option.IN_WILD_MODE);
			if (medalInfoTranslator != null && medalInfoTranslator.IsDisplayable(isWild))
			{
				this.m_nameBoneToUse = this.m_medalNameBone;
				this.m_classBoneToUse = this.m_medalClassBone;
				if (this.m_medalBannerSkinned == null)
				{
					this.m_medalAlphaBanner.SetActive(true);
				}
				else
				{
					this.m_medalBannerSkinned.SetActive(true);
					this.m_alphaBannerSkinned.SetActive(false);
					if (this.m_medalAlphaBanner != null)
					{
						this.m_medalAlphaBanner.SetActive(false);
					}
				}
				if (this.m_alphaBanner != null)
				{
					this.m_alphaBanner.SetActive(false);
				}
				this.m_medal.gameObject.SetActive(true);
				this.m_medal.SetMedal(medalInfoTranslator, false);
				this.m_medal.SetFormat(isWild);
			}
		}
		this.m_playerName.transform.localPosition = this.m_nameBoneToUse.localPosition;
		if (GameMgr.Get().IsTutorial())
		{
			yield break;
		}
		while (p.GetHero().GetClass() == TAG_CLASS.INVALID)
		{
			yield return null;
		}
		this.SetClass(GameStrings.GetClassName(p.GetHero().GetClass()).ToUpper(), p.IsAI());
		yield break;
	}

	// Token: 0x06005365 RID: 21349 RVA: 0x0018E32C File Offset: 0x0018C52C
	public void UpdateMedalChange(MedalInfoTranslator medalInfo)
	{
		if (medalInfo == null || !medalInfo.IsDisplayable(Options.Get().GetBool(Option.IN_WILD_MODE)))
		{
			return;
		}
		this.m_medal.SetMedal(medalInfo, false);
	}

	// Token: 0x06005366 RID: 21350 RVA: 0x0018E368 File Offset: 0x0018C568
	private IEnumerator UpdateUnknownName()
	{
		yield return new WaitForSeconds(5f);
		if (this.m_playerName.Text != string.Empty)
		{
			yield break;
		}
		this.SetName(GameStrings.Get("GAMEPLAY_UNKNOWN_OPPONENT_NAME"));
		yield break;
	}

	// Token: 0x04003989 RID: 14729
	private const float SKINNED_BANNER_MIN_SIZE = 12f;

	// Token: 0x0400398A RID: 14730
	private const float SKINNED_MEDAL_BANNER_MIN_SIZE = 17f;

	// Token: 0x0400398B RID: 14731
	private const float UNKNOWN_NAME_WAIT = 5f;

	// Token: 0x0400398C RID: 14732
	public GameObject m_alphaBannerSkinned;

	// Token: 0x0400398D RID: 14733
	public GameObject m_alphaBannerBone;

	// Token: 0x0400398E RID: 14734
	public GameObject m_medalBannerSkinned;

	// Token: 0x0400398F RID: 14735
	public GameObject m_medalBannerBone;

	// Token: 0x04003990 RID: 14736
	public GameObject m_alphaBanner;

	// Token: 0x04003991 RID: 14737
	public GameObject m_alphaBannerLeft;

	// Token: 0x04003992 RID: 14738
	public GameObject m_alphaBannerMiddle;

	// Token: 0x04003993 RID: 14739
	public GameObject m_alphaBannerRight;

	// Token: 0x04003994 RID: 14740
	public GameObject m_medalAlphaBanner;

	// Token: 0x04003995 RID: 14741
	public GameObject m_medalAlphaBannerLeft;

	// Token: 0x04003996 RID: 14742
	public GameObject m_medalAlphaBannerMiddle;

	// Token: 0x04003997 RID: 14743
	public GameObject m_medalAlphaBannerRight;

	// Token: 0x04003998 RID: 14744
	public UberText m_playerName;

	// Token: 0x04003999 RID: 14745
	public UberText m_className;

	// Token: 0x0400399A RID: 14746
	public Transform m_nameBone;

	// Token: 0x0400399B RID: 14747
	public Transform m_classBone;

	// Token: 0x0400399C RID: 14748
	public Transform m_medalNameBone;

	// Token: 0x0400399D RID: 14749
	public Transform m_medalClassBone;

	// Token: 0x0400399E RID: 14750
	public TournamentMedal m_medal;

	// Token: 0x0400399F RID: 14751
	public float FUDGE_FACTOR = 0.1915f;

	// Token: 0x040039A0 RID: 14752
	private Player.Side m_playerSide;

	// Token: 0x040039A1 RID: 14753
	private Transform m_nameBoneToUse;

	// Token: 0x040039A2 RID: 14754
	private Transform m_classBoneToUse;
}
