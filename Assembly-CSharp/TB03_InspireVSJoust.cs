using System;
using System.Collections;

// Token: 0x0200030C RID: 780
public class TB03_InspireVSJoust : MissionEntity
{
	// Token: 0x060028C6 RID: 10438 RVA: 0x000C644D File Offset: 0x000C464D
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TIRION_INTRO_03");
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000C645C File Offset: 0x000C465C
	public override IEnumerator PlayMissionIntroLineAndWait()
	{
		if (NotificationManager.Get().HasSoundPlayedThisSession("VO_TIRION_INTRO_03"))
		{
			yield break;
		}
		NotificationManager.Get().ForceAddSoundToPlayedList("VO_TIRION_INTRO_03");
		NotificationManager.Get().CreateCharacterQuote("Tirion_Quote", NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get("VO_TIRION_INTRO_03"), string.Empty, false, 30f, null, CanvasAnchor.BOTTOM_LEFT);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_TIRION_INTRO_03", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
		NotificationManager.Get().DestroyActiveQuote(0f);
		yield break;
	}
}
