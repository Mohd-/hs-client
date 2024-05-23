using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007F2 RID: 2034
[CustomEditClass]
public class ArenaTrayDisplay : MonoBehaviour
{
	// Token: 0x06004F04 RID: 20228 RVA: 0x0017705B File Offset: 0x0017525B
	private void Awake()
	{
		ArenaTrayDisplay.s_Instance = this;
	}

	// Token: 0x06004F05 RID: 20229 RVA: 0x00177064 File Offset: 0x00175264
	private void Start()
	{
		if (this.m_WinsUberText == null || this.m_LossesUberText == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: m_WinsUberText or m_LossesUberText is null!");
			return;
		}
		this.m_WinsUberText.Text = GameStrings.Get("GLUE_DRAFT_WINS_LABEL");
		this.m_LossesUberText.Text = GameStrings.Get("GLUE_DRAFT_LOSSES_LABEL");
		if (this.m_BehindTheDoors == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: m_BehindTheDoors is null!");
			return;
		}
		this.m_BehindTheDoors.SetActive(false);
		if (this.m_RewardDoorPlates == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: m_RewardDoorPlates is null!");
			return;
		}
		this.m_RewardDoorPlates.SetActive(false);
		SceneUtils.EnableColliders(this.m_TheKeyMesh, false);
	}

	// Token: 0x06004F06 RID: 20230 RVA: 0x00177124 File Offset: 0x00175324
	private void OnDisable()
	{
	}

	// Token: 0x06004F07 RID: 20231 RVA: 0x00177126 File Offset: 0x00175326
	private void OnDestroy()
	{
	}

	// Token: 0x06004F08 RID: 20232 RVA: 0x00177128 File Offset: 0x00175328
	private void OnEnable()
	{
	}

	// Token: 0x06004F09 RID: 20233 RVA: 0x0017712A File Offset: 0x0017532A
	public static ArenaTrayDisplay Get()
	{
		return ArenaTrayDisplay.s_Instance;
	}

	// Token: 0x06004F0A RID: 20234 RVA: 0x00177131 File Offset: 0x00175331
	public void UpdateTray()
	{
		this.UpdateTray(true);
	}

	// Token: 0x06004F0B RID: 20235 RVA: 0x0017713C File Offset: 0x0017533C
	public void UpdateTray(bool showNewKey)
	{
		this.ShowPlainPaper();
		if (this.m_InstructionText != null)
		{
			this.m_InstructionText.SetActive(false);
		}
		if (this.m_RewardDoorPlates != null && !this.m_RewardDoorPlates.activeSelf)
		{
			this.m_RewardDoorPlates.SetActive(true);
		}
		bool flag = false;
		DraftManager draftManager = DraftManager.Get();
		if (draftManager == null)
		{
			Debug.LogError("ArenaTrayDisplay: DraftManager.Get() == null!");
			return;
		}
		int wins = draftManager.GetWins();
		int losses = draftManager.GetLosses();
		if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && GameMgr.Get().WasArena() && draftManager.GetIsNewKey())
		{
			flag = true;
		}
		this.m_WinCountUberText.Text = wins.ToString();
		if (losses > 0)
		{
			this.m_Xmark1.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			this.m_Xmark1.GetComponent<Renderer>().enabled = false;
		}
		if (losses > 1)
		{
			this.m_Xmark2.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			this.m_Xmark2.GetComponent<Renderer>().enabled = false;
		}
		if (losses > 2)
		{
			this.m_Xmark3.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			this.m_Xmark3.GetComponent<Renderer>().enabled = false;
		}
		this.UpdateXBoxes();
		if (flag && wins > 0 && showNewKey)
		{
			this.UpdateKeyArt(wins - 1);
			base.StartCoroutine(this.AnimateKeyTransition(wins));
		}
		else
		{
			this.UpdateKeyArt(wins);
		}
	}

	// Token: 0x06004F0C RID: 20236 RVA: 0x001772C8 File Offset: 0x001754C8
	public void ShowPlainPaperBackground()
	{
		this.ShowPlainPaper();
		if (this.m_InstructionText != null)
		{
			this.m_InstructionText.SetActive(true);
		}
		if (this.m_RewardDoorPlates != null && this.m_RewardDoorPlates.activeSelf)
		{
			this.m_RewardDoorPlates.SetActive(false);
		}
	}

	// Token: 0x06004F0D RID: 20237 RVA: 0x00177328 File Offset: 0x00175528
	public void ActivateKey()
	{
		SceneUtils.EnableColliders(this.m_TheKeyMesh, true);
		this.m_TheKeySelectionGlow.GetComponent<Renderer>().enabled = true;
		Color color = this.m_TheKeySelectionGlow.GetComponent<Renderer>().sharedMaterial.color;
		color.a = 0f;
		this.m_TheKeySelectionGlow.GetComponent<Renderer>().sharedMaterial.color = color;
		this.m_TheKeySelectionGlow.GetComponent<Renderer>().sharedMaterial.SetFloat("_FxIntensity", 1f);
		iTween.FadeTo(this.m_TheKeySelectionGlow, iTween.Hash(new object[]
		{
			"alpha",
			0.8f,
			"time",
			2f,
			"easetype",
			iTween.EaseType.easeInOutBack
		}));
		Material KeyGlowMat = this.m_TheKeySelectionGlow.GetComponent<Renderer>().material;
		KeyGlowMat.SetFloat("_FxIntensity", 0f);
		Action<object> action = delegate(object amount)
		{
			KeyGlowMat.SetFloat("_FxIntensity", (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			2f,
			"from",
			0f,
			"to",
			1f,
			"easetype",
			iTween.EaseType.easeInOutBack,
			"onupdate",
			action,
			"onupdatetarget",
			this.m_TheKeySelectionGlow
		});
		iTween.ValueTo(this.m_TheKeySelectionGlow, args);
		PegUIElement component = this.m_TheKeyMesh.GetComponent<PegUIElement>();
		if (component == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: PegUIElement missing on the Key!");
			return;
		}
		component.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OpenRewardBox));
		Navigation.Push(new Navigation.NavigateBackHandler(Navigation.BlockBackingOut));
	}

	// Token: 0x06004F0E RID: 20238 RVA: 0x00177510 File Offset: 0x00175710
	public void ShowRewardsOpenAtStart()
	{
		if (this.m_RewardPlaymaker == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: Missing Playmaker FSM!");
			return;
		}
		this.HidePaper();
		if (this.m_InstructionText != null)
		{
			this.m_InstructionText.SetActive(false);
		}
		if (this.m_WinCountUberText != null)
		{
			this.m_WinCountUberText.gameObject.SetActive(false);
		}
		if (this.m_WinsUberText != null)
		{
			this.m_WinsUberText.gameObject.SetActive(false);
		}
		if (this.m_LossesUberText != null)
		{
			this.m_LossesUberText.gameObject.SetActive(false);
		}
		if (this.m_XmarksRoot != null)
		{
			this.m_XmarksRoot.SetActive(false);
		}
		if (this.m_TheKeySelectionGlow != null)
		{
			this.m_TheKeySelectionGlow.SetActive(false);
		}
		this.m_WinsUberText.gameObject.SetActive(false);
		this.m_LossesUberText.gameObject.SetActive(false);
		this.m_TheKeyMesh.gameObject.SetActive(false);
		if (this.m_BehindTheDoors == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: m_BehindTheDoors is null!");
			return;
		}
		this.m_BehindTheDoors.SetActive(true);
		if (DraftManager.Get() == null)
		{
			Debug.LogError("ArenaTrayDisplay: DraftManager.Get() == null!");
			return;
		}
		AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
		{
			this.m_RewardBoxes = go.GetComponent<RewardBoxesDisplay>();
			this.m_RewardBoxes.SetRewards(DraftManager.Get().GetRewards());
			this.m_RewardBoxes.RegisterDoneCallback(new Action(this.OnRewardBoxesDone));
			TransformUtil.AttachAndPreserveLocalTransform(this.m_RewardBoxes.transform, this.m_RewardBoxesBone.transform);
			this.m_RewardBoxes.DebugLogRewards();
			this.m_RewardBoxes.ShowAlreadyOpenedRewards();
		};
		AssetLoader.Get().LoadGameObject("RewardBoxes", callback, null, false);
		this.m_RewardPlaymaker.gameObject.SetActive(true);
		this.m_RewardPlaymaker.SendEvent("Death");
		PegUIElement component = this.m_TheKeyMesh.GetComponent<PegUIElement>();
		if (component == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: PegUIElement missing on the Key!");
			return;
		}
	}

	// Token: 0x06004F0F RID: 20239 RVA: 0x001776D2 File Offset: 0x001758D2
	public void ShowOpenedRewards()
	{
	}

	// Token: 0x06004F10 RID: 20240 RVA: 0x001776D4 File Offset: 0x001758D4
	public void AnimateRewards()
	{
		AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
		{
			this.m_RewardBoxes = go.GetComponent<RewardBoxesDisplay>();
			this.m_RewardBoxes.SetRewards(DraftManager.Get().GetRewards());
			this.m_RewardBoxes.RegisterDoneCallback(new Action(this.OnRewardBoxesDone));
			TransformUtil.AttachAndPreserveLocalTransform(this.m_RewardBoxes.transform, this.m_RewardBoxesBone.transform);
			this.m_RewardBoxes.AnimateRewards();
		};
		AssetLoader.Get().LoadGameObject("RewardBoxes", callback, null, false);
	}

	// Token: 0x06004F11 RID: 20241 RVA: 0x00177704 File Offset: 0x00175904
	public void KeyFXCancel()
	{
		if (this.m_TheKeyIdleEffects)
		{
			PlayMakerFSM componentInChildren = this.m_TheKeyIdleEffects.GetComponentInChildren<PlayMakerFSM>();
			if (componentInChildren)
			{
				componentInChildren.SendEvent("Cancel");
			}
		}
	}

	// Token: 0x06004F12 RID: 20242 RVA: 0x00177744 File Offset: 0x00175944
	private void UpdateKeyArt(int rank)
	{
		if (this.m_TheKeyMesh == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: key mesh missing!");
			return;
		}
		this.ShowRewardPaper();
		ArenaTrayDisplay.ArenaKeyVisualData arenaKeyVisualData = this.m_ArenaKeyVisualData[rank];
		if (arenaKeyVisualData.m_Mesh != null)
		{
			MeshFilter component = this.m_TheKeyMesh.GetComponent<MeshFilter>();
			if (component != null)
			{
				component.mesh = Object.Instantiate<Mesh>(arenaKeyVisualData.m_Mesh);
			}
		}
		if (arenaKeyVisualData.m_Material != null)
		{
			this.m_TheKeyMesh.GetComponent<Renderer>().sharedMaterial = Object.Instantiate<Material>(arenaKeyVisualData.m_Material);
		}
		if (arenaKeyVisualData.m_IdleEffectsPrefabName != string.Empty)
		{
			this.m_isTheKeyIdleEffectsLoading = true;
			AssetLoader.Get().LoadGameObject(arenaKeyVisualData.m_IdleEffectsPrefabName, new AssetLoader.GameObjectCallback(this.OnIdleEffectsLoaded), null, false);
		}
		if (arenaKeyVisualData.m_ParticlePrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(arenaKeyVisualData.m_ParticlePrefab);
			Transform transform = gameObject.transform.FindChild("FX_Motes");
			if (transform != null)
			{
				GameObject gameObject2 = transform.gameObject;
				gameObject2.transform.parent = this.m_TheKeyMesh.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes").Value = gameObject2;
			}
			Transform transform2 = gameObject.transform.FindChild("FX_Motes_glow");
			if (transform2 != null)
			{
				GameObject gameObject3 = transform2.gameObject;
				gameObject3.transform.parent = this.m_TheKeyMesh.transform;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.transform.localRotation = Quaternion.identity;
				this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes_glow").Value = gameObject3;
			}
			Transform transform3 = gameObject.transform.FindChild("FX_Motes_trail");
			if (transform3 != null)
			{
				GameObject gameObject4 = transform3.gameObject;
				gameObject4.transform.parent = this.m_TheKeyMesh.transform;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localRotation = Quaternion.identity;
				this.m_RewardPlaymaker.FsmVariables.GetFsmGameObject("FX_Motes_trail").Value = gameObject4;
			}
		}
		if (this.m_TheKeyGlowPlane != null && arenaKeyVisualData.m_EffectGlowTexture != null)
		{
			this.m_TheKeyGlowPlane.GetComponent<Renderer>().material.mainTexture = arenaKeyVisualData.m_EffectGlowTexture;
		}
		if (arenaKeyVisualData.m_KeyHoleGlowMesh != null)
		{
			MeshFilter component2 = this.m_TheKeyGlowHoleMesh.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				component2.mesh = Object.Instantiate<Mesh>(arenaKeyVisualData.m_KeyHoleGlowMesh);
			}
		}
		if (this.m_TheKeySelectionGlow != null && arenaKeyVisualData.m_SelectionGlowTexture != null)
		{
			this.m_TheKeySelectionGlow.GetComponent<Renderer>().material.mainTexture = arenaKeyVisualData.m_SelectionGlowTexture;
		}
		SceneUtils.SetLayer(this.m_TheKeyMesh.transform.parent.gameObject, GameLayer.Default);
	}

	// Token: 0x06004F13 RID: 20243 RVA: 0x00177A84 File Offset: 0x00175C84
	private void OnIdleEffectsLoaded(string name, GameObject go, object callbackData)
	{
		this.m_isTheKeyIdleEffectsLoading = false;
		if (this.m_TheKeyIdleEffects)
		{
			Object.Destroy(this.m_TheKeyIdleEffects);
		}
		this.m_TheKeyIdleEffects = go;
		go.SetActive(true);
		go.transform.parent = this.m_TheKeyMesh.transform;
		go.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06004F14 RID: 20244 RVA: 0x00177AE8 File Offset: 0x00175CE8
	private IEnumerator AnimateKeyTransition(int rank)
	{
		yield return new WaitForSeconds(this.m_TheKeyTransitionDelay);
		while (this.m_isTheKeyIdleEffectsLoading)
		{
			yield return null;
		}
		int prevRank = rank - 1;
		ArenaTrayDisplay.ArenaKeyVisualData prevKeyData = this.m_ArenaKeyVisualData[prevRank];
		ArenaTrayDisplay.ArenaKeyVisualData keyData = this.m_ArenaKeyVisualData[rank];
		if (this.m_TheKeyOldSelectionGlow != null && prevKeyData.m_EffectGlowTexture != null)
		{
			this.m_TheKeyOldSelectionGlow.GetComponent<Renderer>().material.mainTexture = prevKeyData.m_SelectionGlowTexture;
		}
		this.m_TheKeyOldSelectionGlow.GetComponent<Renderer>().enabled = true;
		Material prevKeyGlowMat = this.m_TheKeyOldSelectionGlow.GetComponent<Renderer>().material;
		prevKeyGlowMat.SetFloat("_FxIntensity", 0f);
		Action<object> prevKeyGlowUpdate = delegate(object amount)
		{
			prevKeyGlowMat.SetFloat("_FxIntensity", (float)amount);
		};
		Hashtable prevKeyGlowArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_TheKeyTransitionFadeInTime,
			"from",
			0f,
			"to",
			1.5f,
			"easetype",
			iTween.EaseType.easeInCubic,
			"onupdate",
			prevKeyGlowUpdate,
			"onupdatetarget",
			this.m_TheKeyOldSelectionGlow
		});
		iTween.ValueTo(this.m_TheKeyOldSelectionGlow, prevKeyGlowArgs);
		if (this.m_TheKeyTransitionSound != string.Empty)
		{
			SoundManager.Get().LoadAndPlay(this.m_TheKeyTransitionSound);
		}
		yield return new WaitForSeconds(this.m_TheKeyTransitionFadeInTime);
		this.m_TheKeyTransitionParticles.Play();
		this.UpdateKeyArt(rank);
		this.m_TheKeyOldSelectionGlow.GetComponent<Renderer>().enabled = false;
		if (this.m_TheKeySelectionGlow != null && keyData.m_EffectGlowTexture != null)
		{
			this.m_TheKeySelectionGlow.GetComponent<Renderer>().material.mainTexture = keyData.m_SelectionGlowTexture;
		}
		this.m_TheKeySelectionGlow.GetComponent<Renderer>().enabled = true;
		prevKeyGlowMat.SetFloat("_FxIntensity", 0f);
		Material KeyGlowMat = this.m_TheKeySelectionGlow.GetComponent<Renderer>().material;
		KeyGlowMat.SetFloat("_FxIntensity", 1.5f);
		Action<object> keyGlowUpdate = delegate(object amount)
		{
			KeyGlowMat.SetFloat("_FxIntensity", (float)amount);
		};
		Hashtable keyGlowArgs = iTween.Hash(new object[]
		{
			"time",
			this.m_TheKeyTransitionFadeOutTime,
			"from",
			1.5f,
			"to",
			0f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"onupdate",
			keyGlowUpdate,
			"onupdatetarget",
			this.m_TheKeySelectionGlow
		});
		iTween.ValueTo(this.m_TheKeySelectionGlow, keyGlowArgs);
		yield return new WaitForSeconds(this.m_TheKeyTransitionFadeOutTime);
		this.m_TheKeySelectionGlow.GetComponent<Renderer>().enabled = false;
		yield break;
	}

	// Token: 0x06004F15 RID: 20245 RVA: 0x00177B14 File Offset: 0x00175D14
	private void UpdateXBoxes()
	{
		if (!DemoMgr.Get().ArenaIs1WinMode())
		{
			return;
		}
		this.m_XmarkBox[0].SetActive(true);
		this.m_XmarkBox[1].SetActive(false);
		this.m_XmarkBox[2].SetActive(false);
	}

	// Token: 0x06004F16 RID: 20246 RVA: 0x00177B67 File Offset: 0x00175D67
	private void OpenRewardBox(UIEvent e)
	{
		this.OpenRewardBox();
	}

	// Token: 0x06004F17 RID: 20247 RVA: 0x00177B70 File Offset: 0x00175D70
	private void OpenRewardBox()
	{
		if (this.m_RewardPlaymaker == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: Missing Playmaker FSM!");
			return;
		}
		if (this.m_XmarksRoot != null)
		{
			this.m_XmarksRoot.SetActive(false);
		}
		if (this.m_TheKeySelectionGlow != null)
		{
			this.m_TheKeySelectionGlow.SetActive(false);
		}
		this.m_WinsUberText.gameObject.SetActive(false);
		this.m_LossesUberText.gameObject.SetActive(false);
		SceneUtils.EnableColliders(this.m_TheKeyMesh, false);
		SceneUtils.SetLayer(this.m_TheKeyMesh.transform.parent.gameObject, GameLayer.Default);
		if (this.m_TheKeyIdleEffects)
		{
			PlayMakerFSM componentInChildren = this.m_TheKeyIdleEffects.GetComponentInChildren<PlayMakerFSM>();
			if (componentInChildren)
			{
				componentInChildren.SendEvent("Death");
			}
		}
		if (this.m_BehindTheDoors == null)
		{
			Debug.LogWarning("ArenaTrayDisplay: m_BehindTheDoors is null!");
			return;
		}
		this.m_BehindTheDoors.SetActive(true);
		this.m_RewardPlaymaker.SendEvent("Birth");
	}

	// Token: 0x06004F18 RID: 20248 RVA: 0x00177C88 File Offset: 0x00175E88
	private void OnRewardBoxesDone()
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		DraftManager draftManager = DraftManager.Get();
		if (draftManager.GetDraftDeck() == null)
		{
			Log.Rachelle.Print("bug 8052, null exception", new object[0]);
		}
		else
		{
			Network.AckDraftRewards(draftManager.GetDraftDeck().ID, draftManager.GetSlot());
		}
		DraftDisplay.Get().OnOpenRewardsComplete();
	}

	// Token: 0x06004F19 RID: 20249 RVA: 0x00177D00 File Offset: 0x00175F00
	private void ShowPlainPaper()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_Paper.SetActive(true);
			Renderer component = this.m_Paper.GetComponent<Renderer>();
			component.sharedMaterial = this.m_PlainPaperMaterial;
		}
		else
		{
			this.m_Paper.SetActive(false);
			this.m_PaperMain.SetActive(true);
		}
		this.m_XmarksRoot.SetActive(false);
		this.m_WinsUberText.Hide();
		this.m_LossesUberText.Hide();
	}

	// Token: 0x06004F1A RID: 20250 RVA: 0x00177D80 File Offset: 0x00175F80
	private void ShowRewardPaper()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_Paper.SetActive(true);
			Renderer component = this.m_Paper.GetComponent<Renderer>();
			component.sharedMaterial = this.m_RewardPaperMaterial;
		}
		else
		{
			this.m_Paper.SetActive(true);
			this.m_PaperMain.SetActive(false);
		}
		this.m_XmarksRoot.SetActive(true);
		this.m_WinsUberText.Show();
		this.m_LossesUberText.Show();
	}

	// Token: 0x06004F1B RID: 20251 RVA: 0x00177DFF File Offset: 0x00175FFF
	private void HidePaper()
	{
		this.m_Paper.SetActive(false);
	}

	// Token: 0x040035DD RID: 13789
	public int m_Rank;

	// Token: 0x040035DE RID: 13790
	public PlayMakerFSM m_RewardPlaymaker;

	// Token: 0x040035DF RID: 13791
	[CustomEditField(Sections = "Keys")]
	public GameObject m_TheKeyMesh;

	// Token: 0x040035E0 RID: 13792
	public GameObject m_TheKeyGlowPlane;

	// Token: 0x040035E1 RID: 13793
	public GameObject m_TheKeyGlowHoleMesh;

	// Token: 0x040035E2 RID: 13794
	public GameObject m_TheKeySelectionGlow;

	// Token: 0x040035E3 RID: 13795
	public GameObject m_TheKeyOldSelectionGlow;

	// Token: 0x040035E4 RID: 13796
	public float m_TheKeyTransitionDelay = 0.5f;

	// Token: 0x040035E5 RID: 13797
	public float m_TheKeyTransitionFadeInTime = 1.5f;

	// Token: 0x040035E6 RID: 13798
	public float m_TheKeyTransitionFadeOutTime = 2f;

	// Token: 0x040035E7 RID: 13799
	public ParticleSystem m_TheKeyTransitionParticles;

	// Token: 0x040035E8 RID: 13800
	public string m_TheKeyTransitionSound = "arena_key_transition";

	// Token: 0x040035E9 RID: 13801
	[CustomEditField(Sections = "Reward Panel")]
	public UberText m_WinCountUberText;

	// Token: 0x040035EA RID: 13802
	public UberText m_WinsUberText;

	// Token: 0x040035EB RID: 13803
	public UberText m_LossesUberText;

	// Token: 0x040035EC RID: 13804
	public GameObject m_XmarksRoot;

	// Token: 0x040035ED RID: 13805
	public List<GameObject> m_XmarkBox;

	// Token: 0x040035EE RID: 13806
	public GameObject m_Xmark1;

	// Token: 0x040035EF RID: 13807
	public GameObject m_Xmark2;

	// Token: 0x040035F0 RID: 13808
	public GameObject m_Xmark3;

	// Token: 0x040035F1 RID: 13809
	public GameObject m_RewardDoorPlates;

	// Token: 0x040035F2 RID: 13810
	public GameObject m_BehindTheDoors;

	// Token: 0x040035F3 RID: 13811
	public GameObject m_Paper;

	// Token: 0x040035F4 RID: 13812
	public GameObject m_PaperMain;

	// Token: 0x040035F5 RID: 13813
	public Material m_PlainPaperMaterial;

	// Token: 0x040035F6 RID: 13814
	public Material m_RewardPaperMaterial;

	// Token: 0x040035F7 RID: 13815
	public GameObject m_RewardBoxesBone;

	// Token: 0x040035F8 RID: 13816
	public GameObject m_InstructionText;

	// Token: 0x040035F9 RID: 13817
	public List<ArenaTrayDisplay.ArenaKeyVisualData> m_ArenaKeyVisualData;

	// Token: 0x040035FA RID: 13818
	private RewardBoxesDisplay m_RewardBoxes;

	// Token: 0x040035FB RID: 13819
	private GameObject m_TheKeyParticleSystems;

	// Token: 0x040035FC RID: 13820
	private GameObject m_TheKeyIdleEffects;

	// Token: 0x040035FD RID: 13821
	private bool m_isTheKeyIdleEffectsLoading;

	// Token: 0x040035FE RID: 13822
	private static ArenaTrayDisplay s_Instance;

	// Token: 0x020007F3 RID: 2035
	[Serializable]
	public class ArenaKeyVisualData
	{
		// Token: 0x040035FF RID: 13823
		public Mesh m_Mesh;

		// Token: 0x04003600 RID: 13824
		public Material m_Material;

		// Token: 0x04003601 RID: 13825
		public Mesh m_KeyHoleGlowMesh;

		// Token: 0x04003602 RID: 13826
		public Texture m_EffectGlowTexture;

		// Token: 0x04003603 RID: 13827
		public Texture m_SelectionGlowTexture;

		// Token: 0x04003604 RID: 13828
		public GameObject m_ParticlePrefab;

		// Token: 0x04003605 RID: 13829
		public string m_IdleEffectsPrefabName;
	}
}
