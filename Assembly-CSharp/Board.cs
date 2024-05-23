using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000328 RID: 808
[CustomEditClass]
public class Board : MonoBehaviour
{
	// Token: 0x06002A05 RID: 10757 RVA: 0x000CDCD8 File Offset: 0x000CBED8
	private void Awake()
	{
		Board.s_instance = this;
		if (LoadingScreen.Get() != null)
		{
			LoadingScreen.Get().NotifyMainSceneObjectAwoke(base.gameObject);
		}
		if (this.m_FriendlyHeroTray == null)
		{
			Debug.LogError("Friendly Hero Tray is not assigned!");
		}
		if (this.m_OpponentHeroTray == null)
		{
			Debug.LogError("Opponent Hero Tray is not assigned!");
		}
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x000CDD41 File Offset: 0x000CBF41
	private void OnDestroy()
	{
		Board.s_instance = null;
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x000CDD4C File Offset: 0x000CBF4C
	private void Start()
	{
		ProjectedShadow.SetShadowColor(this.m_ShadowColor);
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].normalizedTime = 0.25f;
			base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].speed = -3f;
			base.GetComponent<Animation>().Play(base.GetComponent<Animation>().clip.name);
		}
		base.StartCoroutine(this.GoldenHeroes());
		foreach (Board.BoardSpecialEvents boardSpecialEvents in this.m_SpecialEvents)
		{
			if (SpecialEventManager.Get().IsEventActive(boardSpecialEvents.EventType, false))
			{
				Log.Kyle.Print("Board Special Event: ", new object[]
				{
					boardSpecialEvents.EventType
				});
				this.LoadBoardSpecialEvent(boardSpecialEvents);
			}
		}
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000CDE70 File Offset: 0x000CC070
	public static Board Get()
	{
		return Board.s_instance;
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x000CDE77 File Offset: 0x000CC077
	public void SetBoardDbId(int id)
	{
		this.m_boardDbId = id;
		Log.Kyle.Print("Board DB ID: {0}", new object[]
		{
			id
		});
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x000CDE9E File Offset: 0x000CC09E
	public void ResetAmbientColor()
	{
		RenderSettings.ambientLight = this.m_AmbientColor;
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x000CDEAB File Offset: 0x000CC0AB
	[ContextMenu("RaiseTheLights")]
	public void RaiseTheLights()
	{
		this.RaiseTheLights(1f);
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000CDEB8 File Offset: 0x000CC0B8
	public void RaiseTheLightsQuickly()
	{
		this.RaiseTheLights(5f);
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000CDEC8 File Offset: 0x000CC0C8
	public void RaiseTheLights(float speed)
	{
		if (this.m_raisedLights)
		{
			return;
		}
		float num = 3f / speed;
		Action<object> action = delegate(object amount)
		{
			RenderSettings.ambientLight = (Color)amount;
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			RenderSettings.ambientLight,
			"to",
			this.m_AmbientColor,
			"time",
			num,
			"easeType",
			iTween.EaseType.easeInOutQuad,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args);
		Action<object> action2 = delegate(object amount)
		{
			this.m_DirectionalLight.intensity = (float)amount;
		};
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			this.m_DirectionalLight.intensity,
			"to",
			this.m_DirectionalLightIntensity,
			"time",
			num,
			"easeType",
			iTween.EaseType.easeInOutQuad,
			"onupdate",
			action2,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args2);
		this.m_raisedLights = true;
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x000CE02F File Offset: 0x000CC22F
	public void SetMulliganLighting()
	{
		RenderSettings.ambientLight = this.MULLIGAN_AMBIENT_LIGHT_COLOR;
		this.m_DirectionalLight.intensity = 0f;
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000CE04C File Offset: 0x000CC24C
	public void DimTheLights()
	{
		this.DimTheLights(5f);
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x000CE05C File Offset: 0x000CC25C
	public void DimTheLights(float speed)
	{
		if (!this.m_raisedLights)
		{
			return;
		}
		float num = 3f / speed;
		Action<object> action = delegate(object amount)
		{
			RenderSettings.ambientLight = (Color)amount;
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			RenderSettings.ambientLight,
			"to",
			this.MULLIGAN_AMBIENT_LIGHT_COLOR,
			"time",
			num,
			"easeType",
			iTween.EaseType.easeInOutQuad,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args);
		Action<object> action2 = delegate(object amount)
		{
			this.m_DirectionalLight.intensity = (float)amount;
		};
		Hashtable args2 = iTween.Hash(new object[]
		{
			"from",
			this.m_DirectionalLight.intensity,
			"to",
			0f,
			"time",
			num,
			"easeType",
			iTween.EaseType.easeInOutQuad,
			"onupdate",
			action2,
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args2);
		this.m_raisedLights = false;
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x000CE1C4 File Offset: 0x000CC3C4
	public Transform FindBone(string name)
	{
		if (this.m_BoneParent != null)
		{
			Transform transform = this.m_BoneParent.Find(name);
			if (transform != null)
			{
				return transform;
			}
		}
		return BoardStandardGame.Get().FindBone(name);
	}

	// Token: 0x06002A12 RID: 10770 RVA: 0x000CE208 File Offset: 0x000CC408
	public Collider FindCollider(string name)
	{
		if (this.m_ColliderParent != null)
		{
			Transform transform = this.m_ColliderParent.Find(name);
			if (transform != null)
			{
				return (!(transform == null)) ? transform.GetComponent<Collider>() : null;
			}
		}
		return BoardStandardGame.Get().FindCollider(name);
	}

	// Token: 0x06002A13 RID: 10771 RVA: 0x000CE263 File Offset: 0x000CC463
	public GameObject GetMouseClickDustEffectPrefab()
	{
		return this.m_MouseClickDustEffect;
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x000CE26C File Offset: 0x000CC46C
	public void CombinedSurface()
	{
		if (this.m_CombinedPlaySurface != null && this.m_SplitPlaySurface != null)
		{
			this.m_CombinedPlaySurface.SetActive(true);
			this.m_SplitPlaySurface.SetActive(false);
		}
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x000CE2B4 File Offset: 0x000CC4B4
	public void SplitSurface()
	{
		if (this.m_CombinedPlaySurface != null && this.m_SplitPlaySurface != null)
		{
			this.m_CombinedPlaySurface.SetActive(false);
			this.m_SplitPlaySurface.SetActive(true);
		}
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x000CE2FB File Offset: 0x000CC4FB
	public Spell GetFriendlyTraySpell()
	{
		return this.m_FriendlyTraySpellEffect;
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000CE303 File Offset: 0x000CC503
	public Spell GetOpponentTraySpell()
	{
		return this.m_OpponentTraySpellEffect;
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000CE30C File Offset: 0x000CC50C
	private IEnumerator GoldenHeroes()
	{
		bool friendlyHeroIsGolden = false;
		bool opposingHeroIsGolden = false;
		GameState gameState = GameState.Get();
		while (gameState == null)
		{
			gameState = GameState.Get();
			yield return null;
		}
		Player friendlyPlayer = gameState.GetFriendlySidePlayer();
		while (friendlyPlayer == null)
		{
			friendlyPlayer = gameState.GetFriendlySidePlayer();
			yield return null;
		}
		Player opposingPlayer = gameState.GetOpposingSidePlayer();
		Card friendlyHeroCard = friendlyPlayer.GetHeroCard();
		while (friendlyHeroCard == null)
		{
			friendlyHeroCard = friendlyPlayer.GetHeroCard();
			yield return null;
		}
		Card opposingHeroCard = opposingPlayer.GetHeroCard();
		while (opposingHeroCard == null)
		{
			opposingHeroCard = opposingPlayer.GetHeroCard();
			yield return null;
		}
		while (friendlyHeroCard.GetEntity() == null)
		{
			yield return null;
		}
		Entity friendlyEntityDef = friendlyHeroCard.GetEntity();
		while (opposingHeroCard.GetEntity() == null)
		{
			yield return null;
		}
		Entity opposingEntityDef = opposingHeroCard.GetEntity();
		if (friendlyHeroCard.GetPremium() == TAG_PREMIUM.GOLDEN && friendlyEntityDef.GetCardSet() != TAG_CARD_SET.HERO_SKINS)
		{
			friendlyHeroIsGolden = true;
		}
		if (opposingHeroCard.GetPremium() == TAG_PREMIUM.GOLDEN && opposingEntityDef.GetCardSet() != TAG_CARD_SET.HERO_SKINS)
		{
			opposingHeroIsGolden = true;
		}
		if (friendlyHeroIsGolden)
		{
			AssetLoader.Get().LoadGameObject("HeroTray_Golden_Friendly", new AssetLoader.GameObjectCallback(this.ShowFriendlyHeroTray), null, false);
		}
		else
		{
			base.StartCoroutine(this.UpdateHeroTray(Player.Side.FRIENDLY, false));
		}
		if (opposingHeroIsGolden)
		{
			AssetLoader.Get().LoadGameObject("HeroTray_Golden_Opponent", new AssetLoader.GameObjectCallback(this.ShowOpponentHeroTray), null, false);
		}
		else
		{
			base.StartCoroutine(this.UpdateHeroTray(Player.Side.OPPOSING, false));
		}
		yield break;
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x000CE328 File Offset: 0x000CC528
	private void ShowFriendlyHeroTray(string name, GameObject go, object callbackData)
	{
		go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).transform.position;
		go.SetActive(true);
		foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
		{
			renderer.material.color = this.m_GoldenHeroTrayColor;
		}
		Object.Destroy(this.m_FriendlyHeroTray);
		this.m_FriendlyHeroTray = go;
		base.StartCoroutine(this.UpdateHeroTray(Player.Side.FRIENDLY, true));
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x000CE3B0 File Offset: 0x000CC5B0
	private void ShowOpponentHeroTray(string name, GameObject go, object callbackData)
	{
		go.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position;
		go.SetActive(true);
		foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
		{
			renderer.material.color = this.m_GoldenHeroTrayColor;
		}
		this.m_OpponentHeroTray.SetActive(false);
		Object.Destroy(this.m_OpponentHeroTray);
		this.m_OpponentHeroTray = go;
		base.StartCoroutine(this.UpdateHeroTray(Player.Side.OPPOSING, true));
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000CE444 File Offset: 0x000CC644
	private IEnumerator UpdateHeroTray(Player.Side side, bool isGolden)
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
				if (player.GetSide() == side)
				{
					p = player;
					break;
				}
			}
			yield return null;
		}
		while (p.GetHero() == null)
		{
			yield return null;
		}
		Entity hero = p.GetHero();
		while (hero.IsLoadingAssets())
		{
			yield return null;
		}
		while (hero.GetCard() == null)
		{
			yield return null;
		}
		Card heroCard = hero.GetCard();
		while (heroCard.GetCardDef() == null)
		{
			yield return null;
		}
		CardDef cardDef = heroCard.GetCardDef();
		for (int i = 0; i < cardDef.m_CustomHeroTraySettings.Count; i++)
		{
			if (this.m_boardDbId == (int)cardDef.m_CustomHeroTraySettings[i].m_Board)
			{
				this.m_TrayTint = cardDef.m_CustomHeroTraySettings[i].m_Tint;
			}
		}
		if (!string.IsNullOrEmpty(cardDef.m_CustomHeroTray))
		{
			AssetLoader.Get().LoadTexture(cardDef.m_CustomHeroTray, new AssetLoader.ObjectCallback(this.OnHeroTrayTextureLoaded), side, false);
		}
		if (UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(cardDef.m_CustomHeroPhoneTray))
		{
			AssetLoader.Get().LoadTexture(cardDef.m_CustomHeroPhoneTray, new AssetLoader.ObjectCallback(this.OnHeroPhoneTrayTextureLoaded), side, false);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			while (ManaCrystalMgr.Get() == null)
			{
				yield return null;
			}
			if (side == Player.Side.FRIENDLY)
			{
				if (!string.IsNullOrEmpty(cardDef.m_CustomHeroPhoneManaGem))
				{
					AssetLoader.Get().LoadTexture(cardDef.m_CustomHeroPhoneManaGem, new AssetLoader.ObjectCallback(this.OnHeroSkinManaGemTextureLoaded), null, false);
				}
				else if (this.m_GemManaPhoneTexture != null)
				{
					ManaCrystalMgr.Get().SetFriendlyManaGemTexture(this.m_GemManaPhoneTexture);
				}
			}
		}
		yield break;
	}

	// Token: 0x06002A1C RID: 10780 RVA: 0x000CE470 File Offset: 0x000CC670
	private void OnHeroSkinManaGemTextureLoaded(string path, Object obj, object callbackData)
	{
		if (obj == null)
		{
			Debug.LogError("OnHeroSkinManaGemTextureLoaded() loaded texture is null!");
			return;
		}
		Texture friendlyManaGemTexture = (Texture)obj;
		ManaCrystalMgr.Get().SetFriendlyManaGemTexture(friendlyManaGemTexture);
		ManaCrystalMgr.Get().SetFriendlyManaGemTint(this.m_TrayTint);
	}

	// Token: 0x06002A1D RID: 10781 RVA: 0x000CE4B8 File Offset: 0x000CC6B8
	private void OnHeroTrayTextureLoaded(string path, Object obj, object callbackData)
	{
		if (obj == null)
		{
			Debug.LogError("Board.OnHeroTrayTextureLoaded() loaded texture is null!");
			return;
		}
		Texture mainTexture = (Texture)obj;
		Player.Side side = (Player.Side)((int)callbackData);
		if (side == Player.Side.FRIENDLY)
		{
			Material material = this.m_FriendlyHeroTray.GetComponentInChildren<MeshRenderer>().material;
			material.mainTexture = mainTexture;
			material.color = this.m_TrayTint;
		}
		else
		{
			Material material2 = this.m_OpponentHeroTray.GetComponentInChildren<MeshRenderer>().material;
			material2.mainTexture = mainTexture;
			material2.color = this.m_TrayTint;
		}
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000CE540 File Offset: 0x000CC740
	private void OnHeroPhoneTrayTextureLoaded(string path, Object obj, object callbackData)
	{
		if (obj == null)
		{
			Debug.LogError("Board.OnHeroTrayTextureLoaded() loaded texture is null!");
			return;
		}
		Texture mainTexture = (Texture)obj;
		Player.Side side = (Player.Side)((int)callbackData);
		if (side == Player.Side.FRIENDLY)
		{
			if (this.m_FriendlyHeroPhoneTray == null)
			{
				Debug.LogWarning("Friendly Hero Phone Tray Object on Board is null!");
				return;
			}
			Material material = this.m_FriendlyHeroPhoneTray.GetComponentInChildren<MeshRenderer>().material;
			material.mainTexture = mainTexture;
			material.color = this.m_TrayTint;
		}
		else
		{
			if (this.m_OpponentHeroPhoneTray == null)
			{
				Debug.LogWarning("Opponent Hero Phone Tray Object on Board is null!");
				return;
			}
			Material material2 = this.m_OpponentHeroPhoneTray.GetComponentInChildren<MeshRenderer>().material;
			material2.mainTexture = mainTexture;
			material2.color = this.m_TrayTint;
		}
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000CE600 File Offset: 0x000CC800
	private void OnHeroTrayEffectLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("Board.OnHeroTrayEffectLoaded() Hero tray effect is null!");
			return;
		}
		Spell component = go.GetComponent<Spell>();
		if (component == null)
		{
			Debug.LogError("Board.OnHeroTrayEffectLoaded() Hero tray effect: could not find spell component!");
			return;
		}
		Player.Side side = (Player.Side)((int)callbackData);
		if (side == Player.Side.FRIENDLY)
		{
			go.transform.parent = base.transform;
			go.transform.position = this.FindBone("CustomSocketIn_Friendly").position;
			this.m_FriendlyTraySpellEffect = component;
		}
		else
		{
			go.transform.parent = base.transform;
			go.transform.position = this.FindBone("CustomSocketIn_Opposing").position;
			this.m_OpponentTraySpellEffect = component;
		}
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000CE6BC File Offset: 0x000CC8BC
	private void LoadBoardSpecialEvent(Board.BoardSpecialEvents boardSpecialEvent)
	{
		Log.Kyle.Print("Loading Board Special Event Prefab: {0}", new object[]
		{
			boardSpecialEvent.Prefab
		});
		string name = FileUtils.GameAssetPathToName(boardSpecialEvent.Prefab);
		GameObject gameObject = AssetLoader.Get().LoadGameObject(name, true, false);
		if (gameObject == null)
		{
			Debug.LogWarning(string.Format("Failed to load special board event: {0}", boardSpecialEvent.Prefab));
		}
		this.m_AmbientColor = boardSpecialEvent.AmbientColorOverride;
	}

	// Token: 0x04001874 RID: 6260
	private const string GOLDEN_HERO_TRAY_FRIENDLY = "HeroTray_Golden_Friendly";

	// Token: 0x04001875 RID: 6261
	private const string GOLDEN_HERO_TRAY_OPPONENT = "HeroTray_Golden_Opponent";

	// Token: 0x04001876 RID: 6262
	private const float MULLIGAN_LIGHT_INTENSITY = 0f;

	// Token: 0x04001877 RID: 6263
	private readonly Color MULLIGAN_AMBIENT_LIGHT_COLOR = new Color(0.1607843f, 0.1921569f, 0.282353f, 1f);

	// Token: 0x04001878 RID: 6264
	public Color m_AmbientColor = Color.white;

	// Token: 0x04001879 RID: 6265
	public Light m_DirectionalLight;

	// Token: 0x0400187A RID: 6266
	public float m_DirectionalLightIntensity = 0.275f;

	// Token: 0x0400187B RID: 6267
	public GameObject m_FriendlyHeroTray;

	// Token: 0x0400187C RID: 6268
	public GameObject m_OpponentHeroTray;

	// Token: 0x0400187D RID: 6269
	public GameObject m_FriendlyHeroPhoneTray;

	// Token: 0x0400187E RID: 6270
	public GameObject m_OpponentHeroPhoneTray;

	// Token: 0x0400187F RID: 6271
	public Transform m_BoneParent;

	// Token: 0x04001880 RID: 6272
	public GameObject m_SplitPlaySurface;

	// Token: 0x04001881 RID: 6273
	public GameObject m_CombinedPlaySurface;

	// Token: 0x04001882 RID: 6274
	public Transform m_ColliderParent;

	// Token: 0x04001883 RID: 6275
	public GameObject m_MouseClickDustEffect;

	// Token: 0x04001884 RID: 6276
	public Color m_ShadowColor = new Color(0.098f, 0.098f, 0.235f, 0.45f);

	// Token: 0x04001885 RID: 6277
	public Color m_DeckColor = Color.white;

	// Token: 0x04001886 RID: 6278
	public Color m_EndTurnButtonColor = Color.white;

	// Token: 0x04001887 RID: 6279
	public Color m_HistoryTileColor = Color.white;

	// Token: 0x04001888 RID: 6280
	public Color m_GoldenHeroTrayColor = Color.white;

	// Token: 0x04001889 RID: 6281
	public List<Board.BoardSpecialEvents> m_SpecialEvents;

	// Token: 0x0400188A RID: 6282
	public MusicPlaylistType m_BoardMusic = MusicPlaylistType.InGame_Default;

	// Token: 0x0400188B RID: 6283
	public Texture m_GemManaPhoneTexture;

	// Token: 0x0400188C RID: 6284
	private static Board s_instance;

	// Token: 0x0400188D RID: 6285
	private bool m_raisedLights;

	// Token: 0x0400188E RID: 6286
	private Spell m_FriendlyTraySpellEffect;

	// Token: 0x0400188F RID: 6287
	private Spell m_OpponentTraySpellEffect;

	// Token: 0x04001890 RID: 6288
	private int m_boardDbId;

	// Token: 0x04001891 RID: 6289
	private Color m_TrayTint = Color.white;

	// Token: 0x02000329 RID: 809
	[Serializable]
	public class CustomTraySettings
	{
		// Token: 0x04001894 RID: 6292
		public BoardDdId m_Board;

		// Token: 0x04001895 RID: 6293
		public Color m_Tint = Color.white;
	}

	// Token: 0x020006AE RID: 1710
	[Serializable]
	public class BoardSpecialEvents
	{
		// Token: 0x04002ED3 RID: 11987
		public SpecialEventType EventType;

		// Token: 0x04002ED4 RID: 11988
		[CustomEditField(T = EditType.GAME_OBJECT)]
		public string Prefab;

		// Token: 0x04002ED5 RID: 11989
		public Color AmbientColorOverride = Color.white;
	}
}
