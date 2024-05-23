using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000617 RID: 1559
public class MulliganManager : MonoBehaviour
{
	// Token: 0x0600437E RID: 17278 RVA: 0x00142F49 File Offset: 0x00141149
	private void Awake()
	{
		MulliganManager.s_instance = this;
	}

	// Token: 0x0600437F RID: 17279 RVA: 0x00142F54 File Offset: 0x00141154
	private void OnDestroy()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
			GameState.Get().UnregisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
			GameState.Get().UnregisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
			GameState.Get().UnregisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
		MulliganManager.s_instance = null;
	}

	// Token: 0x06004380 RID: 17280 RVA: 0x00142FD0 File Offset: 0x001411D0
	private void Start()
	{
		if (GameState.Get().IsGameCreatedOrCreating())
		{
			this.HandleGameStart();
		}
		else
		{
			GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
		}
		GameState.Get().RegisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
		GameState.Get().RegisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
		GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		if (UniversalInputManager.UsePhoneUI)
		{
			this.myheroAnimatesToPosition = this.myheroAnimatesToPosition_iPhone;
			this.hisheroAnimatesToPosition = this.hisheroAnimatesToPosition_iPhone;
			this.cardAnimatesFromBoardToDeck = this.cardAnimatesFromBoardToDeck_iPhone;
		}
	}

	// Token: 0x06004381 RID: 17281 RVA: 0x00143087 File Offset: 0x00141287
	public static MulliganManager Get()
	{
		return MulliganManager.s_instance;
	}

	// Token: 0x06004382 RID: 17282 RVA: 0x0014308E File Offset: 0x0014128E
	public bool IsMulliganActive()
	{
		return this.mulliganActive;
	}

	// Token: 0x06004383 RID: 17283 RVA: 0x00143096 File Offset: 0x00141296
	public void ForceMulliganActive(bool active)
	{
		this.mulliganActive = active;
	}

	// Token: 0x06004384 RID: 17284 RVA: 0x001430A0 File Offset: 0x001412A0
	private IEnumerator WaitForBoardThenLoadButton()
	{
		while (BoardStandardGame.Get() == null)
		{
			yield return null;
		}
		AssetLoader.Get().LoadActor("MulliganButton", new AssetLoader.GameObjectCallback(this.OnMulliganButtonLoaded), null, false);
		yield break;
	}

	// Token: 0x06004385 RID: 17285 RVA: 0x001430BC File Offset: 0x001412BC
	private void OnMulliganButtonLoaded(string name, GameObject go, object userData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("MulliganManager.OnMulliganButtonLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.mulliganButton = go.GetComponent<NormalButton>();
		if (this.mulliganButton == null)
		{
			Debug.LogError(string.Format("MulliganManager.OnMulliganButtonLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(NormalButton)));
			return;
		}
		this.mulliganButton.SetText(GameStrings.Get("GLOBAL_CONFIRM"));
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x00143134 File Offset: 0x00141334
	private void OnVersusVoLoaded(string name, GameObject go, object userData)
	{
		this.waitingForVersusVo = false;
		if (go == null)
		{
			Debug.LogError(string.Format("MulliganManager.OnVersusVoLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.versusVo = go.GetComponent<AudioSource>();
		if (this.versusVo == null)
		{
			Debug.LogError(string.Format("MulliganManager.OnVersusVoLoaded() - ERROR \"{0}\" has no {1} component", name, typeof(AudioSource)));
			return;
		}
	}

	// Token: 0x06004387 RID: 17287 RVA: 0x0014319D File Offset: 0x0014139D
	private void OnVersusTextLoaded(string name, GameObject go, object userData)
	{
		this.waitingForVersusText = false;
		if (go == null)
		{
			Debug.LogError(string.Format("MulliganManager.OnVersusTextLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		this.versusTextObject = go;
	}

	// Token: 0x06004388 RID: 17288 RVA: 0x001431CC File Offset: 0x001413CC
	private IEnumerator WaitForHeroesAndStartAnimations()
	{
		Log.LoadingScreen.Print("MulliganManager.WaitForHeroesAndStartAnimations()", new object[0]);
		Player friendlyPlayer = GameState.Get().GetFriendlySidePlayer();
		Player opposingPlayer = GameState.Get().GetOpposingSidePlayer();
		while (friendlyPlayer == null)
		{
			yield return null;
			friendlyPlayer = GameState.Get().GetFriendlySidePlayer();
		}
		while (opposingPlayer == null)
		{
			yield return null;
			opposingPlayer = GameState.Get().GetOpposingSidePlayer();
		}
		while (this.myHeroCardActor == null)
		{
			Card heroCard = friendlyPlayer.GetHeroCard();
			if (heroCard != null)
			{
				this.myHeroCardActor = heroCard.GetActor();
			}
			yield return null;
		}
		while (this.hisHeroCardActor == null)
		{
			Card heroCard2 = opposingPlayer.GetHeroCard();
			if (heroCard2 != null)
			{
				this.hisHeroCardActor = heroCard2.GetActor();
			}
			yield return null;
		}
		while (friendlyPlayer.GetHeroPower() != null && this.myHeroPowerCardActor == null)
		{
			Card heroPowerCard = friendlyPlayer.GetHeroPowerCard();
			if (heroPowerCard != null)
			{
				this.myHeroPowerCardActor = heroPowerCard.GetActor();
				if (this.myHeroPowerCardActor != null)
				{
					this.myHeroPowerCardActor.TurnOffCollider();
				}
			}
			yield return null;
		}
		while (opposingPlayer.GetHeroPower() != null && this.hisHeroPowerCardActor == null)
		{
			Card heroPowerCard2 = opposingPlayer.GetHeroPowerCard();
			if (heroPowerCard2 != null)
			{
				this.hisHeroPowerCardActor = heroPowerCard2.GetActor();
				if (this.hisHeroPowerCardActor != null)
				{
					this.hisHeroPowerCardActor.TurnOffCollider();
				}
			}
			yield return null;
		}
		while (GameState.Get() == null || GameState.Get().GetGameEntity().IsPreloadingAssets())
		{
			yield return null;
		}
		while (this.myHeroCardActor.GetCardDef() == null)
		{
			yield return null;
		}
		while (this.hisHeroCardActor.GetCardDef() == null)
		{
			yield return null;
		}
		this.LoadMyHeroSkinSocketInEffect(this.myHeroCardActor.GetCardDef());
		this.LoadHisHeroSkinSocketInEffect(this.hisHeroCardActor.GetCardDef());
		while (this.m_isLoadingMyCustomSocketIn || this.m_isLoadingHisCustomSocketIn)
		{
			yield return null;
		}
		Material myHeroMat = this.myHeroCardActor.m_portraitMesh.GetComponent<Renderer>().materials[this.myHeroCardActor.m_portraitMatIdx];
		Material myHeroFrameMat = this.myHeroCardActor.m_portraitMesh.GetComponent<Renderer>().materials[this.myHeroCardActor.m_portraitFrameMatIdx];
		if (myHeroMat != null && myHeroMat.HasProperty("_LightingBlend"))
		{
			myHeroMat.SetFloat("_LightingBlend", 0f);
		}
		if (myHeroFrameMat != null && myHeroFrameMat.HasProperty("_LightingBlend"))
		{
			myHeroFrameMat.SetFloat("_LightingBlend", 0f);
		}
		Material hisHeroMat = this.hisHeroCardActor.m_portraitMesh.GetComponent<Renderer>().materials[this.hisHeroCardActor.m_portraitMatIdx];
		Material hisHeroFrameMat = this.hisHeroCardActor.m_portraitMesh.GetComponent<Renderer>().materials[this.hisHeroCardActor.m_portraitFrameMatIdx];
		if (hisHeroMat != null && hisHeroMat.HasProperty("_LightingBlend"))
		{
			hisHeroMat.SetFloat("_LightingBlend", 0f);
		}
		if (hisHeroFrameMat != null && hisHeroFrameMat.HasProperty("_LightingBlend"))
		{
			hisHeroFrameMat.SetFloat("_LightingBlend", 0f);
		}
		this.myHeroCardActor.TurnOffCollider();
		this.hisHeroCardActor.TurnOffCollider();
		GameState.Get().GetGameEntity().NotifyOfMulliganInitialized();
		bool alternateIntro = GameState.Get().GetGameEntity().DoAlternateMulliganIntro();
		if (alternateIntro)
		{
			this.introComplete = true;
			yield break;
		}
		while (this.waitingForVersusText || this.waitingForVersusVo)
		{
			yield return null;
		}
		Log.LoadingScreen.Print("MulliganManager.WaitForHeroesAndStartAnimations() - NotifySceneLoaded()", new object[0]);
		SceneMgr.Get().NotifySceneLoaded();
		while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
		{
			yield return null;
		}
		GameMgr.Get().UpdatePresence();
		GameObject myHero = this.myHeroCardActor.gameObject;
		GameObject hisHero = this.hisHeroCardActor.gameObject;
		this.myHeroCardActor.GetHealthObject().Hide();
		this.hisHeroCardActor.GetHealthObject().Hide();
		if (this.versusTextObject)
		{
			this.versusTextObject.transform.position = Board.Get().FindBone("VS_Position").position;
		}
		GameObject heroLabelCopy = Object.Instantiate<GameObject>(this.heroLabelPrefab);
		this.myheroLabel = heroLabelCopy.GetComponent<HeroLabel>();
		this.myheroLabel.transform.parent = this.myHeroCardActor.GetMeshRenderer().transform;
		this.myheroLabel.transform.localPosition = new Vector3(0f, 0f, 0f);
		TAG_CLASS classTag = this.myHeroCardActor.GetEntity().GetClass();
		string className = string.Empty;
		if (classTag != TAG_CLASS.INVALID && !GameMgr.Get().IsTutorial())
		{
			className = GameStrings.GetClassName(classTag).ToUpper();
		}
		this.myheroLabel.UpdateText(this.myHeroCardActor.GetEntity().GetName(), className);
		heroLabelCopy = Object.Instantiate<GameObject>(this.heroLabelPrefab);
		this.hisheroLabel = heroLabelCopy.GetComponent<HeroLabel>();
		this.hisheroLabel.transform.parent = this.hisHeroCardActor.GetMeshRenderer().transform;
		this.hisheroLabel.transform.localPosition = new Vector3(0f, 0f, 0f);
		classTag = this.hisHeroCardActor.GetEntity().GetClass();
		className = string.Empty;
		if (classTag != TAG_CLASS.INVALID && !GameMgr.Get().IsTutorial())
		{
			className = GameStrings.GetClassName(classTag).ToUpper();
		}
		this.hisheroLabel.UpdateText(this.hisHeroCardActor.GetEntity().GetName(), className);
		if (GameState.Get().WasConcedeRequested())
		{
			yield break;
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_Mulligan);
		Animation cardAnim = myHero.GetComponent<Animation>();
		if (cardAnim == null)
		{
			cardAnim = myHero.AddComponent<Animation>();
		}
		cardAnim.AddClip(this.hisheroAnimatesToPosition, "hisHeroAnimateToPosition");
		base.StartCoroutine(this.SampleAnimFrame(cardAnim, "hisHeroAnimateToPosition", 0f));
		Animation oppCardAnim = hisHero.GetComponent<Animation>();
		if (oppCardAnim == null)
		{
			oppCardAnim = hisHero.AddComponent<Animation>();
		}
		oppCardAnim.AddClip(this.myheroAnimatesToPosition, "myHeroAnimateToPosition");
		base.StartCoroutine(this.SampleAnimFrame(oppCardAnim, "myHeroAnimateToPosition", 0f));
		while (LoadingScreen.Get().IsTransitioning())
		{
			yield return null;
		}
		if (this.versusVo)
		{
			AudioSource myHeroLine = this.myHeroCardActor.GetCard().GetAnnouncerLine();
			SoundManager.Get().Play(myHeroLine);
			while (SoundManager.Get().IsActive(myHeroLine))
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.05f);
			SoundManager.Get().PlayPreloaded(this.versusVo);
			while (SoundManager.Get().IsActive(this.versusVo))
			{
				yield return null;
			}
			yield return new WaitForSeconds(0.05f);
			AudioSource hisHeroLine = this.hisHeroCardActor.GetCard().GetAnnouncerLine();
			if (hisHeroLine != null && hisHeroLine.clip != null)
			{
				SoundManager.Get().Play(hisHeroLine);
				while (SoundManager.Get().IsActive(hisHeroLine))
				{
					yield return null;
				}
			}
		}
		yield return base.StartCoroutine(GameState.Get().GetGameEntity().PlayMissionIntroLineAndWait());
		this.myheroLabel.transform.parent = null;
		this.hisheroLabel.transform.parent = null;
		this.myheroLabel.FadeOut();
		this.hisheroLabel.FadeOut();
		yield return new WaitForSeconds(0.5f);
		if (this.m_MyCustomSocketInSpell != null)
		{
			this.m_MyCustomSocketInSpell.m_Location = SpellLocation.NONE;
			this.m_MyCustomSocketInSpell.gameObject.SetActive(true);
			if (this.myHeroCardActor.GetCardDef().m_SocketInParentEffectToHero)
			{
				Vector3 myActorScale = this.myHeroCardActor.transform.localScale;
				this.myHeroCardActor.transform.localScale = Vector3.one;
				this.m_MyCustomSocketInSpell.transform.parent = this.myHeroCardActor.transform;
				this.m_MyCustomSocketInSpell.transform.localPosition = Vector3.zero;
				this.myHeroCardActor.transform.localScale = myActorScale;
			}
			this.m_MyCustomSocketInSpell.SetSource(this.myHeroCardActor.GetCard().gameObject);
			this.m_MyCustomSocketInSpell.RemoveAllTargets();
			GameObject myHeroSocketBone = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).gameObject;
			this.m_MyCustomSocketInSpell.AddTarget(myHeroSocketBone);
			this.m_MyCustomSocketInSpell.ActivateState(SpellStateType.BIRTH);
			this.m_MyCustomSocketInSpell.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
			{
				this.myHeroCardActor.transform.position = myHeroSocketBone.transform.position;
				this.myHeroCardActor.transform.localScale = Vector3.one;
			});
			if (!this.myHeroCardActor.GetCardDef().m_SocketInOverrideHeroAnimation)
			{
				cardAnim["hisHeroAnimateToPosition"].enabled = true;
			}
		}
		else
		{
			cardAnim["hisHeroAnimateToPosition"].enabled = true;
		}
		if (this.m_HisCustomSocketInSpell != null)
		{
			if (this.m_MyCustomSocketInSpell)
			{
				SoundUtils.SetSourceVolumes(this.m_HisCustomSocketInSpell, 0f, false);
			}
			this.m_HisCustomSocketInSpell.m_Location = SpellLocation.NONE;
			if (this.hisHeroCardActor.GetCardDef().m_SocketInOverrideHeroAnimation)
			{
				yield return new WaitForSeconds(0.25f);
			}
			this.m_HisCustomSocketInSpell.gameObject.SetActive(true);
			if (this.hisHeroCardActor.GetCardDef().m_SocketInParentEffectToHero)
			{
				Vector3 hisActorScale = this.hisHeroCardActor.transform.localScale;
				this.hisHeroCardActor.transform.localScale = Vector3.one;
				this.m_HisCustomSocketInSpell.transform.parent = this.hisHeroCardActor.transform;
				this.m_HisCustomSocketInSpell.transform.localPosition = Vector3.zero;
				this.hisHeroCardActor.transform.localScale = hisActorScale;
			}
			this.m_HisCustomSocketInSpell.SetSource(this.hisHeroCardActor.GetCard().gameObject);
			this.m_HisCustomSocketInSpell.RemoveAllTargets();
			GameObject hisHeroSocketBone = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).gameObject;
			this.m_HisCustomSocketInSpell.AddTarget(hisHeroSocketBone);
			this.m_HisCustomSocketInSpell.ActivateState(SpellStateType.BIRTH);
			this.m_HisCustomSocketInSpell.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
			{
				this.hisHeroCardActor.transform.position = hisHeroSocketBone.transform.position;
				this.hisHeroCardActor.transform.localScale = Vector3.one;
			});
			if (!this.hisHeroCardActor.GetCardDef().m_SocketInOverrideHeroAnimation)
			{
				oppCardAnim["myHeroAnimateToPosition"].enabled = true;
			}
		}
		else
		{
			oppCardAnim["myHeroAnimateToPosition"].enabled = true;
		}
		SoundManager.Get().LoadAndPlay("FX_MulliganCoin01_HeroCoinDrop", this.hisHeroCardActor.GetCard().gameObject);
		if (this.versusTextObject)
		{
			yield return new WaitForSeconds(0.1f);
			this.versusTextObject.GetComponentInChildren<Animation>().Play();
			yield return new WaitForSeconds(0.32f);
		}
		if (this.m_MyCustomSocketInSpell == null)
		{
			this.myWeldEffect = Object.Instantiate<GameObject>(this.weldPrefab);
			this.myWeldEffect.transform.position = myHero.transform.position;
			if (this.m_HisCustomSocketInSpell)
			{
				SoundUtils.SetSourceVolumes(this.myWeldEffect, 0f, false);
			}
			this.myWeldEffect.GetComponent<HeroWeld>().DoAnim();
		}
		if (this.m_HisCustomSocketInSpell == null)
		{
			this.hisWeldEffect = Object.Instantiate<GameObject>(this.weldPrefab);
			this.hisWeldEffect.transform.position = hisHero.transform.position;
			if (this.m_MyCustomSocketInSpell)
			{
				SoundUtils.SetSourceVolumes(this.hisWeldEffect, 0f, false);
			}
			this.hisWeldEffect.GetComponent<HeroWeld>().DoAnim();
		}
		yield return new WaitForSeconds(0.05f);
		iTween.ShakePosition(Camera.main.gameObject, iTween.Hash(new object[]
		{
			"time",
			0.6f,
			"amount",
			new Vector3(0.03f, 0.01f, 0.03f)
		}));
		Action<object> OnMyLightBlendUpdate = delegate(object amount)
		{
			myHeroMat.SetFloat("_LightingBlend", (float)amount);
			myHeroFrameMat.SetFloat("_LightingBlend", (float)amount);
		};
		OnMyLightBlendUpdate.Invoke(0f);
		Hashtable myLightBlendArgs = iTween.Hash(new object[]
		{
			"time",
			1f,
			"from",
			0f,
			"to",
			1f,
			"delay",
			2f,
			"onupdate",
			OnMyLightBlendUpdate,
			"onupdatetarget",
			base.gameObject,
			"name",
			"MyHeroLightBlend"
		});
		iTween.ValueTo(base.gameObject, myLightBlendArgs);
		Action<object> OnHisLightBlendUpdate = delegate(object amount)
		{
			hisHeroMat.SetFloat("_LightingBlend", (float)amount);
			hisHeroFrameMat.SetFloat("_LightingBlend", (float)amount);
		};
		OnHisLightBlendUpdate.Invoke(0f);
		Hashtable hisLightBlendArgs = iTween.Hash(new object[]
		{
			"time",
			1f,
			"from",
			0f,
			"to",
			1f,
			"delay",
			2f,
			"onupdate",
			OnHisLightBlendUpdate,
			"onupdatetarget",
			base.gameObject,
			"name",
			"HisHeroLightBlend"
		});
		iTween.ValueTo(base.gameObject, hisLightBlendArgs);
		this.introComplete = true;
		GameState.Get().GetGameEntity().NotifyOfHeroesFinishedAnimatingInMulligan();
		yield break;
	}

	// Token: 0x06004389 RID: 17289 RVA: 0x001431E8 File Offset: 0x001413E8
	public void BeginMulligan()
	{
		bool flag = this.mulliganActive;
		this.mulliganActive = true;
		if (GameState.Get().WasConcedeRequested())
		{
			this.HandleGameOverDuringMulligan();
		}
		else
		{
			if (flag && SpectatorManager.Get().IsSpectatingOpposingSide())
			{
				return;
			}
			base.StartCoroutine(this.ContinueMulliganWhenBoardLoads());
		}
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x00143240 File Offset: 0x00141440
	private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
	{
		GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
		this.HandleGameStart();
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x00143260 File Offset: 0x00141460
	private void HandleGameStart()
	{
		Log.LoadingScreen.Print("MulliganManager.HandleGameStart() - IsPastBeginPhase()={0}", new object[]
		{
			GameState.Get().IsPastBeginPhase()
		});
		if (GameState.Get().IsPastBeginPhase())
		{
			base.StartCoroutine(this.SkipMulliganForResume());
			return;
		}
		if (!GameState.Get().GetGameEntity().ShouldDoAlternateMulliganIntro())
		{
			this.m_xLabels = new GameObject[4];
			this.coinObject = Object.Instantiate<GameObject>(this.coinPrefab);
			this.coinObject.SetActive(false);
			if (!Cheats.Get().QuickGameSkipMulligan())
			{
				if (Cheats.Get().IsLaunchingQuickGame())
				{
					Time.timeScale = SceneDebugger.GetDevTimescale();
				}
				this.waitingForVersusVo = true;
				AssetLoader.Get().LoadSound("VO_ANNOUNCER_VERSUS_21", new AssetLoader.GameObjectCallback(this.OnVersusVoLoaded), null, false, null);
			}
			this.waitingForVersusText = true;
			AssetLoader.Get().LoadGameObject("GameStart_VS_Letters", new AssetLoader.GameObjectCallback(this.OnVersusTextLoaded), null, false);
			base.StartCoroutine("WaitForBoardThenLoadButton");
		}
		base.StartCoroutine("WaitForHeroesAndStartAnimations");
		Log.LoadingScreen.Print("MulliganManager.HandleGameStart() - IsMulliganPhase()={0}", new object[]
		{
			GameState.Get().IsMulliganPhase()
		});
		if (GameState.Get().IsMulliganPhase())
		{
			base.StartCoroutine(this.ResumeMulligan());
		}
	}

	// Token: 0x0600438C RID: 17292 RVA: 0x001433BC File Offset: 0x001415BC
	private IEnumerator ResumeMulligan()
	{
		this.m_resuming = true;
		foreach (Player player in GameState.Get().GetPlayerMap().Values)
		{
			TAG_MULLIGAN mulliganState = player.GetTag<TAG_MULLIGAN>(GAME_TAG.MULLIGAN_STATE);
			if (mulliganState == TAG_MULLIGAN.DONE)
			{
				if (player.IsFriendlySide())
				{
					this.friendlyPlayerHasReplacementCards = true;
				}
				else
				{
					this.opponentPlayerHasReplacementCards = true;
				}
			}
		}
		if (this.friendlyPlayerHasReplacementCards)
		{
			this.skipCardChoosing = true;
		}
		else
		{
			while (GameState.Get().GetResponseMode() != GameState.ResponseMode.CHOICE)
			{
				yield return null;
			}
		}
		this.BeginMulligan();
		yield break;
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x001433D8 File Offset: 0x001415D8
	private void OnMulliganTimerUpdate(TurnTimerUpdate update, object userData)
	{
		if (update.GetSecondsRemaining() > Mathf.Epsilon)
		{
			if (update.ShouldShow())
			{
				this.BeginMulliganCountdown(update.GetEndTimestamp());
			}
			return;
		}
		GameState.Get().UnregisterMulliganTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnMulliganTimerUpdate));
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x00143424 File Offset: 0x00141624
	private bool OnEntitiesChosenReceived(Network.EntitiesChosen chosen, Network.EntityChoices choices, object userData)
	{
		if (!GameMgr.Get().IsSpectator())
		{
			return false;
		}
		int playerId = chosen.PlayerId;
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		if (playerId == friendlyPlayerId)
		{
			base.StartCoroutine(this.Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen(chosen));
			return true;
		}
		return false;
	}

	// Token: 0x0600438F RID: 17295 RVA: 0x0014346C File Offset: 0x0014166C
	private void OnGameOver(object userData)
	{
		this.HandleGameOverDuringMulligan();
	}

	// Token: 0x06004390 RID: 17296 RVA: 0x00143474 File Offset: 0x00141674
	private IEnumerator Spectator_WaitForFriendlyPlayerThenProcessEntitiesChosen(Network.EntitiesChosen chosen)
	{
		while (!this.m_waitingForUserInput)
		{
			if (GameState.Get().IsGameOver())
			{
				yield break;
			}
			if (this.skipCardChoosing)
			{
				yield break;
			}
			yield return null;
		}
		for (int cardIndex = 0; cardIndex < this.m_startingCards.Count; cardIndex++)
		{
			Card card = this.m_startingCards[cardIndex];
			int entityId = card.GetEntity().GetEntityId();
			bool spectateeMarkedReplaced = !chosen.Entities.Contains(entityId);
			if (this.m_handCardsMarkedForReplace[cardIndex] != spectateeMarkedReplaced)
			{
				this.ToggleHoldState(cardIndex);
			}
		}
		GameState.Get().OnEntitiesChosenProcessed(chosen);
		this.BeginDealNewCards();
		yield break;
	}

	// Token: 0x06004391 RID: 17297 RVA: 0x001434A0 File Offset: 0x001416A0
	private IEnumerator ContinueMulliganWhenBoardLoads()
	{
		while (ZoneMgr.Get() == null)
		{
			yield return null;
		}
		Board board = Board.Get();
		board.SetMulliganLighting();
		this.startingHandZone = board.FindBone("StartingHandZone").gameObject;
		this.InitZones();
		if (this.m_resuming)
		{
			while (this.ShouldWaitForMulliganCardsToBeProcessed())
			{
				yield return null;
			}
		}
		this.SortHand(this.friendlySideHandZone);
		this.SortHand(this.opposingSideHandZone);
		board.CombinedSurface();
		Collider dragPlane = board.FindCollider("DragPlane");
		dragPlane.enabled = false;
		base.StartCoroutine("DealStartingCards");
		yield break;
	}

	// Token: 0x06004392 RID: 17298 RVA: 0x001434BC File Offset: 0x001416BC
	private void InitZones()
	{
		List<Zone> zones = ZoneMgr.Get().GetZones();
		foreach (Zone zone in zones)
		{
			if (zone is ZoneHand)
			{
				if (zone.m_Side == Player.Side.FRIENDLY)
				{
					this.friendlySideHandZone = (ZoneHand)zone;
				}
				else
				{
					this.opposingSideHandZone = (ZoneHand)zone;
				}
			}
			if (zone is ZoneDeck)
			{
				if (zone.m_Side == Player.Side.FRIENDLY)
				{
					this.friendlySideDeck = (ZoneDeck)zone;
					this.friendlySideDeck.SetSuppressEmotes(true);
					this.friendlySideDeck.UpdateLayout();
				}
				else
				{
					this.opposingSideDeck = (ZoneDeck)zone;
					this.opposingSideDeck.SetSuppressEmotes(true);
					this.opposingSideDeck.UpdateLayout();
				}
			}
		}
	}

	// Token: 0x06004393 RID: 17299 RVA: 0x001435AC File Offset: 0x001417AC
	private bool ShouldWaitForMulliganCardsToBeProcessed()
	{
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		bool receivedEndOfMulligan = false;
		powerProcessor.ForEachTaskList(delegate(int index, PowerTaskList taskList)
		{
			if (this.IsTaskListPuttingUsPastMulligan(taskList))
			{
				receivedEndOfMulligan = true;
				return;
			}
		});
		return !receivedEndOfMulligan && powerProcessor.HasTaskLists();
	}

	// Token: 0x06004394 RID: 17300 RVA: 0x00143600 File Offset: 0x00141800
	private bool IsTaskListPuttingUsPastMulligan(PowerTaskList taskList)
	{
		List<PowerTask> taskList2 = taskList.GetTaskList();
		foreach (PowerTask powerTask in taskList2)
		{
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = power as Network.HistTagChange;
				if (histTagChange.Tag == 198 && GameUtils.IsPastBeginPhase((TAG_STEP)histTagChange.Value))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004395 RID: 17301 RVA: 0x001436A4 File Offset: 0x001418A4
	private void GetStartingLists()
	{
		List<Card> cards = this.friendlySideHandZone.GetCards();
		List<Card> cards2 = this.opposingSideHandZone.GetCards();
		int num;
		if (this.ShouldHandleCoinCard())
		{
			if (this.friendlyPlayerGoesFirst)
			{
				num = cards.Count;
				this.m_bonusCardIndex = cards2.Count - 2;
				this.m_coinCardIndex = cards2.Count - 1;
			}
			else
			{
				num = cards.Count - 1;
				this.m_bonusCardIndex = cards.Count - 2;
			}
		}
		else
		{
			num = cards.Count;
			if (this.friendlyPlayerGoesFirst)
			{
				this.m_bonusCardIndex = cards2.Count - 1;
			}
			else
			{
				this.m_bonusCardIndex = cards.Count - 1;
			}
		}
		this.m_startingCards = new List<Card>();
		for (int i = 0; i < num; i++)
		{
			this.m_startingCards.Add(cards[i]);
		}
		this.m_startingOppCards = new List<Card>();
		for (int j = 0; j < cards2.Count; j++)
		{
			this.m_startingOppCards.Add(cards2[j]);
		}
	}

	// Token: 0x06004396 RID: 17302 RVA: 0x001437C0 File Offset: 0x001419C0
	private IEnumerator PlayStartingTaunts()
	{
		Card heroCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
		Card heroPowerCard = GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard();
		iTween.StopByName(base.gameObject, "HisHeroLightBlend");
		if (heroPowerCard != null)
		{
			GameState.Get().GetGameEntity().FadeInActor(heroPowerCard.GetActor(), 0.4f);
		}
		GameState.Get().GetGameEntity().FadeInHeroActor(heroCard.GetActor());
		CardSoundSpell emoteSpell = heroCard.PlayEmote(EmoteType.START);
		if (emoteSpell != null)
		{
			while (emoteSpell.GetActiveState() != SpellStateType.NONE)
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(2.5f);
		}
		GameState.Get().GetGameEntity().FadeOutHeroActor(heroCard.GetActor());
		if (heroPowerCard != null)
		{
			GameState.Get().GetGameEntity().FadeOutActor(heroPowerCard.GetActor());
		}
		Card myHeroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
		Card myHeroPowerCard = GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard();
		if (MulliganManager.Get() == null)
		{
			yield break;
		}
		iTween.StopByName(base.gameObject, "MyHeroLightBlend");
		if (myHeroPowerCard != null)
		{
			GameState.Get().GetGameEntity().FadeInActor(myHeroPowerCard.GetActor(), 0.4f);
		}
		EmoteType emoteToPlay = EmoteType.START;
		if (myHeroCard.GetEntity().GetCardId() == heroCard.GetEntity().GetCardId())
		{
			emoteToPlay = EmoteType.MIRROR_START;
		}
		GameState.Get().GetGameEntity().FadeInHeroActor(myHeroCard.GetActor());
		emoteSpell = myHeroCard.PlayEmote(emoteToPlay, Notification.SpeechBubbleDirection.BottomRight);
		if (emoteSpell != null)
		{
			while (emoteSpell.GetActiveState() != SpellStateType.NONE)
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(2.5f);
		}
		GameState.Get().GetGameEntity().FadeOutHeroActor(myHeroCard.GetActor());
		if (myHeroPowerCard != null)
		{
			GameState.Get().GetGameEntity().FadeOutActor(myHeroPowerCard.GetActor());
		}
		yield break;
	}

	// Token: 0x06004397 RID: 17303 RVA: 0x001437DC File Offset: 0x001419DC
	private IEnumerator DealStartingCards()
	{
		yield return new WaitForSeconds(1f);
		while (!this.introComplete)
		{
			yield return null;
		}
		yield return base.StartCoroutine(GameState.Get().GetGameEntity().DoActionsAfterIntroBeforeMulligan());
		if (GameState.Get().GetGameEntity().ShouldDoOpeningTaunts() && !Cheats.Get().QuickGameSkipMulligan())
		{
			base.StartCoroutine("PlayStartingTaunts");
		}
		Player friendlyPlayer = GameState.Get().GetFriendlySidePlayer();
		this.friendlyPlayerGoesFirst = friendlyPlayer.HasTag(GAME_TAG.FIRST_PLAYER);
		this.GetStartingLists();
		foreach (Card card in this.m_startingCards)
		{
			card.GetActor().SetActorState(ActorStateType.CARD_IDLE);
			card.GetActor().TurnOffCollider();
			card.GetActor().GetMeshRenderer().gameObject.layer = 8;
		}
		float zoneWidth = this.startingHandZone.GetComponent<Collider>().bounds.size.x;
		if (UniversalInputManager.UsePhoneUI)
		{
			zoneWidth *= 0.55f;
		}
		float spaceForEachCard = zoneWidth / (float)this.m_startingCards.Count;
		float spaceForEachCardPre4th = zoneWidth / (float)(this.m_startingCards.Count + 1);
		float spacingToUse = spaceForEachCardPre4th;
		float leftSideOfZone = this.startingHandZone.transform.position.x - zoneWidth / 2f;
		float rightSideOfZone = this.startingHandZone.transform.position.x + zoneWidth / 2f;
		float timingBonus = 0.1f;
		int numCardsToDealExcludingBonusCard = this.m_startingCards.Count;
		if (!this.friendlyPlayerGoesFirst)
		{
			numCardsToDealExcludingBonusCard = this.m_bonusCardIndex;
			spacingToUse = spaceForEachCard;
		}
		else if (this.m_startingOppCards.Count > 0)
		{
			this.m_startingOppCards[this.m_bonusCardIndex].SetDoNotSort(true);
			if (this.m_coinCardIndex >= 0)
			{
				this.m_startingOppCards[this.m_coinCardIndex].SetDoNotSort(true);
			}
		}
		this.opposingSideHandZone.SetDoNotUpdateLayout(false);
		this.opposingSideHandZone.UpdateLayout(-1, true, 3);
		float cardHeightOffset = 0f;
		if (UniversalInputManager.UsePhoneUI)
		{
			cardHeightOffset = 7f;
		}
		float cardZpos = this.startingHandZone.transform.position.z - 0.3f;
		if (UniversalInputManager.UsePhoneUI)
		{
			cardZpos = this.startingHandZone.transform.position.z - 0.2f;
		}
		float xOffset = spacingToUse / 2f;
		for (int i = 0; i < numCardsToDealExcludingBonusCard; i++)
		{
			GameObject topCard = this.m_startingCards[i].gameObject;
			iTween.Stop(topCard);
			iTween.MoveTo(topCard, iTween.Hash(new object[]
			{
				"path",
				new Vector3[]
				{
					topCard.transform.position,
					new Vector3(topCard.transform.position.x, topCard.transform.position.y + 3.6f, topCard.transform.position.z),
					new Vector3(leftSideOfZone + xOffset, this.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
				},
				"time",
				1.5f,
				"easetype",
				iTween.EaseType.easeInSineOutExpo
			}));
			if (UniversalInputManager.UsePhoneUI)
			{
				iTween.ScaleTo(topCard, new Vector3(0.9f, 1.1f, 0.9f), 1.5f);
			}
			else
			{
				iTween.ScaleTo(topCard, new Vector3(1.1f, 1.1f, 1.1f), 1.5f);
			}
			iTween.RotateTo(topCard, iTween.Hash(new object[]
			{
				"rotation",
				new Vector3(0f, 0f, 0f),
				"time",
				1.5f,
				"delay",
				0.09375f
			}));
			yield return new WaitForSeconds(0.04f);
			SoundManager.Get().LoadAndPlay("FX_GameStart09_CardsOntoTable", topCard);
			xOffset += spacingToUse;
			yield return new WaitForSeconds(0.05f + timingBonus);
			timingBonus = 0f;
		}
		if (this.skipCardChoosing)
		{
			this.mulliganChooseBanner = Object.Instantiate<GameObject>(this.mulliganChooseBannerPrefab);
			this.mulliganChooseBanner.GetComponent<Banner>().SetText(GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND"));
			Vector3 mulliganChooseBannerPosition = Board.Get().FindBone("ChoiceBanner").position;
			this.mulliganChooseBanner.transform.position = mulliganChooseBannerPosition;
			Vector3 startingScale = this.mulliganChooseBanner.transform.localScale;
			this.mulliganChooseBanner.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
			iTween.ScaleTo(this.mulliganChooseBanner, startingScale, 0.5f);
			base.StartCoroutine(this.ShrinkStartingHandBanner(this.mulliganChooseBanner));
		}
		yield return new WaitForSeconds(1.1f);
		Transform coinSpawnLocation = Board.Get().FindBone("MulliganCoinPosition");
		this.coinObject.transform.position = coinSpawnLocation.position;
		this.coinObject.transform.localEulerAngles = coinSpawnLocation.localEulerAngles;
		this.coinObject.SetActive(true);
		this.coinObject.GetComponent<CoinEffect>().DoAnim(this.friendlyPlayerGoesFirst);
		SoundManager.Get().LoadAndPlay("FX_MulliganCoin03_CoinFlip", this.coinObject);
		this.coinLocation = coinSpawnLocation.position;
		AssetLoader.Get().LoadActor("MulliganResultText", new AssetLoader.GameObjectCallback(this.CoinTossTextCallback), null, false);
		yield return new WaitForSeconds(2f);
		if (!this.friendlyPlayerGoesFirst)
		{
			GameObject bonusCard = this.m_startingCards[this.m_bonusCardIndex].gameObject;
			iTween.MoveTo(bonusCard, iTween.Hash(new object[]
			{
				"path",
				new Vector3[]
				{
					bonusCard.transform.position,
					new Vector3(bonusCard.transform.position.x, bonusCard.transform.position.y + 3.6f, bonusCard.transform.position.z),
					new Vector3(leftSideOfZone + xOffset, this.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos)
				},
				"time",
				1.5f,
				"easetype",
				iTween.EaseType.easeInSineOutExpo
			}));
			if (UniversalInputManager.UsePhoneUI)
			{
				iTween.ScaleTo(bonusCard, new Vector3(0.9f, 1.1f, 0.9f), 1.5f);
			}
			else
			{
				iTween.ScaleTo(bonusCard, new Vector3(1.1f, 1.1f, 1.1f), 1.5f);
			}
			iTween.RotateTo(bonusCard, iTween.Hash(new object[]
			{
				"rotation",
				new Vector3(0f, 0f, 0f),
				"time",
				1.5f,
				"delay",
				0.1875f
			}));
			yield return new WaitForSeconds(0.04f);
			SoundManager.Get().LoadAndPlay("FX_GameStart20_CardDealSingle", bonusCard);
		}
		else if (this.m_startingOppCards.Count > 0)
		{
			this.m_startingOppCards[this.m_bonusCardIndex].SetDoNotSort(false);
			this.opposingSideHandZone.UpdateLayout(-1, true, 4);
		}
		yield return new WaitForSeconds(1.75f);
		while (GameState.Get().IsBusy())
		{
			yield return null;
		}
		if (this.friendlyPlayerGoesFirst)
		{
			xOffset = 0f;
			for (int j = this.m_startingCards.Count - 1; j >= 0; j--)
			{
				GameObject topCard2 = this.m_startingCards[j].gameObject;
				iTween.Stop(topCard2);
				iTween.MoveTo(topCard2, iTween.Hash(new object[]
				{
					"position",
					new Vector3(rightSideOfZone - spaceForEachCard - xOffset + spaceForEachCard / 2f, this.friendlySideHandZone.transform.position.y + cardHeightOffset, cardZpos),
					"time",
					0.93333334f,
					"easetype",
					iTween.EaseType.easeInOutCubic
				}));
				xOffset += spaceForEachCard;
			}
		}
		yield return new WaitForSeconds(0.6f);
		if (this.skipCardChoosing)
		{
			if (GameState.Get().IsMulliganPhase())
			{
				if (GameState.Get().IsFriendlySidePlayerTurn())
				{
					TurnStartManager.Get().BeginListeningForTurnEvents();
				}
				base.StartCoroutine(this.WaitForOpponentToFinishMulligan());
			}
			else
			{
				yield return new WaitForSeconds(2f);
				this.EndMulligan();
			}
			yield break;
		}
		foreach (Card startCard in this.m_startingCards)
		{
			startCard.GetActor().TurnOnCollider();
		}
		this.mulliganChooseBanner = (GameObject)Object.Instantiate(this.mulliganChooseBannerPrefab, Board.Get().FindBone("ChoiceBanner").position, new Quaternion(0f, 0f, 0f, 0f));
		this.mulliganChooseBanner.GetComponent<Banner>().SetText(GameStrings.Get("GAMEPLAY_MULLIGAN_STARTING_HAND"), GameStrings.Get("GAMEPLAY_MULLIGAN_SUBTITLE"));
		this.m_replaceLabels = new List<MulliganReplaceLabel>();
		for (int k = 0; k < this.m_startingCards.Count; k++)
		{
			InputManager.Get().DoNetworkResponse(this.m_startingCards[k].GetEntity(), true);
			this.m_replaceLabels.Add(null);
		}
		while (this.mulliganButton == null)
		{
			yield return null;
		}
		this.mulliganButton.transform.position = new Vector3(this.startingHandZone.transform.position.x, this.friendlySideHandZone.transform.position.y, this.myHeroCardActor.transform.position.z);
		this.mulliganButton.transform.localEulerAngles = new Vector3(90f, 90f, 90f);
		this.mulliganButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMulliganButtonReleased));
		base.StartCoroutine(this.WaitAFrameBeforeSendingEventToMulliganButton());
		if (!GameMgr.Get().IsSpectator() && !Options.Get().GetBool(Option.HAS_SEEN_MULLIGAN, false) && UserAttentionManager.CanShowAttentionGrabber("MulliganManager.DealStartingCards:" + Option.HAS_SEEN_MULLIGAN))
		{
			this.innkeeperMulliganDialog = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_MULLIGAN_13"), "VO_INNKEEPER_MULLIGAN_13", 0f, null);
			Options.Get().SetBool(Option.HAS_SEEN_MULLIGAN, true);
			this.mulliganButton.GetComponent<Collider>().enabled = false;
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_MulliganSoft);
		this.m_waitingForUserInput = true;
		while (this.innkeeperMulliganDialog != null)
		{
			yield return null;
		}
		this.mulliganButton.GetComponent<Collider>().enabled = true;
		if (Cheats.Get().QuickGameSkipMulligan())
		{
			this.BeginDealNewCards();
		}
		yield break;
	}

	// Token: 0x06004398 RID: 17304 RVA: 0x001437F8 File Offset: 0x001419F8
	private IEnumerator WaitAFrameBeforeSendingEventToMulliganButton()
	{
		yield return null;
		this.mulliganButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Birth");
		yield break;
	}

	// Token: 0x06004399 RID: 17305 RVA: 0x00143814 File Offset: 0x00141A14
	private void BeginMulliganCountdown(float endTimeStamp)
	{
		if (!this.m_waitingForUserInput)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.mulliganTimerPrefab);
		this.m_mulliganTimer = gameObject.GetComponent<MulliganTimer>();
		if (this.m_mulliganTimer == null)
		{
			Object.Destroy(gameObject);
		}
		if (!this.m_waitingForUserInput)
		{
			this.DestroyMulliganTimer();
		}
		this.m_mulliganTimer.SetEndTime(endTimeStamp);
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x00143879 File Offset: 0x00141A79
	public NormalButton GetMulliganButton()
	{
		return this.mulliganButton;
	}

	// Token: 0x0600439B RID: 17307 RVA: 0x00143884 File Offset: 0x00141A84
	private void CoinTossTextCallback(string actorName, GameObject actorObject, object callbackData)
	{
		this.coinTossText = actorObject;
		RenderUtils.SetAlpha(actorObject, 1f);
		actorObject.transform.position = this.coinLocation + new Vector3(0f, 0f, -1f);
		actorObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		UberText componentInChildren = actorObject.transform.GetComponentInChildren<UberText>();
		string text;
		if (this.friendlyPlayerGoesFirst)
		{
			text = GameStrings.Get("GAMEPLAY_COIN_TOSS_WON");
		}
		else
		{
			text = GameStrings.Get("GAMEPLAY_COIN_TOSS_LOST");
		}
		componentInChildren.Text = text;
		GameState.Get().GetGameEntity().NotifyOfCoinFlipResult();
		base.StartCoroutine(this.AnimateCoinTossText());
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x00143944 File Offset: 0x00141B44
	private IEnumerator AnimateCoinTossText()
	{
		yield return new WaitForSeconds(1.8f);
		if (this.coinTossText == null)
		{
			yield break;
		}
		iTween.FadeTo(this.coinTossText, 1f, 0.25f);
		iTween.MoveTo(this.coinTossText, this.coinTossText.transform.position + new Vector3(0f, 0.5f, 0f), 2f);
		yield return new WaitForSeconds(1.9f);
		while (GameState.Get().IsBusy())
		{
			yield return null;
		}
		if (this.coinTossText == null)
		{
			yield break;
		}
		iTween.FadeTo(this.coinTossText, 0f, 1f);
		yield return new WaitForSeconds(0.1f);
		Object.Destroy(this.coinTossText);
		yield break;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x00143960 File Offset: 0x00141B60
	private MulliganReplaceLabel CreateNewUILabelAtCardPosition(MulliganReplaceLabel prefab, int cardPosition)
	{
		MulliganReplaceLabel mulliganReplaceLabel = Object.Instantiate<MulliganReplaceLabel>(prefab);
		if (UniversalInputManager.UsePhoneUI)
		{
			mulliganReplaceLabel.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
			mulliganReplaceLabel.transform.position = new Vector3(this.m_startingCards[cardPosition].transform.position.x, this.m_startingCards[cardPosition].transform.position.y + 0.3f, this.m_startingCards[cardPosition].transform.position.z - 1.1f);
		}
		else
		{
			mulliganReplaceLabel.transform.position = new Vector3(this.m_startingCards[cardPosition].transform.position.x, this.m_startingCards[cardPosition].transform.position.y + 0.3f, this.m_startingCards[cardPosition].transform.position.z - this.startingHandZone.GetComponent<Collider>().bounds.size.z / 2.6f);
		}
		return mulliganReplaceLabel;
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x00143ABC File Offset: 0x00141CBC
	public void SetAllMulliganCardsToHold()
	{
		List<Card> cards = this.friendlySideHandZone.GetCards();
		foreach (Card card in cards)
		{
			InputManager.Get().DoNetworkResponse(card.GetEntity(), true);
		}
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x00143B28 File Offset: 0x00141D28
	private void ToggleHoldState(int startingCardsIndex)
	{
		if (startingCardsIndex >= this.m_startingCards.Count)
		{
			return;
		}
		if (!InputManager.Get().DoNetworkResponse(this.m_startingCards[startingCardsIndex].GetEntity(), true))
		{
			return;
		}
		this.m_handCardsMarkedForReplace[startingCardsIndex] = !this.m_handCardsMarkedForReplace[startingCardsIndex];
		if (!this.m_handCardsMarkedForReplace[startingCardsIndex])
		{
			SoundManager.Get().LoadAndPlay("GM_ChatWarning");
			if (this.m_xLabels[startingCardsIndex] != null)
			{
				Object.Destroy(this.m_xLabels[startingCardsIndex]);
			}
			Object.Destroy(this.m_replaceLabels[startingCardsIndex].gameObject);
		}
		else
		{
			SoundManager.Get().LoadAndPlay("HeroDropItem1");
			if (this.m_xLabels[startingCardsIndex] != null)
			{
				Object.Destroy(this.m_xLabels[startingCardsIndex]);
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.mulliganXlabelPrefab);
			gameObject.transform.position = this.m_startingCards[startingCardsIndex].transform.position;
			gameObject.transform.rotation = this.m_startingCards[startingCardsIndex].transform.rotation;
			this.m_xLabels[startingCardsIndex] = gameObject;
			this.m_replaceLabels[startingCardsIndex] = this.CreateNewUILabelAtCardPosition(this.mulliganReplaceLabelPrefab, startingCardsIndex);
		}
	}

	// Token: 0x060043A0 RID: 17312 RVA: 0x00143C74 File Offset: 0x00141E74
	private void DestroyXobjects()
	{
		if (this.m_xLabels == null)
		{
			return;
		}
		for (int i = 0; i < this.m_xLabels.Length; i++)
		{
			Object.Destroy(this.m_xLabels[i]);
		}
		this.m_xLabels = null;
	}

	// Token: 0x060043A1 RID: 17313 RVA: 0x00143CBA File Offset: 0x00141EBA
	private void DestroyChooseBanner()
	{
		if (this.mulliganChooseBanner == null)
		{
			return;
		}
		Object.Destroy(this.mulliganChooseBanner);
	}

	// Token: 0x060043A2 RID: 17314 RVA: 0x00143CDC File Offset: 0x00141EDC
	private void DestroyMulliganTimer()
	{
		if (this.m_mulliganTimer == null)
		{
			return;
		}
		this.m_mulliganTimer.SelfDestruct();
		this.m_mulliganTimer = null;
	}

	// Token: 0x060043A3 RID: 17315 RVA: 0x00143D10 File Offset: 0x00141F10
	public void ToggleHoldState(Card toggleCard)
	{
		for (int i = 0; i < this.m_startingCards.Count; i++)
		{
			if (this.m_startingCards[i] == toggleCard)
			{
				this.ToggleHoldState(i);
				return;
			}
		}
	}

	// Token: 0x060043A4 RID: 17316 RVA: 0x00143D58 File Offset: 0x00141F58
	public void ServerHasDealtReplacementCards(bool isFriendlySide)
	{
		if (isFriendlySide)
		{
			this.friendlyPlayerHasReplacementCards = true;
			if (GameState.Get().IsFriendlySidePlayerTurn())
			{
				TurnStartManager.Get().BeginListeningForTurnEvents();
			}
		}
		else
		{
			this.opponentPlayerHasReplacementCards = true;
		}
	}

	// Token: 0x060043A5 RID: 17317 RVA: 0x00143D8C File Offset: 0x00141F8C
	public void AutomaticContinueMulligan()
	{
		if (this.mulliganButton != null)
		{
			this.mulliganButton.SetEnabled(false);
		}
		this.DestroyMulliganTimer();
		this.BeginDealNewCards();
	}

	// Token: 0x060043A6 RID: 17318 RVA: 0x00143DB8 File Offset: 0x00141FB8
	private void OnMulliganButtonReleased(UIEvent e)
	{
		if (GameMgr.Get().IsSpectator())
		{
			return;
		}
		NormalButton normalButton = (NormalButton)e.GetElement();
		normalButton.SetEnabled(false);
		this.BeginDealNewCards();
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x00143DEE File Offset: 0x00141FEE
	private void BeginDealNewCards()
	{
		base.StartCoroutine("RemoveOldCardsAnimation");
	}

	// Token: 0x060043A8 RID: 17320 RVA: 0x00143DFC File Offset: 0x00141FFC
	private IEnumerator RemoveOldCardsAnimation()
	{
		this.m_waitingForUserInput = false;
		this.DestroyMulliganTimer();
		SoundManager.Get().LoadAndPlay("FX_GameStart28_CardDismissWoosh2_v2");
		Vector3 mulliganedCardsPosition = Board.Get().FindBone("MulliganedCardsPosition").position;
		this.DestroyXobjects();
		this.DestroyChooseBanner();
		if (!UniversalInputManager.UsePhoneUI)
		{
			Gameplay.Get().RemoveClassNames();
		}
		foreach (Card card in this.m_startingCards)
		{
			card.GetActor().SetActorState(ActorStateType.CARD_IDLE);
			card.GetActor().ToggleForceIdle(true);
			card.GetActor().TurnOffCollider();
		}
		base.StartCoroutine(this.RemoveUIButtons());
		float TO_DECK_ANIMATION_TIME = 1.5f;
		for (int i = 0; i < this.m_startingCards.Count; i++)
		{
			if (this.m_handCardsMarkedForReplace[i])
			{
				GameObject cardObject = this.m_startingCards[i].gameObject;
				iTween.MoveTo(cardObject, iTween.Hash(new object[]
				{
					"path",
					new Vector3[]
					{
						cardObject.transform.position,
						new Vector3(cardObject.transform.position.x + 2f, cardObject.transform.position.y - 1.7f, cardObject.transform.position.z),
						new Vector3(mulliganedCardsPosition.x, mulliganedCardsPosition.y, mulliganedCardsPosition.z),
						this.friendlySideDeck.transform.position
					},
					"time",
					TO_DECK_ANIMATION_TIME,
					"easetype",
					iTween.EaseType.easeOutCubic
				}));
				Animation cardAnim = cardObject.GetComponent<Animation>();
				if (cardAnim == null)
				{
					cardAnim = cardObject.AddComponent<Animation>();
				}
				cardAnim.AddClip(this.cardAnimatesFromBoardToDeck, "putCardBack");
				cardAnim.Play("putCardBack");
				yield return new WaitForSeconds(0.5f);
			}
		}
		InputManager.Get().DoEndTurnButton();
		while (!this.friendlyPlayerHasReplacementCards)
		{
			yield return null;
		}
		this.SortHand(this.friendlySideHandZone);
		List<Card> handZoneCards = this.friendlySideHandZone.GetCards();
		foreach (Card card2 in handZoneCards)
		{
			if (!this.IsCoinCard(card2))
			{
				card2.GetActor().SetActorState(ActorStateType.CARD_IDLE);
				card2.GetActor().ToggleForceIdle(true);
			}
		}
		float zoneWidth = this.startingHandZone.GetComponent<Collider>().bounds.size.x;
		if (UniversalInputManager.UsePhoneUI)
		{
			zoneWidth *= 0.55f;
		}
		float spaceForEachCard = zoneWidth / (float)this.m_startingCards.Count;
		float leftSideOfZone = this.startingHandZone.transform.position.x - zoneWidth / 2f;
		float xOffset = 0f;
		float cardHeightOffset = 0f;
		if (UniversalInputManager.UsePhoneUI)
		{
			cardHeightOffset = 7f;
		}
		float cardZpos = this.startingHandZone.transform.position.z - 0.3f;
		if (UniversalInputManager.UsePhoneUI)
		{
			cardZpos = this.startingHandZone.transform.position.z - 0.2f;
		}
		for (int j = 0; j < this.m_startingCards.Count; j++)
		{
			if (this.m_handCardsMarkedForReplace[j])
			{
				GameObject topCard = handZoneCards[j].gameObject;
				iTween.Stop(topCard);
				iTween.MoveTo(topCard, iTween.Hash(new object[]
				{
					"position",
					new Vector3(leftSideOfZone + spaceForEachCard + xOffset - spaceForEachCard / 2f, this.friendlySideHandZone.GetComponent<Collider>().bounds.center.y, this.startingHandZone.transform.position.z),
					"time",
					3f
				}));
				Vector3[] drawPath = new Vector3[]
				{
					topCard.transform.position,
					new Vector3(mulliganedCardsPosition.x, mulliganedCardsPosition.y, mulliganedCardsPosition.z),
					default(Vector3),
					new Vector3(leftSideOfZone + spaceForEachCard + xOffset - spaceForEachCard / 2f, this.friendlySideHandZone.GetComponent<Collider>().bounds.center.y + cardHeightOffset, cardZpos)
				};
				drawPath[2] = new Vector3(drawPath[3].x + 2f, drawPath[3].y - 1.7f, drawPath[3].z);
				iTween.MoveTo(topCard, iTween.Hash(new object[]
				{
					"path",
					drawPath,
					"time",
					TO_DECK_ANIMATION_TIME,
					"easetype",
					iTween.EaseType.easeInCubic
				}));
				if (UniversalInputManager.UsePhoneUI)
				{
					iTween.ScaleTo(topCard, new Vector3(0.9f, 1.1f, 0.9f), 1.5f);
				}
				else
				{
					iTween.ScaleTo(topCard, new Vector3(1.1f, 1.1f, 1.1f), 1.5f);
				}
				Animation cardAnim2 = topCard.GetComponent<Animation>();
				if (cardAnim2 == null)
				{
					cardAnim2 = topCard.AddComponent<Animation>();
				}
				string animName = "putCardBack";
				cardAnim2.AddClip(this.cardAnimatesFromBoardToDeck, animName);
				cardAnim2[animName].normalizedTime = 1f;
				cardAnim2[animName].speed = -1f;
				cardAnim2.Play(animName);
				yield return new WaitForSeconds(0.5f);
				if (topCard.GetComponent<AudioSource>() == null)
				{
					topCard.AddComponent<AudioSource>();
				}
				SoundManager.Get().LoadAndPlay("FX_GameStart30_CardReplaceSingle", topCard);
			}
			xOffset += spaceForEachCard;
		}
		yield return new WaitForSeconds(1f);
		this.ShuffleDeck();
		yield return new WaitForSeconds(1.5f);
		if (this.opponentPlayerHasReplacementCards)
		{
			this.EndMulligan();
		}
		else
		{
			base.StartCoroutine(this.WaitForOpponentToFinishMulligan());
		}
		yield break;
	}

	// Token: 0x060043A9 RID: 17321 RVA: 0x00143E18 File Offset: 0x00142018
	private IEnumerator WaitForOpponentToFinishMulligan()
	{
		this.DestroyChooseBanner();
		Vector3 mulliganBannerPosition = Board.Get().FindBone("ChoiceBanner").position;
		Vector3 position;
		Vector3 endScale;
		if (UniversalInputManager.UsePhoneUI)
		{
			position = new Vector3(mulliganBannerPosition.x, this.friendlySideHandZone.transform.position.y + 1f, this.myHeroCardActor.transform.position.z + 6.8f);
			endScale = new Vector3(2.5f, 2.5f, 2.5f);
		}
		else
		{
			position = new Vector3(mulliganBannerPosition.x, this.friendlySideHandZone.transform.position.y, this.myHeroCardActor.transform.position.z + 0.4f);
			endScale = new Vector3(1.4f, 1.4f, 1.4f);
		}
		this.mulliganChooseBanner = (GameObject)Object.Instantiate(this.mulliganChooseBannerPrefab, position, new Quaternion(0f, 0f, 0f, 0f));
		this.mulliganChooseBanner.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		iTween.ScaleTo(this.mulliganChooseBanner, endScale, 0.4f);
		this.mulliganChooseBanner.GetComponent<Banner>().SetText(GameStrings.Get("GAMEPLAY_MULLIGAN_WAITING"));
		this.mulliganChooseBanner.GetComponent<Banner>().MoveGlowForBottomPlacement();
		while (!this.opponentPlayerHasReplacementCards && !GameState.Get().IsGameOver())
		{
			yield return null;
		}
		this.EndMulligan();
		yield break;
	}

	// Token: 0x060043AA RID: 17322 RVA: 0x00143E34 File Offset: 0x00142034
	private IEnumerator RemoveUIButtons()
	{
		if (this.mulliganButton != null)
		{
			this.mulliganButton.m_button.GetComponent<PlayMakerFSM>().SendEvent("Death");
		}
		if (this.m_replaceLabels != null)
		{
			for (int i = 0; i < this.m_replaceLabels.Count; i++)
			{
				if (this.m_replaceLabels[i] != null)
				{
					iTween.RotateTo(this.m_replaceLabels[i].gameObject, iTween.Hash(new object[]
					{
						"rotation",
						new Vector3(0f, 0f, 0f),
						"time",
						0.5f,
						"easetype",
						iTween.EaseType.easeInExpo
					}));
					iTween.ScaleTo(this.m_replaceLabels[i].gameObject, iTween.Hash(new object[]
					{
						"scale",
						new Vector3(0.001f, 0.001f, 0.001f),
						"time",
						0.5f,
						"easetype",
						iTween.EaseType.easeInExpo,
						"oncomplete",
						"DestroyButton",
						"oncompletetarget",
						base.gameObject,
						"oncompleteparams",
						this.m_replaceLabels[i]
					}));
					yield return new WaitForSeconds(0.05f);
				}
			}
		}
		yield return new WaitForSeconds(3.5f);
		if (this.mulliganButton != null)
		{
			Object.Destroy(this.mulliganButton.gameObject);
		}
		yield break;
	}

	// Token: 0x060043AB RID: 17323 RVA: 0x00143E4F File Offset: 0x0014204F
	private void DestroyButton(Object buttonToDestroy)
	{
		Object.Destroy(buttonToDestroy);
	}

	// Token: 0x060043AC RID: 17324 RVA: 0x00143E58 File Offset: 0x00142058
	private void HandleGameOverDuringMulligan()
	{
		base.StopCoroutine("WaitForBoardThenLoadButton");
		base.StopCoroutine("WaitForHeroesAndStartAnimations");
		base.StopCoroutine("DealStartingCards");
		base.StopCoroutine("RemoveOldCardsAnimation");
		base.StopCoroutine("PlayStartingTaunts");
		this.m_waitingForUserInput = false;
		this.DestroyXobjects();
		this.DestroyChooseBanner();
		this.DestroyMulliganTimer();
		if (this.coinObject != null)
		{
			Object.Destroy(this.coinObject);
		}
		if (this.versusTextObject != null)
		{
			Object.Destroy(this.versusTextObject);
		}
		if (this.versusVo != null)
		{
			SoundManager.Get().Destroy(this.versusVo);
		}
		if (this.coinTossText != null)
		{
			Object.Destroy(this.coinTossText);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			Gameplay.Get().RemoveNameBanners();
		}
		else
		{
			Gameplay.Get().RemoveClassNames();
		}
		base.StartCoroutine(this.RemoveUIButtons());
		if (this.mulliganButton != null)
		{
			this.mulliganButton.SetEnabled(false);
		}
		this.DestoryHeroSkinSocketInEffects();
		if (this.myheroLabel != null)
		{
			this.myheroLabel.FadeOut();
		}
		if (this.hisheroLabel != null)
		{
			this.hisheroLabel.FadeOut();
		}
		if (this.friendlySideHandZone != null)
		{
			foreach (Card card in this.friendlySideHandZone.GetCards())
			{
				Actor actor = card.GetActor();
				actor.SetActorState(ActorStateType.CARD_IDLE);
				actor.ToggleForceIdle(true);
			}
			if (!this.friendlyPlayerGoesFirst)
			{
				Card coinCardFromFriendlyHand = this.GetCoinCardFromFriendlyHand();
				coinCardFromFriendlyHand.SetDoNotSort(false);
				coinCardFromFriendlyHand.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
				this.PutCoinCardInSpawnPosition(coinCardFromFriendlyHand);
				Actor actor2 = coinCardFromFriendlyHand.GetActor();
				actor2.Show();
			}
			this.friendlySideHandZone.ForceStandInUpdate();
			this.friendlySideHandZone.SetDoNotUpdateLayout(false);
			this.friendlySideHandZone.UpdateLayout();
		}
		Board board = Board.Get();
		if (board != null)
		{
			board.RaiseTheLightsQuickly();
		}
		Animation component = this.myHeroCardActor.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			component.Stop();
		}
		Animation component2 = this.hisHeroCardActor.gameObject.GetComponent<Animation>();
		if (component2 != null)
		{
			component2.Stop();
		}
		this.myHeroCardActor.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).transform.position;
		this.hisHeroCardActor.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position;
	}

	// Token: 0x060043AD RID: 17325 RVA: 0x00144134 File Offset: 0x00142334
	public void EndMulligan()
	{
		this.m_waitingForUserInput = false;
		if (this.m_replaceLabels != null)
		{
			for (int i = 0; i < this.m_replaceLabels.Count; i++)
			{
				Object.Destroy(this.m_replaceLabels[i]);
			}
		}
		if (this.mulliganButton != null)
		{
			Object.Destroy(this.mulliganButton.gameObject);
		}
		this.DestroyXobjects();
		this.DestroyChooseBanner();
		if (this.versusTextObject != null)
		{
			Object.Destroy(this.versusTextObject);
		}
		if (this.versusVo != null)
		{
			SoundManager.Get().Destroy(this.versusVo);
		}
		if (this.coinTossText != null)
		{
			Object.Destroy(this.coinTossText);
		}
		if (this.hisheroLabel != null)
		{
			this.hisheroLabel.FadeOut();
		}
		if (this.myheroLabel != null)
		{
			this.myheroLabel.FadeOut();
		}
		this.DestoryHeroSkinSocketInEffects();
		this.myHeroCardActor.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.hisHeroCardActor.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (GameState.Get().IsGameOver())
		{
			return;
		}
		this.myHeroCardActor.GetHealthObject().Show();
		this.hisHeroCardActor.GetHealthObject().Show();
		this.friendlySideHandZone.ForceStandInUpdate();
		this.friendlySideHandZone.SetDoNotUpdateLayout(false);
		this.friendlySideHandZone.UpdateLayout();
		if (this.m_startingOppCards != null && this.m_startingOppCards.Count > 0)
		{
			this.m_startingOppCards[this.m_startingOppCards.Count - 1].SetDoNotSort(false);
		}
		this.opposingSideHandZone.SetDoNotUpdateLayout(false);
		this.opposingSideHandZone.UpdateLayout();
		this.friendlySideDeck.SetSuppressEmotes(false);
		this.opposingSideDeck.SetSuppressEmotes(false);
		Board.Get().SplitSurface();
		if (UniversalInputManager.UsePhoneUI)
		{
			Gameplay.Get().RemoveNameBanners();
			Gameplay.Get().AddGamePlayNameBannerPhone();
		}
		if (this.m_MyCustomSocketInSpell != null)
		{
			Object.Destroy(this.m_MyCustomSocketInSpell);
		}
		if (this.m_HisCustomSocketInSpell != null)
		{
			Object.Destroy(this.m_HisCustomSocketInSpell);
		}
		base.StartCoroutine(this.EndMulliganWithTiming());
	}

	// Token: 0x060043AE RID: 17326 RVA: 0x001443BC File Offset: 0x001425BC
	private IEnumerator EndMulliganWithTiming()
	{
		if (this.ShouldHandleCoinCard())
		{
			yield return base.StartCoroutine(this.HandleCoinCard());
		}
		else
		{
			Object.Destroy(this.coinObject);
		}
		this.myHeroCardActor.TurnOnCollider();
		this.hisHeroCardActor.TurnOnCollider();
		this.FadeOutMulliganMusicAndStartGameplayMusic();
		foreach (Card card in this.friendlySideHandZone.GetCards())
		{
			card.GetActor().TurnOnCollider();
			card.GetActor().ToggleForceIdle(false);
		}
		if (!this.friendlyPlayerHasReplacementCards)
		{
			base.StartCoroutine(this.EnableHandCollidersAfterCardsAreDealt());
		}
		Collider dragPlane = Board.Get().FindCollider("DragPlane");
		dragPlane.enabled = true;
		this.mulliganActive = false;
		Board.Get().RaiseTheLights();
		this.FadeHeroPowerIn(GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard());
		this.FadeHeroPowerIn(GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard());
		InputManager.Get().OnMulliganEnded();
		EndTurnButton.Get().OnMulliganEnded();
		GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
		base.StartCoroutine(this.WaitForBoardAnimToCompleteThenStartTurn());
		yield break;
	}

	// Token: 0x060043AF RID: 17327 RVA: 0x001443D8 File Offset: 0x001425D8
	private IEnumerator HandleCoinCard()
	{
		if (!this.friendlyPlayerGoesFirst)
		{
			if (this.coinObject.activeSelf)
			{
				yield return new WaitForSeconds(0.5f);
				PlayMakerFSM coinFSM = this.coinObject.GetComponentInChildren<PlayMakerFSM>();
				coinFSM.SendEvent("Birth");
				yield return new WaitForSeconds(0.1f);
			}
			if (!GameMgr.Get().IsSpectator() && !Options.Get().GetBool(Option.HAS_SEEN_THE_COIN, false) && UserAttentionManager.CanShowAttentionGrabber("MulliganManager.HandleCoinCard:" + Option.HAS_SEEN_THE_COIN))
			{
				NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, new Vector3(155.3f, NotificationManager.DEPTH, 34.5f), GameStrings.Get("VO_INNKEEPER_COIN_INTRO"), "VO_INNKEEPER_COIN_INTRO", 0f, null);
				Options.Get().SetBool(Option.HAS_SEEN_THE_COIN, true);
			}
			Card coinCard = this.GetCoinCardFromFriendlyHand();
			this.PutCoinCardInSpawnPosition(coinCard);
			coinCard.ActivateActorSpell(SpellType.SUMMON_IN, new Spell.FinishedCallback(this.CoinCardSummonFinishedCallback));
			yield return new WaitForSeconds(1f);
		}
		else
		{
			Object.Destroy(this.coinObject);
			this.m_startingOppCards[this.m_coinCardIndex].SetDoNotSort(false);
			this.opposingSideHandZone.UpdateLayout();
		}
		yield break;
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x001443F4 File Offset: 0x001425F4
	private bool IsCoinCard(Card card)
	{
		Entity entity = card.GetEntity();
		string cardId = entity.GetCardId();
		return cardId == "GAME_005";
	}

	// Token: 0x060043B1 RID: 17329 RVA: 0x0014441C File Offset: 0x0014261C
	private Card GetCoinCardFromFriendlyHand()
	{
		List<Card> cards = this.friendlySideHandZone.GetCards();
		return cards[cards.Count - 1];
	}

	// Token: 0x060043B2 RID: 17330 RVA: 0x00144444 File Offset: 0x00142644
	private void PutCoinCardInSpawnPosition(Card coinCard)
	{
		coinCard.transform.position = Board.Get().FindBone("MulliganCoinCardSpawnPosition").position;
		coinCard.transform.localScale = Board.Get().FindBone("MulliganCoinCardSpawnPosition").localScale;
	}

	// Token: 0x060043B3 RID: 17331 RVA: 0x00144490 File Offset: 0x00142690
	private bool ShouldHandleCoinCard()
	{
		return GameState.Get().IsMulliganPhase() && GameState.Get().GetGameEntity().ShouldHandleCoin();
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x001444C8 File Offset: 0x001426C8
	private void CoinCardSummonFinishedCallback(Spell spell, object userData)
	{
		Card card = SceneUtils.FindComponentInParents<Card>(spell);
		card.RefreshActor();
		card.UpdateActorComponents();
		card.SetDoNotSort(false);
		Object.Destroy(this.coinObject);
		card.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
		this.friendlySideHandZone.UpdateLayout(-1, true);
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x00144510 File Offset: 0x00142710
	private IEnumerator EnableHandCollidersAfterCardsAreDealt()
	{
		while (!this.friendlyPlayerHasReplacementCards)
		{
			yield return null;
		}
		foreach (Card card in this.friendlySideHandZone.GetCards())
		{
			card.GetActor().TurnOnCollider();
		}
		yield break;
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x0014452B File Offset: 0x0014272B
	public void SkipCardChoosing()
	{
		this.skipCardChoosing = true;
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x00144534 File Offset: 0x00142734
	public void SkipMulliganForDev()
	{
		base.StopCoroutine("WaitForBoardThenLoadButton");
		base.StopCoroutine("WaitForHeroesAndStartAnimations");
		base.StopCoroutine("DealStartingCards");
		this.EndMulligan();
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x00144560 File Offset: 0x00142760
	private IEnumerator SkipMulliganForResume()
	{
		this.introComplete = true;
		SoundDucker ducker = null;
		if (!GameMgr.Get().IsSpectator())
		{
			ducker = base.gameObject.AddComponent<SoundDucker>();
			ducker.m_DuckedCategoryDefs = new List<SoundDuckedCategoryDef>();
			foreach (object obj in Enum.GetValues(typeof(SoundCategory)))
			{
				SoundCategory soundCategory = (SoundCategory)((int)obj);
				if (soundCategory != SoundCategory.AMBIENCE)
				{
					if (soundCategory != SoundCategory.MUSIC)
					{
						SoundDuckedCategoryDef categoryDef = new SoundDuckedCategoryDef();
						categoryDef.m_Category = soundCategory;
						categoryDef.m_Volume = 0f;
						categoryDef.m_RestoreSec = 5f;
						categoryDef.m_BeginSec = 0f;
						ducker.m_DuckedCategoryDefs.Add(categoryDef);
					}
				}
			}
			ducker.StartDucking();
		}
		while (Board.Get() == null)
		{
			yield return null;
		}
		Board.Get().RaiseTheLightsQuickly();
		while (ZoneMgr.Get() == null)
		{
			yield return null;
		}
		this.InitZones();
		Collider dragPlane = Board.Get().FindCollider("DragPlane");
		this.friendlySideHandZone.SetDoNotUpdateLayout(false);
		this.opposingSideHandZone.SetDoNotUpdateLayout(false);
		dragPlane.enabled = false;
		this.friendlySideHandZone.AddInputBlocker();
		this.opposingSideHandZone.AddInputBlocker();
		while (!GameState.Get().IsGameCreated())
		{
			yield return null;
		}
		while (ZoneMgr.Get().HasActiveServerChange())
		{
			yield return null;
		}
		GameState.Get().GetGameEntity().NotifyOfMulliganInitialized();
		SceneMgr.Get().NotifySceneLoaded();
		while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
		{
			yield return null;
		}
		if (ducker != null)
		{
			ducker.StopDucking();
			Object.Destroy(ducker);
		}
		if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
		{
			this.FadeOutMulliganMusicAndStartGameplayMusic();
		}
		dragPlane.enabled = true;
		this.friendlySideHandZone.RemoveInputBlocker();
		this.opposingSideHandZone.RemoveInputBlocker();
		this.friendlySideDeck.SetSuppressEmotes(false);
		this.opposingSideDeck.SetSuppressEmotes(false);
		if (GameState.Get().GetResponseMode() == GameState.ResponseMode.CHOICE)
		{
			GameState.Get().UpdateChoiceHighlights();
		}
		GameMgr.Get().UpdatePresence();
		InputManager.Get().OnMulliganEnded();
		EndTurnButton.Get().OnMulliganEnded();
		GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x0014457B File Offset: 0x0014277B
	public void SkipMulligan()
	{
		base.StartCoroutine(this.SkipMulliganWhenIntroComplete());
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x0014458C File Offset: 0x0014278C
	private IEnumerator SkipMulliganWhenIntroComplete()
	{
		this.m_waitingForUserInput = false;
		while (!this.introComplete)
		{
			yield return null;
		}
		this.myHeroCardActor.TurnOnCollider();
		this.hisHeroCardActor.TurnOnCollider();
		this.FadeOutMulliganMusicAndStartGameplayMusic();
		this.myHeroCardActor.GetHealthObject().Show();
		this.hisHeroCardActor.GetHealthObject().Show();
		Collider dragPlane = Board.Get().FindCollider("DragPlane");
		dragPlane.enabled = true;
		Board.Get().SplitSurface();
		this.FadeHeroPowerIn(GameState.Get().GetFriendlySidePlayer().GetHeroPowerCard());
		this.FadeHeroPowerIn(GameState.Get().GetOpposingSidePlayer().GetHeroPowerCard());
		this.mulliganActive = false;
		this.InitZones();
		this.friendlySideHandZone.SetDoNotUpdateLayout(false);
		this.friendlySideHandZone.UpdateLayout();
		this.opposingSideHandZone.SetDoNotUpdateLayout(false);
		this.opposingSideHandZone.UpdateLayout();
		this.friendlySideDeck.SetSuppressEmotes(false);
		this.opposingSideDeck.SetSuppressEmotes(false);
		InputManager.Get().OnMulliganEnded();
		EndTurnButton.Get().OnMulliganEnded();
		GameState.Get().GetGameEntity().NotifyOfMulliganEnded();
		base.StartCoroutine(this.WaitForBoardAnimToCompleteThenStartTurn());
		yield break;
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x001445A7 File Offset: 0x001427A7
	private void FadeOutMulliganMusicAndStartGameplayMusic()
	{
		GameState.Get().GetGameEntity().StartGameplaySoundtracks();
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x001445B8 File Offset: 0x001427B8
	private IEnumerator WaitForBoardAnimToCompleteThenStartTurn()
	{
		yield return new WaitForSeconds(1.5f);
		GameState.Get().SetMulliganBusy(false);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x001445D4 File Offset: 0x001427D4
	private void ShuffleDeck()
	{
		SoundManager.Get().LoadAndPlay("FX_MulliganCoin09_DeckShuffle", this.friendlySideDeck.gameObject);
		Animation animation = this.friendlySideDeck.gameObject.GetComponent<Animation>();
		if (animation == null)
		{
			animation = this.friendlySideDeck.gameObject.AddComponent<Animation>();
		}
		animation.AddClip(this.shuffleDeck, "shuffleDeckAnim");
		animation.Play("shuffleDeckAnim");
		animation = this.opposingSideDeck.gameObject.GetComponent<Animation>();
		if (animation == null)
		{
			animation = this.opposingSideDeck.gameObject.AddComponent<Animation>();
		}
		animation.AddClip(this.shuffleDeck, "shuffleDeckAnim");
		animation.Play("shuffleDeckAnim");
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x00144694 File Offset: 0x00142894
	private void SlideCard(GameObject topCard)
	{
		iTween.MoveTo(topCard, iTween.Hash(new object[]
		{
			"position",
			new Vector3(topCard.transform.position.x - 0.5f, topCard.transform.position.y, topCard.transform.position.z),
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.linear
		}));
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x00144730 File Offset: 0x00142930
	private IEnumerator SampleAnimFrame(Animation animToUse, string animName, float startSec)
	{
		AnimationState state = animToUse[animName];
		state.enabled = true;
		state.time = startSec;
		animToUse.Play(animName);
		yield return null;
		state.enabled = false;
		yield break;
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x0014476E File Offset: 0x0014296E
	private void SortHand(Zone zone)
	{
		zone.GetCards().Sort(new Comparison<Card>(Zone.CardSortComparison));
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x00144788 File Offset: 0x00142988
	private IEnumerator ShrinkStartingHandBanner(GameObject banner)
	{
		yield return new WaitForSeconds(4f);
		iTween.ScaleTo(banner, new Vector3(0f, 0f, 0f), 0.5f);
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(banner);
		yield break;
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x001447AC File Offset: 0x001429AC
	private void FadeHeroPowerIn(Card heroPowerCard)
	{
		if (heroPowerCard == null)
		{
			return;
		}
		Actor actor = heroPowerCard.GetActor();
		if (actor == null)
		{
			return;
		}
		actor.TurnOnCollider();
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x001447E0 File Offset: 0x001429E0
	private void LoadMyHeroSkinSocketInEffect(CardDef cardDef)
	{
		if (string.IsNullOrEmpty(cardDef.m_SocketInEffectFriendly) && !UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		if (string.IsNullOrEmpty(cardDef.m_SocketInEffectFriendlyPhone) && UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.m_isLoadingMyCustomSocketIn = true;
		string name = cardDef.m_SocketInEffectFriendly;
		if (UniversalInputManager.UsePhoneUI)
		{
			name = cardDef.m_SocketInEffectFriendlyPhone;
		}
		AssetLoader.Get().LoadSpell(name, new AssetLoader.GameObjectCallback(this.OnMyHeroSkinSocketInEffectLoaded), null, false);
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x0014486C File Offset: 0x00142A6C
	private void OnMyHeroSkinSocketInEffectLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("Failed to load My custom hero socket in effect!");
			this.m_isLoadingMyCustomSocketIn = false;
			return;
		}
		go.transform.position = Board.Get().FindBone("CustomSocketIn_Friendly").position;
		Spell component = go.GetComponent<Spell>();
		if (component == null)
		{
			Debug.LogError("Faild to locate Spell on custom socket in effect!");
			this.m_isLoadingMyCustomSocketIn = false;
			return;
		}
		this.m_MyCustomSocketInSpell = component;
		if (this.m_MyCustomSocketInSpell.HasUsableState(SpellStateType.IDLE))
		{
			this.m_MyCustomSocketInSpell.ActivateState(SpellStateType.IDLE);
		}
		else
		{
			this.m_MyCustomSocketInSpell.gameObject.SetActive(false);
		}
		this.m_isLoadingMyCustomSocketIn = false;
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x0014491C File Offset: 0x00142B1C
	private void LoadHisHeroSkinSocketInEffect(CardDef cardDef)
	{
		if (string.IsNullOrEmpty(cardDef.m_SocketInEffectOpponent) && !UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		if (string.IsNullOrEmpty(cardDef.m_SocketInEffectOpponentPhone) && UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		this.m_isLoadingHisCustomSocketIn = true;
		string name = cardDef.m_SocketInEffectOpponent;
		if (UniversalInputManager.UsePhoneUI)
		{
			name = cardDef.m_SocketInEffectOpponentPhone;
		}
		AssetLoader.Get().LoadSpell(name, new AssetLoader.GameObjectCallback(this.OnHisHeroSkinSocketInEffectLoaded), null, false);
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x001449A8 File Offset: 0x00142BA8
	private void OnHisHeroSkinSocketInEffectLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError("Failed to load His custom hero socket in effect!");
			this.m_isLoadingHisCustomSocketIn = false;
			return;
		}
		go.transform.position = Board.Get().FindBone("CustomSocketIn_Opposing").position;
		Spell component = go.GetComponent<Spell>();
		if (component == null)
		{
			Debug.LogError("Faild to locate Spell on custom socket in effect!");
			this.m_isLoadingHisCustomSocketIn = false;
			return;
		}
		this.m_HisCustomSocketInSpell = component;
		if (this.m_HisCustomSocketInSpell.HasUsableState(SpellStateType.IDLE))
		{
			this.m_HisCustomSocketInSpell.ActivateState(SpellStateType.IDLE);
		}
		else
		{
			this.m_HisCustomSocketInSpell.gameObject.SetActive(false);
		}
		this.m_isLoadingHisCustomSocketIn = false;
	}

	// Token: 0x060043C7 RID: 17351 RVA: 0x00144A58 File Offset: 0x00142C58
	private void DestoryHeroSkinSocketInEffects()
	{
		if (this.m_MyCustomSocketInSpell != null)
		{
			Object.Destroy(this.m_MyCustomSocketInSpell.gameObject);
		}
		if (this.m_HisCustomSocketInSpell != null)
		{
			Object.Destroy(this.m_HisCustomSocketInSpell.gameObject);
		}
	}

	// Token: 0x04002ABA RID: 10938
	private const float PHONE_HEIGHT_OFFSET = 7f;

	// Token: 0x04002ABB RID: 10939
	private const float PHONE_CARD_Z_OFFSET = 0.2f;

	// Token: 0x04002ABC RID: 10940
	private const float PHONE_CARD_SCALE = 0.9f;

	// Token: 0x04002ABD RID: 10941
	private const float PHONE_ZONE_SIZE_ADJUST = 0.55f;

	// Token: 0x04002ABE RID: 10942
	private const float ANIMATION_TIME_DEAL_CARD = 1.5f;

	// Token: 0x04002ABF RID: 10943
	private const float DEFAULT_STARTING_TAUNT_DURATION = 2.5f;

	// Token: 0x04002AC0 RID: 10944
	public AnimationClip cardAnimatesFromBoardToDeck;

	// Token: 0x04002AC1 RID: 10945
	public AnimationClip cardAnimatesFromBoardToDeck_iPhone;

	// Token: 0x04002AC2 RID: 10946
	public AnimationClip cardAnimatesFromTableToSky;

	// Token: 0x04002AC3 RID: 10947
	public AnimationClip cardAnimatesFromDeckToBoard;

	// Token: 0x04002AC4 RID: 10948
	public AnimationClip shuffleDeck;

	// Token: 0x04002AC5 RID: 10949
	public AnimationClip myheroAnimatesToPosition;

	// Token: 0x04002AC6 RID: 10950
	public AnimationClip hisheroAnimatesToPosition;

	// Token: 0x04002AC7 RID: 10951
	public AnimationClip myheroAnimatesToPosition_iPhone;

	// Token: 0x04002AC8 RID: 10952
	public AnimationClip hisheroAnimatesToPosition_iPhone;

	// Token: 0x04002AC9 RID: 10953
	public GameObject coinPrefab;

	// Token: 0x04002ACA RID: 10954
	public GameObject weldPrefab;

	// Token: 0x04002ACB RID: 10955
	public GameObject mulliganChooseBannerPrefab;

	// Token: 0x04002ACC RID: 10956
	public GameObject mulliganKeepLabelPrefab;

	// Token: 0x04002ACD RID: 10957
	public MulliganReplaceLabel mulliganReplaceLabelPrefab;

	// Token: 0x04002ACE RID: 10958
	public GameObject mulliganXlabelPrefab;

	// Token: 0x04002ACF RID: 10959
	public GameObject mulliganTimerPrefab;

	// Token: 0x04002AD0 RID: 10960
	public GameObject heroLabelPrefab;

	// Token: 0x04002AD1 RID: 10961
	private static MulliganManager s_instance;

	// Token: 0x04002AD2 RID: 10962
	private bool mulliganActive;

	// Token: 0x04002AD3 RID: 10963
	private MulliganTimer m_mulliganTimer;

	// Token: 0x04002AD4 RID: 10964
	private NormalButton mulliganButton;

	// Token: 0x04002AD5 RID: 10965
	private GameObject myWeldEffect;

	// Token: 0x04002AD6 RID: 10966
	private GameObject hisWeldEffect;

	// Token: 0x04002AD7 RID: 10967
	private GameObject coinObject;

	// Token: 0x04002AD8 RID: 10968
	private GameObject startingHandZone;

	// Token: 0x04002AD9 RID: 10969
	private GameObject coinTossText;

	// Token: 0x04002ADA RID: 10970
	private ZoneHand friendlySideHandZone;

	// Token: 0x04002ADB RID: 10971
	private ZoneHand opposingSideHandZone;

	// Token: 0x04002ADC RID: 10972
	private ZoneDeck friendlySideDeck;

	// Token: 0x04002ADD RID: 10973
	private ZoneDeck opposingSideDeck;

	// Token: 0x04002ADE RID: 10974
	private Actor myHeroCardActor;

	// Token: 0x04002ADF RID: 10975
	private Actor hisHeroCardActor;

	// Token: 0x04002AE0 RID: 10976
	private Actor myHeroPowerCardActor;

	// Token: 0x04002AE1 RID: 10977
	private Actor hisHeroPowerCardActor;

	// Token: 0x04002AE2 RID: 10978
	private bool waitingForVersusText;

	// Token: 0x04002AE3 RID: 10979
	private GameObject versusTextObject;

	// Token: 0x04002AE4 RID: 10980
	private bool waitingForVersusVo;

	// Token: 0x04002AE5 RID: 10981
	private AudioSource versusVo;

	// Token: 0x04002AE6 RID: 10982
	private bool introComplete;

	// Token: 0x04002AE7 RID: 10983
	private bool skipCardChoosing;

	// Token: 0x04002AE8 RID: 10984
	private List<Card> m_startingCards;

	// Token: 0x04002AE9 RID: 10985
	private List<Card> m_startingOppCards;

	// Token: 0x04002AEA RID: 10986
	private int m_coinCardIndex = -1;

	// Token: 0x04002AEB RID: 10987
	private int m_bonusCardIndex = -1;

	// Token: 0x04002AEC RID: 10988
	private GameObject mulliganChooseBanner;

	// Token: 0x04002AED RID: 10989
	private List<MulliganReplaceLabel> m_replaceLabels;

	// Token: 0x04002AEE RID: 10990
	private GameObject[] m_xLabels;

	// Token: 0x04002AEF RID: 10991
	private bool[] m_handCardsMarkedForReplace = new bool[4];

	// Token: 0x04002AF0 RID: 10992
	private Vector3 coinLocation;

	// Token: 0x04002AF1 RID: 10993
	private bool friendlyPlayerGoesFirst;

	// Token: 0x04002AF2 RID: 10994
	private HeroLabel myheroLabel;

	// Token: 0x04002AF3 RID: 10995
	private HeroLabel hisheroLabel;

	// Token: 0x04002AF4 RID: 10996
	private Spell m_MyCustomSocketInSpell;

	// Token: 0x04002AF5 RID: 10997
	private Spell m_HisCustomSocketInSpell;

	// Token: 0x04002AF6 RID: 10998
	private bool m_isLoadingMyCustomSocketIn;

	// Token: 0x04002AF7 RID: 10999
	private bool m_isLoadingHisCustomSocketIn;

	// Token: 0x04002AF8 RID: 11000
	private bool friendlyPlayerHasReplacementCards;

	// Token: 0x04002AF9 RID: 11001
	private bool opponentPlayerHasReplacementCards;

	// Token: 0x04002AFA RID: 11002
	private bool m_waitingForUserInput;

	// Token: 0x04002AFB RID: 11003
	private Notification innkeeperMulliganDialog;

	// Token: 0x04002AFC RID: 11004
	private bool m_resuming;
}
