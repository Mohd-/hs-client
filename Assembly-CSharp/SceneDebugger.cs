using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class SceneDebugger : MonoBehaviour
{
	// Token: 0x06001BF3 RID: 7155 RVA: 0x000837FF File Offset: 0x000819FF
	private void Awake()
	{
		SceneDebugger.s_instance = this;
		if (ApplicationMgr.IsPublic())
		{
			Object.Destroy(this);
			return;
		}
		Time.timeScale = SceneDebugger.GetDevTimescale();
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x00083822 File Offset: 0x00081A22
	private void OnDestroy()
	{
		SceneDebugger.s_instance = null;
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x0008382A File Offset: 0x00081A2A
	private void OnGUI()
	{
		if (Options.Get().GetBool(Option.HUD))
		{
			this.LayoutLeftScreenControls();
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x00083842 File Offset: 0x00081A42
	public static float GetDevTimescale()
	{
		if (ApplicationMgr.IsPublic())
		{
			return 1f;
		}
		return Options.Get().GetFloat(Option.DEV_TIMESCALE, 1f);
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x00083865 File Offset: 0x00081A65
	public static void SetDevTimescale(float f)
	{
		if (ApplicationMgr.IsPublic())
		{
			return;
		}
		Options.Get().SetFloat(Option.DEV_TIMESCALE, f);
		Time.timeScale = f;
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x00083885 File Offset: 0x00081A85
	public static SceneDebugger Get()
	{
		return SceneDebugger.s_instance;
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x0008388C File Offset: 0x00081A8C
	public void AddMessage(string message)
	{
		this.InitMessagesIfNecessary();
		if (this.m_messages.Count >= 60)
		{
			this.m_messages.Dequeue();
		}
		this.m_messages.Enqueue(message);
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000838CC File Offset: 0x00081ACC
	private void LayoutLeftScreenControls()
	{
		Vector2 guisize = this.m_GUISize;
		Vector2 vector;
		vector..ctor((float)Screen.width * this.m_GUIPosition.x, (float)Screen.height * this.m_GUIPosition.y);
		Vector2 vector2;
		vector2..ctor(vector.x, vector.y);
		Vector2 vector3 = default(Vector2);
		vector3 = vector2;
		this.LayoutTimeControls(ref vector3, guisize);
		this.LayoutQualityControls(ref vector3, guisize);
		this.LayoutStats(ref vector3, guisize);
		this.LayoutMessages(ref vector3, guisize);
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x00083950 File Offset: 0x00081B50
	private void LayoutTimeControls(ref Vector2 offset, Vector2 size)
	{
		GUI.Box(new Rect(offset.x, offset.y, size.x, size.y), string.Format("Time Scale: {0}", SceneDebugger.GetDevTimescale()));
		offset.y += 1f * size.y;
		float devTimescale = GUI.HorizontalSlider(new Rect(offset.x, offset.y, size.x, size.y), Time.timeScale, this.m_MinTimeScale, this.m_MaxTimeScale);
		SceneDebugger.SetDevTimescale(devTimescale);
		offset.y += 1f * size.y;
		if (GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), "Reset Time Scale"))
		{
			SceneDebugger.SetDevTimescale(1f);
		}
		offset.y += 1.5f * size.y;
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x00083A58 File Offset: 0x00081C58
	private void LayoutQualityControls(ref Vector2 offset, Vector2 size)
	{
		float num = size.x / 3f;
		if (GUI.Button(new Rect(offset.x, offset.y, num, size.y), "Low"))
		{
			GraphicsManager.Get().RenderQualityLevel = GraphicsQuality.Low;
		}
		if (GUI.Button(new Rect(offset.x + num, offset.y, num, size.y), "Medium"))
		{
			GraphicsManager.Get().RenderQualityLevel = GraphicsQuality.Medium;
		}
		if (GUI.Button(new Rect(offset.x + num * 2f, offset.y, num, size.y), "High"))
		{
			GraphicsManager.Get().RenderQualityLevel = GraphicsQuality.High;
		}
		offset.y += 1.5f * size.y;
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x00083B30 File Offset: 0x00081D30
	private void LayoutStats(ref Vector2 offset, Vector2 size)
	{
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00083B34 File Offset: 0x00081D34
	[Conditional("UNITY_EDITOR")]
	private void LayoutCursorControls(ref Vector2 offset, Vector2 size)
	{
		if (Cursor.visible)
		{
			if (GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), "Force Hardware Cursor Off"))
			{
				Cursor.visible = false;
			}
		}
		else if (GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), "Force Hardware Cursor On"))
		{
			Cursor.visible = true;
		}
		offset.y += 1.5f * size.y;
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x00083BD2 File Offset: 0x00081DD2
	private void InitMessagesIfNecessary()
	{
		if (this.m_messages != null)
		{
			return;
		}
		this.m_messages = new QueueList<string>();
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x00083BEC File Offset: 0x00081DEC
	private void InitMessageStyleIfNecessary()
	{
		if (this.m_messageStyle != null)
		{
			return;
		}
		this.m_messageStyle = new GUIStyle("box")
		{
			alignment = 0,
			wordWrap = true,
			clipping = 0,
			stretchWidth = true
		};
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x00083C38 File Offset: 0x00081E38
	private void LayoutMessages(ref Vector2 offset, Vector2 size)
	{
		if (this.m_messages == null)
		{
			return;
		}
		if (this.m_messages.Count == 0)
		{
			return;
		}
		this.InitMessageStyleIfNecessary();
		if (this.m_hideMessages)
		{
			if (!GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), "Show Messages"))
			{
				return;
			}
			this.m_hideMessages = false;
		}
		else if (GUI.Button(new Rect(offset.x, offset.y, size.x, size.y), "Hide Messages"))
		{
			this.m_hideMessages = true;
			return;
		}
		if (GUI.Button(new Rect(size.x + offset.x, offset.y, size.x, size.y), "Clear Messages"))
		{
			this.m_messages.Clear();
			return;
		}
		offset.y += size.y;
		string messageText = this.GetMessageText();
		float num = (float)Screen.height - offset.y;
		GUI.Box(new Rect(offset.x, offset.y, (float)Screen.width, num), messageText, this.m_messageStyle);
		offset.y += num;
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x00083D88 File Offset: 0x00081F88
	private string GetMessageText()
	{
		this.m_messageBuilder = new StringBuilder();
		for (int i = 0; i < this.m_messages.Count; i++)
		{
			this.m_messageBuilder.AppendLine(this.m_messages[i]);
		}
		return this.m_messageBuilder.ToString();
	}

	// Token: 0x04000EA7 RID: 3751
	private const int MAX_MESSAGES = 60;

	// Token: 0x04000EA8 RID: 3752
	public float m_MinTimeScale = 0.01f;

	// Token: 0x04000EA9 RID: 3753
	public float m_MaxTimeScale = 4f;

	// Token: 0x04000EAA RID: 3754
	public Vector2 m_GUIPosition = new Vector2(0.01f, 0.065f);

	// Token: 0x04000EAB RID: 3755
	public Vector2 m_GUISize = new Vector2(175f, 30f);

	// Token: 0x04000EAC RID: 3756
	private static SceneDebugger s_instance;

	// Token: 0x04000EAD RID: 3757
	private QueueList<string> m_messages;

	// Token: 0x04000EAE RID: 3758
	private StringBuilder m_messageBuilder;

	// Token: 0x04000EAF RID: 3759
	private GUIStyle m_messageStyle;

	// Token: 0x04000EB0 RID: 3760
	private bool m_hideMessages;
}
