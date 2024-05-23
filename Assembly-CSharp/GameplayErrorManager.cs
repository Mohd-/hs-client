using System;
using UnityEngine;

// Token: 0x02000859 RID: 2137
public class GameplayErrorManager : MonoBehaviour
{
	// Token: 0x06005247 RID: 21063 RVA: 0x00188F89 File Offset: 0x00187189
	private void Awake()
	{
		GameplayErrorManager.s_instance = this;
		GameplayErrorManager.s_messageInstance = Object.Instantiate<GameplayErrorCloud>(this.m_errorMessagePrefab);
	}

	// Token: 0x06005248 RID: 21064 RVA: 0x00188FA1 File Offset: 0x001871A1
	private void OnDestroy()
	{
		GameplayErrorManager.s_instance = null;
	}

	// Token: 0x06005249 RID: 21065 RVA: 0x00188FA9 File Offset: 0x001871A9
	private void Start()
	{
		this.m_message = string.Empty;
		this.m_errorDisplayStyle = new GUIStyle();
		this.m_errorDisplayStyle.fontSize = 24;
		this.m_errorDisplayStyle.fontStyle = 1;
		this.m_errorDisplayStyle.alignment = 1;
	}

	// Token: 0x0600524A RID: 21066 RVA: 0x00188FE6 File Offset: 0x001871E6
	public static GameplayErrorManager Get()
	{
		return GameplayErrorManager.s_instance;
	}

	// Token: 0x0600524B RID: 21067 RVA: 0x00188FF0 File Offset: 0x001871F0
	public void DisplayMessage(string message)
	{
		this.m_message = message;
		this.m_displaySecsLeft = (float)message.Length * 0.1f;
		if (UniversalInputManager.UsePhoneUI)
		{
			GameplayErrorManager.s_messageInstance.transform.localPosition = new Vector3(-7.9f, 9f, -4.43f);
			UberText componentInChildren = GameplayErrorManager.s_messageInstance.gameObject.GetComponentInChildren<UberText>();
			componentInChildren.gameObject.transform.localPosition = new Vector3(2.49f, 0f, -2.13f);
		}
		else
		{
			GameplayErrorManager.s_messageInstance.transform.localPosition = new Vector3(-7.9f, 9.98f, -5.17f);
		}
		GameplayErrorManager.s_messageInstance.ShowMessage(this.m_message, this.m_displaySecsLeft);
		SoundManager.Get().LoadAndPlay("UI_no_can_do");
	}

	// Token: 0x0400388A RID: 14474
	private static GameplayErrorManager s_instance;

	// Token: 0x0400388B RID: 14475
	private static GameplayErrorCloud s_messageInstance;

	// Token: 0x0400388C RID: 14476
	private GUIStyle m_errorDisplayStyle;

	// Token: 0x0400388D RID: 14477
	private string m_message;

	// Token: 0x0400388E RID: 14478
	private float m_displaySecsLeft;

	// Token: 0x0400388F RID: 14479
	public GameplayErrorCloud m_errorMessagePrefab;
}
