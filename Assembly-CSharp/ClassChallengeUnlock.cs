using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AAA RID: 2730
[CustomEditClass]
public class ClassChallengeUnlock : Reward
{
	// Token: 0x06005EA4 RID: 24228 RVA: 0x001C53E4 File Offset: 0x001C35E4
	protected override void Awake()
	{
		base.Awake();
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_rewardBanner.transform.localScale = this.m_rewardBanner.transform.localScale * 8f;
		}
	}

	// Token: 0x06005EA5 RID: 24229 RVA: 0x001C5430 File Offset: 0x001C3630
	protected override void InitData()
	{
		base.SetData(new ClassChallengeUnlockData(), false);
	}

	// Token: 0x06005EA6 RID: 24230 RVA: 0x001C5440 File Offset: 0x001C3640
	protected override void ShowReward(bool updateCacheValues)
	{
		this.m_root.SetActive(true);
		this.m_classFrameContainer.UpdatePositions();
		foreach (GameObject gameObject in this.m_classFrames)
		{
			gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				new Vector3(0f, 0f, 540f),
				"time",
				1.5f,
				"easeType",
				iTween.EaseType.easeOutElastic,
				"space",
				1
			});
			iTween.RotateAdd(gameObject, args);
		}
		FullScreenFXMgr.Get().StartStandardBlurVignette(1f);
	}

	// Token: 0x06005EA7 RID: 24231 RVA: 0x001C5548 File Offset: 0x001C3748
	protected override void HideReward()
	{
		base.HideReward();
		FullScreenFXMgr.Get().EndStandardBlurVignette(1f, new FullScreenFXMgr.EffectListener(this.DestroyClassChallengeUnlock));
		this.m_root.SetActive(false);
	}

	// Token: 0x06005EA8 RID: 24232 RVA: 0x001C5578 File Offset: 0x001C3778
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		ClassChallengeUnlockData classChallengeUnlockData = base.Data as ClassChallengeUnlockData;
		if (classChallengeUnlockData == null)
		{
			Debug.LogWarning(string.Format("ClassChallengeUnlock.OnDataSet() - Data {0} is not ClassChallengeUnlockData", base.Data));
			return;
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<AdventureMissionDbfRecord> records = GameDbf.AdventureMission.GetRecords();
		foreach (AdventureMissionDbfRecord adventureMissionDbfRecord in records)
		{
			int reqWingId = adventureMissionDbfRecord.ReqWingId;
			if (reqWingId == classChallengeUnlockData.WingID)
			{
				int scenarioId = adventureMissionDbfRecord.ScenarioId;
				ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(scenarioId);
				if (record == null)
				{
					Debug.LogError(string.Format("Unable to find Scenario record with ID: {0}", scenarioId));
				}
				else if (record.ModeId == 4)
				{
					if (!string.IsNullOrEmpty(adventureMissionDbfRecord.ClassChallengePrefabPopup))
					{
						DbfLocValue shortName = record.ShortName;
						list.Add(adventureMissionDbfRecord.ClassChallengePrefabPopup);
						list2.Add(shortName);
					}
					else
					{
						Debug.LogWarning(string.Format("CLASS_CHALLENGE_PREFAB_POPUP not define for AdventureMission SCENARIO_ID: {0}", scenarioId));
					}
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogError(string.Format("Unable to find AdventureMission record with REQ_WING_ID: {0}.", classChallengeUnlockData.WingID));
			return;
		}
		GameStrings.PluralNumber[] pluralNumbers = new GameStrings.PluralNumber[]
		{
			new GameStrings.PluralNumber
			{
				m_number = list.Count
			}
		};
		this.m_headerText.Text = GameStrings.FormatPlurals("GLOBAL_REWARD_CLASS_CHALLENGE_HEADLINE", pluralNumbers, new object[0]);
		string headline;
		if (list.Count > 0)
		{
			headline = string.Join(", ", list2.ToArray());
		}
		else
		{
			headline = string.Empty;
		}
		WingDbfRecord record2 = GameDbf.Wing.GetRecord(classChallengeUnlockData.WingID);
		string source = record2.ClassChallengeRewardSource;
		base.SetRewardText(headline, string.Empty, source);
		foreach (string path in list)
		{
			GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(path), true, false);
			if (!(gameObject == null))
			{
				GameUtils.SetParent(gameObject, this.m_classFrameContainer, false);
				gameObject.transform.localRotation = Quaternion.identity;
				this.m_classFrameContainer.AddObject(gameObject, true);
				this.m_classFrames.Add(gameObject);
			}
		}
		this.m_classFrameContainer.UpdatePositions();
		base.SetReady(true);
		base.EnableClickCatcher(true);
		base.RegisterClickListener(new Reward.OnClickedCallback(this.OnClicked));
	}

	// Token: 0x06005EA9 RID: 24233 RVA: 0x001C584C File Offset: 0x001C3A4C
	private void OnClicked(Reward reward, object userData)
	{
		this.HideReward();
	}

	// Token: 0x06005EAA RID: 24234 RVA: 0x001C5854 File Offset: 0x001C3A54
	private void DestroyClassChallengeUnlock()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x04004629 RID: 17961
	[CustomEditField(Sections = "Container")]
	public UIBObjectSpacing m_classFrameContainer;

	// Token: 0x0400462A RID: 17962
	[CustomEditField(Sections = "Text Settings")]
	public UberText m_headerText;

	// Token: 0x0400462B RID: 17963
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_appearSound;

	// Token: 0x0400462C RID: 17964
	private List<GameObject> m_classFrames = new List<GameObject>();
}
