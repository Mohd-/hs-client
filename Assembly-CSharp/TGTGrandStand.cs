using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000666 RID: 1638
[CustomEditClass]
public class TGTGrandStand : MonoBehaviour
{
	// Token: 0x06004621 RID: 17953 RVA: 0x00151953 File Offset: 0x0014FB53
	private void Awake()
	{
		TGTGrandStand.s_instance = this;
	}

	// Token: 0x06004622 RID: 17954 RVA: 0x0015195B File Offset: 0x0014FB5B
	private void Start()
	{
		base.StartCoroutine(this.RegisterBoardEvents());
	}

	// Token: 0x06004623 RID: 17955 RVA: 0x0015196A File Offset: 0x0014FB6A
	private void Update()
	{
		this.HandleClicks();
	}

	// Token: 0x06004624 RID: 17956 RVA: 0x00151972 File Offset: 0x0014FB72
	private void OnDestroy()
	{
		TGTGrandStand.s_instance = null;
	}

	// Token: 0x06004625 RID: 17957 RVA: 0x0015197A File Offset: 0x0014FB7A
	public static TGTGrandStand Get()
	{
		return TGTGrandStand.s_instance;
	}

	// Token: 0x06004626 RID: 17958 RVA: 0x00151984 File Offset: 0x0014FB84
	private void HandleClicks()
	{
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_HumanRoot))
		{
			this.HumanClick();
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_OrcRoot))
		{
			this.OrcClick();
		}
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_KnightRoot))
		{
			this.KnightClick();
		}
	}

	// Token: 0x06004627 RID: 17959 RVA: 0x00151A08 File Offset: 0x0014FC08
	private void HumanClick()
	{
		this.m_HumanRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -30f);
		if (!string.IsNullOrEmpty(this.m_ClickHumanSound))
		{
			string text = FileUtils.GameAssetPathToName(this.m_ClickHumanSound);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_HumanRoot);
			}
		}
	}

	// Token: 0x06004628 RID: 17960 RVA: 0x00151A74 File Offset: 0x0014FC74
	private void OrcClick()
	{
		this.m_OrcRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -30f);
		if (!string.IsNullOrEmpty(this.m_ClickOrcSound))
		{
			string text = FileUtils.GameAssetPathToName(this.m_ClickOrcSound);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_OrcRoot);
			}
		}
	}

	// Token: 0x06004629 RID: 17961 RVA: 0x00151AE0 File Offset: 0x0014FCE0
	private void KnightClick()
	{
		this.m_KnightRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -30f);
		if (!string.IsNullOrEmpty(this.m_ClickKnightSound))
		{
			string text = FileUtils.GameAssetPathToName(this.m_ClickKnightSound);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_KnightRoot);
			}
		}
	}

	// Token: 0x0600462A RID: 17962 RVA: 0x00151B4C File Offset: 0x0014FD4C
	private IEnumerator TestAnimations()
	{
		yield return new WaitForSeconds(4f);
		this.PlayCheerAnimation();
		yield return new WaitForSeconds(8f);
		this.PlayCheerAnimation();
		yield return new WaitForSeconds(9f);
		this.PlayCheerAnimation();
		yield return new WaitForSeconds(8f);
		this.PlayOhNoAnimation();
		yield return new WaitForSeconds(8f);
		this.PlayOhNoAnimation();
		yield break;
	}

	// Token: 0x0600462B RID: 17963 RVA: 0x00151B68 File Offset: 0x0014FD68
	public void PlayCheerAnimation()
	{
		int num = Random.Range(0, this.ANIMATION_CHEER.Length);
		string animName = this.ANIMATION_CHEER[num];
		this.PlayAnimation(this.m_HumanAnimator, animName, 4f);
		this.PlaySoundFromList(this.m_CheerHumanSounds, num);
		num = Random.Range(0, this.ANIMATION_CHEER.Length);
		animName = this.ANIMATION_CHEER[num];
		this.PlayAnimation(this.m_OrcAnimator, animName, 4f);
		this.PlaySoundFromList(this.m_CheerOrcSounds, num);
		num = Random.Range(0, this.ANIMATION_CHEER.Length);
		animName = this.ANIMATION_CHEER[num];
		this.PlayAnimation(this.m_KnightAnimator, animName, 4f);
		this.PlaySoundFromList(this.m_CheerKnightSounds, num);
	}

	// Token: 0x0600462C RID: 17964 RVA: 0x00151C1C File Offset: 0x0014FE1C
	public void PlayOhNoAnimation()
	{
		int num = Random.Range(0, this.ANIMATION_OHNO.Length);
		string animName = this.ANIMATION_OHNO[num];
		this.PlayAnimation(this.m_HumanAnimator, animName, 3.5f);
		this.PlaySoundFromList(this.m_OhNoHumanSounds, num);
		num = Random.Range(0, this.ANIMATION_OHNO.Length);
		animName = this.ANIMATION_OHNO[num];
		this.PlayAnimation(this.m_OrcAnimator, animName, 3.5f);
		this.PlaySoundFromList(this.m_OhNoOrcSounds, num);
		num = Random.Range(0, this.ANIMATION_OHNO.Length);
		animName = this.ANIMATION_OHNO[num];
		this.PlayAnimation(this.m_KnightAnimator, animName, 3.5f);
		this.PlaySoundFromList(this.m_OhNoKnightSounds, num);
	}

	// Token: 0x0600462D RID: 17965 RVA: 0x00151CD0 File Offset: 0x0014FED0
	public void PlayScoreCard(string humanScore, string orcScore, string knightScore)
	{
		this.m_HumanScoreUberText.Text = humanScore;
		this.m_OrcScoreUberText.Text = orcScore;
		this.m_KnightScoreUberText.Text = knightScore;
		this.m_HumanAnimator.SetTrigger("ScoreCard");
		this.m_OrcAnimator.SetTrigger("ScoreCard");
		this.m_KnightAnimator.SetTrigger("ScoreCard");
		this.PlaySound(this.m_ScoreCardSound);
	}

	// Token: 0x0600462E RID: 17966 RVA: 0x00151D40 File Offset: 0x0014FF40
	private void PlaySoundFromList(List<string> soundList, int index)
	{
		if (soundList == null || soundList.Count == 0)
		{
			return;
		}
		if (index > soundList.Count)
		{
			index = 0;
		}
		this.PlaySound(soundList[index]);
	}

	// Token: 0x0600462F RID: 17967 RVA: 0x00151D7C File Offset: 0x0014FF7C
	private void PlaySound(string soundString)
	{
		if (!string.IsNullOrEmpty(soundString))
		{
			string text = FileUtils.GameAssetPathToName(soundString);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_OrcRoot);
			}
		}
	}

	// Token: 0x06004630 RID: 17968 RVA: 0x00151DB8 File Offset: 0x0014FFB8
	private void PlayAnimation(Animator animator, string animName, float time)
	{
		this.m_isAnimating = true;
		this.m_HumanScoreCard.SetActive(false);
		this.m_OrcScoreCard.SetActive(false);
		this.m_KnightScoreCard.SetActive(false);
		base.StartCoroutine(this.PlayAnimationRandomStart(animator, animName, time));
	}

	// Token: 0x06004631 RID: 17969 RVA: 0x00151E00 File Offset: 0x00150000
	private IEnumerator PlayAnimationRandomStart(Animator animator, string animName, float time)
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
		animator.SetTrigger(animName);
		base.StartCoroutine(this.ReturnToIdleAnimation(animator, time));
		yield break;
	}

	// Token: 0x06004632 RID: 17970 RVA: 0x00151E48 File Offset: 0x00150048
	private IEnumerator ReturnToIdleAnimation(Animator animator, float time)
	{
		yield return new WaitForSeconds(time);
		this.m_isAnimating = false;
		animator.SetTrigger("Idle");
		yield break;
	}

	// Token: 0x06004633 RID: 17971 RVA: 0x00151E80 File Offset: 0x00150080
	private void Shake()
	{
		if (this.m_isAnimating)
		{
			return;
		}
		base.StartCoroutine(this.ShakeHuman());
		base.StartCoroutine(this.ShakeOrc());
		base.StartCoroutine(this.ShakeKnight());
	}

	// Token: 0x06004634 RID: 17972 RVA: 0x00151EC0 File Offset: 0x001500C0
	private IEnumerator ShakeHuman()
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
		this.m_HumanRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -25f);
		yield break;
	}

	// Token: 0x06004635 RID: 17973 RVA: 0x00151EDC File Offset: 0x001500DC
	private IEnumerator ShakeOrc()
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
		this.m_OrcRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -25f);
		yield break;
	}

	// Token: 0x06004636 RID: 17974 RVA: 0x00151EF8 File Offset: 0x001500F8
	private IEnumerator ShakeKnight()
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
		this.m_KnightRoot.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -25f);
		yield break;
	}

	// Token: 0x06004637 RID: 17975 RVA: 0x00151F14 File Offset: 0x00150114
	private bool IsOver(GameObject go)
	{
		return go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
	}

	// Token: 0x06004638 RID: 17976 RVA: 0x00151F50 File Offset: 0x00150150
	private IEnumerator RegisterBoardEvents()
	{
		while (BoardEvents.Get() == null)
		{
			yield return null;
		}
		this.m_boardEvents = BoardEvents.Get();
		this.m_boardEvents.RegisterFriendlyHeroDamageEvent(new BoardEvents.EventDelegate(this.FriendlyHeroDamage), 7f);
		this.m_boardEvents.RegisterOpponentHeroDamageEvent(new BoardEvents.EventDelegate(this.OpponentHeroDamage), 10f);
		this.m_boardEvents.RegisterFriendlyLegendaryMinionSpawnEvent(new BoardEvents.EventDelegate(this.FriendlyLegendarySpawn), 6f);
		this.m_boardEvents.RegisterOppenentLegendaryMinionSpawnEvent(new BoardEvents.EventDelegate(this.OpponentLegendarySpawn), 9f);
		this.m_boardEvents.RegisterFriendlyLegendaryMinionDeathEvent(new BoardEvents.EventDelegate(this.FriendlyLegendaryDeath), 6f);
		this.m_boardEvents.RegisterOppenentLegendaryMinionDeathEvent(new BoardEvents.EventDelegate(this.OpponentLegendaryDeath), 9f);
		this.m_boardEvents.RegisterFriendlyMinionDamageEvent(new BoardEvents.EventDelegate(this.FriendlyMinionDamage), 15f);
		this.m_boardEvents.RegisterOpponentMinionDamageEvent(new BoardEvents.EventDelegate(this.OpponentMinionDamage), 15f);
		this.m_boardEvents.RegisterFriendlyMinionDeathEvent(new BoardEvents.EventDelegate(this.FriendlyMinionDeath), 15f);
		this.m_boardEvents.RegisterOppenentMinionDeathEvent(new BoardEvents.EventDelegate(this.OpponentMinionDeath), 15f);
		this.m_boardEvents.RegisterFriendlyMinionSpawnEvent(new BoardEvents.EventDelegate(this.FriendlyMinionSpawn), 10f);
		this.m_boardEvents.RegisterOppenentMinionSpawnEvent(new BoardEvents.EventDelegate(this.OpponentMinionSpawn), 10f);
		this.m_boardEvents.RegisterLargeShakeEvent(new BoardEvents.LargeShakeEventDelegate(this.Shake));
		yield break;
	}

	// Token: 0x06004639 RID: 17977 RVA: 0x00151F6B File Offset: 0x0015016B
	private void FriendlyHeroDamage(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x0600463A RID: 17978 RVA: 0x00151F74 File Offset: 0x00150174
	private void OpponentHeroDamage(float weight)
	{
		if (weight <= 15f)
		{
			this.PlayCheerAnimation();
			return;
		}
		if (weight > 20f)
		{
			this.PlayScoreCard("10", "10", "10");
			return;
		}
		this.PlayScoreCard("10", Random.Range(7, 9).ToString(), Random.Range(8, 10).ToString());
	}

	// Token: 0x0600463B RID: 17979 RVA: 0x00151FDF File Offset: 0x001501DF
	private void FriendlyLegendarySpawn(float weight)
	{
		this.PlayCheerAnimation();
	}

	// Token: 0x0600463C RID: 17980 RVA: 0x00151FE7 File Offset: 0x001501E7
	private void OpponentLegendarySpawn(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x0600463D RID: 17981 RVA: 0x00151FEF File Offset: 0x001501EF
	private void FriendlyLegendaryDeath(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x0600463E RID: 17982 RVA: 0x00151FF7 File Offset: 0x001501F7
	private void OpponentLegendaryDeath(float weight)
	{
		this.PlayCheerAnimation();
	}

	// Token: 0x0600463F RID: 17983 RVA: 0x00151FFF File Offset: 0x001501FF
	private void FriendlyMinionDamage(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x06004640 RID: 17984 RVA: 0x00152007 File Offset: 0x00150207
	private void OpponentMinionDamage(float weight)
	{
		this.PlayCheerAnimation();
	}

	// Token: 0x06004641 RID: 17985 RVA: 0x0015200F File Offset: 0x0015020F
	private void FriendlyMinionDeath(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x06004642 RID: 17986 RVA: 0x00152017 File Offset: 0x00150217
	private void OpponentMinionDeath(float weight)
	{
		this.PlayCheerAnimation();
	}

	// Token: 0x06004643 RID: 17987 RVA: 0x0015201F File Offset: 0x0015021F
	private void FriendlyMinionSpawn(float weight)
	{
		this.PlayCheerAnimation();
	}

	// Token: 0x06004644 RID: 17988 RVA: 0x00152027 File Offset: 0x00150227
	private void OpponentMinionSpawn(float weight)
	{
		this.PlayOhNoAnimation();
	}

	// Token: 0x04002D1F RID: 11551
	private const string ANIMATION_IDLE = "Idle";

	// Token: 0x04002D20 RID: 11552
	private const string ANIMATION_SCORE_CARD = "ScoreCard";

	// Token: 0x04002D21 RID: 11553
	private const float MIN_RANDOM_TIME_FACTOR = 0.05f;

	// Token: 0x04002D22 RID: 11554
	private const float MAX_RANDOM_TIME_FACTOR = 0.2f;

	// Token: 0x04002D23 RID: 11555
	private const float CHEER_ANIMATION_PLAY_TIME = 4f;

	// Token: 0x04002D24 RID: 11556
	private const float OHNO_ANIMATION_PLAY_TIME = 3.5f;

	// Token: 0x04002D25 RID: 11557
	private const float FRIENDLY_HERO_DAMAGE_WEIGHT_TRGGER = 7f;

	// Token: 0x04002D26 RID: 11558
	private const float OPPONENT_HERO_DAMAGE_WEIGHT_TRGGER = 10f;

	// Token: 0x04002D27 RID: 11559
	private const float FRIENDLY_LEGENDARY_SPAWN_MIN_COST_TRGGER = 6f;

	// Token: 0x04002D28 RID: 11560
	private const float OPPONENT_LEGENDARY_SPAWN_MIN_COST_TRGGER = 9f;

	// Token: 0x04002D29 RID: 11561
	private const float FRIENDLY_LEGENDARY_DEATH_MIN_COST_TRGGER = 6f;

	// Token: 0x04002D2A RID: 11562
	private const float OPPONENT_LEGENDARY_DEATH_MIN_COST_TRGGER = 9f;

	// Token: 0x04002D2B RID: 11563
	private const float FRIENDLY_MINION_DAMAGE_WEIGHT = 15f;

	// Token: 0x04002D2C RID: 11564
	private const float OPPONENT_MINION_DAMAGE_WEIGHT = 15f;

	// Token: 0x04002D2D RID: 11565
	private const float FRIENDLY_MINION_DEATH_WEIGHT = 15f;

	// Token: 0x04002D2E RID: 11566
	private const float OPPONENT_MINION_DEATH_WEIGHT = 15f;

	// Token: 0x04002D2F RID: 11567
	private const float FRIENDLY_MINION_SPAWN_WEIGHT = 10f;

	// Token: 0x04002D30 RID: 11568
	private const float OPPONENT_MINION_SPAWN_WEIGHT = 10f;

	// Token: 0x04002D31 RID: 11569
	private const float OPPONENT_HERO_DAMAGE_SCORE_CARD_TRIGGER = 15f;

	// Token: 0x04002D32 RID: 11570
	private const float OPPONENT_HERO_DAMAGE_SCORE_CARD_10S_TRIGGER = 20f;

	// Token: 0x04002D33 RID: 11571
	private readonly string[] ANIMATION_CHEER = new string[]
	{
		"Cheer01",
		"Cheer02",
		"Cheer03"
	};

	// Token: 0x04002D34 RID: 11572
	private readonly string[] ANIMATION_OHNO = new string[]
	{
		"OhNo01",
		"OhNo02"
	};

	// Token: 0x04002D35 RID: 11573
	public GameObject m_HumanRoot;

	// Token: 0x04002D36 RID: 11574
	public GameObject m_OrcRoot;

	// Token: 0x04002D37 RID: 11575
	public GameObject m_KnightRoot;

	// Token: 0x04002D38 RID: 11576
	public Animator m_HumanAnimator;

	// Token: 0x04002D39 RID: 11577
	public Animator m_OrcAnimator;

	// Token: 0x04002D3A RID: 11578
	public Animator m_KnightAnimator;

	// Token: 0x04002D3B RID: 11579
	public GameObject m_HumanScoreCard;

	// Token: 0x04002D3C RID: 11580
	public GameObject m_OrcScoreCard;

	// Token: 0x04002D3D RID: 11581
	public GameObject m_KnightScoreCard;

	// Token: 0x04002D3E RID: 11582
	public UberText m_HumanScoreUberText;

	// Token: 0x04002D3F RID: 11583
	public UberText m_OrcScoreUberText;

	// Token: 0x04002D40 RID: 11584
	public UberText m_KnightScoreUberText;

	// Token: 0x04002D41 RID: 11585
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Human Sounds")]
	public string m_ClickHumanSound;

	// Token: 0x04002D42 RID: 11586
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Human Sounds")]
	public List<string> m_CheerHumanSounds;

	// Token: 0x04002D43 RID: 11587
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Human Sounds")]
	public List<string> m_OhNoHumanSounds;

	// Token: 0x04002D44 RID: 11588
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Orc Sounds")]
	public string m_ClickOrcSound;

	// Token: 0x04002D45 RID: 11589
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Orc Sounds")]
	public List<string> m_CheerOrcSounds;

	// Token: 0x04002D46 RID: 11590
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Orc Sounds")]
	public List<string> m_OhNoOrcSounds;

	// Token: 0x04002D47 RID: 11591
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Knight Sounds")]
	public string m_ClickKnightSound;

	// Token: 0x04002D48 RID: 11592
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Knight Sounds")]
	public List<string> m_CheerKnightSounds;

	// Token: 0x04002D49 RID: 11593
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Knight Sounds")]
	public List<string> m_OhNoKnightSounds;

	// Token: 0x04002D4A RID: 11594
	[CustomEditField(T = EditType.SOUND_PREFAB, Sections = "Sounds")]
	public string m_ScoreCardSound;

	// Token: 0x04002D4B RID: 11595
	private BoardEvents m_boardEvents;

	// Token: 0x04002D4C RID: 11596
	private bool m_isAnimating;

	// Token: 0x04002D4D RID: 11597
	private static TGTGrandStand s_instance;
}
