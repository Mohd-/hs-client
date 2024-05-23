using System;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class ChatLogMessageFrame : MonoBehaviour
{
	// Token: 0x060040B6 RID: 16566 RVA: 0x00137F7C File Offset: 0x0013617C
	private void Awake()
	{
		Bounds bounds = this.m_Background.GetComponent<Collider>().bounds;
		Bounds textWorldSpaceBounds = this.m_Text.GetTextWorldSpaceBounds();
		this.m_initialPadding = bounds.size.y - textWorldSpaceBounds.size.y;
		this.m_initialBackgroundHeight = bounds.size.y;
		this.m_initialBackgroundLocalScaleY = this.m_Background.transform.localScale.y;
	}

	// Token: 0x060040B7 RID: 16567 RVA: 0x00138000 File Offset: 0x00136200
	public string GetMessage()
	{
		return this.m_Text.Text;
	}

	// Token: 0x060040B8 RID: 16568 RVA: 0x0013800D File Offset: 0x0013620D
	public void SetMessage(string message)
	{
		this.m_Text.Text = message;
		this.UpdateText();
		this.UpdateBackground();
	}

	// Token: 0x060040B9 RID: 16569 RVA: 0x00138027 File Offset: 0x00136227
	public Color GetColor()
	{
		return this.m_Text.TextColor;
	}

	// Token: 0x060040BA RID: 16570 RVA: 0x00138034 File Offset: 0x00136234
	public void SetColor(Color color)
	{
		this.m_Text.TextColor = color;
	}

	// Token: 0x060040BB RID: 16571 RVA: 0x00138042 File Offset: 0x00136242
	private void UpdateText()
	{
		this.m_Text.UpdateNow();
	}

	// Token: 0x060040BC RID: 16572 RVA: 0x00138050 File Offset: 0x00136250
	private void UpdateBackground()
	{
		float num = this.m_Text.GetTextWorldSpaceBounds().size.y + this.m_initialPadding;
		float num2 = this.m_initialBackgroundLocalScaleY;
		if (num > this.m_initialBackgroundHeight)
		{
			num2 *= num / this.m_initialBackgroundHeight;
		}
		TransformUtil.SetLocalScaleY(this.m_Background, num2);
	}

	// Token: 0x04002941 RID: 10561
	public GameObject m_Background;

	// Token: 0x04002942 RID: 10562
	public UberText m_Text;

	// Token: 0x04002943 RID: 10563
	private float m_initialPadding;

	// Token: 0x04002944 RID: 10564
	private float m_initialBackgroundHeight;

	// Token: 0x04002945 RID: 10565
	private float m_initialBackgroundLocalScaleY;
}
