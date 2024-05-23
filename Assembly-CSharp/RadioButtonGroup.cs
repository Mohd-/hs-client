using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F5C RID: 3932
public class RadioButtonGroup : MonoBehaviour
{
	// Token: 0x060074CC RID: 29900 RVA: 0x00227D74 File Offset: 0x00225F74
	public void ShowButtons(List<RadioButtonGroup.ButtonData> buttonData, RadioButtonGroup.DelButtonSelected buttonSelectedCallback, RadioButtonGroup.DelButtonDoubleClicked buttonDoubleClickedCallback)
	{
		this.m_buttonContainer.SetActive(true);
		int count = buttonData.Count;
		while (this.m_framedRadioButtons.Count > count)
		{
			FramedRadioButton framedRadioButton = this.m_framedRadioButtons[0];
			this.m_framedRadioButtons.RemoveAt(0);
			Object.DestroyImmediate(framedRadioButton);
		}
		bool flag = 1 == count;
		Vector3 position = this.m_buttonContainer.transform.position;
		GameObject relative = new GameObject();
		RadioButton radioButton = null;
		for (int i = 0; i < count; i++)
		{
			FramedRadioButton framedRadioButton2;
			if (this.m_framedRadioButtons.Count > i)
			{
				framedRadioButton2 = this.m_framedRadioButtons[i];
			}
			else
			{
				framedRadioButton2 = this.CreateNewFramedRadioButton();
				this.m_framedRadioButtons.Add(framedRadioButton2);
			}
			FramedRadioButton.FrameType frameType;
			if (flag)
			{
				frameType = FramedRadioButton.FrameType.SINGLE;
			}
			else if (i == 0)
			{
				frameType = FramedRadioButton.FrameType.MULTI_LEFT_END;
			}
			else if (count - 1 == i)
			{
				frameType = FramedRadioButton.FrameType.MULTI_RIGHT_END;
			}
			else
			{
				frameType = FramedRadioButton.FrameType.MULTI_MIDDLE;
			}
			RadioButtonGroup.ButtonData buttonData2 = buttonData[i];
			framedRadioButton2.Show();
			framedRadioButton2.Init(frameType, buttonData2.m_text, buttonData2.m_id, buttonData2.m_userData);
			if (buttonData2.m_selected)
			{
				if (radioButton != null)
				{
					Debug.LogWarning("RadioButtonGroup.WaitThenShowButtons(): more than one button was set as selected. Selecting the FIRST provided option.");
					framedRadioButton2.m_radioButton.SetSelected(false);
				}
				else
				{
					radioButton = framedRadioButton2.m_radioButton;
					radioButton.SetSelected(true);
				}
			}
			else
			{
				framedRadioButton2.m_radioButton.SetSelected(false);
			}
			if (i == 0)
			{
				TransformUtil.SetPoint(framedRadioButton2.gameObject, Anchor.LEFT, this.m_firstRadioButtonBone, Anchor.LEFT);
			}
			else
			{
				TransformUtil.SetPoint(framedRadioButton2.gameObject, new Vector3(0f, 1f, 0.5f), relative, new Vector3(1f, 1f, 0.5f), this.m_spacingFudgeFactor);
			}
			relative = framedRadioButton2.m_frameFill;
		}
		position.x -= TransformUtil.GetBoundsOfChildren(this.m_buttonContainer).size.x / 2f;
		this.m_buttonContainer.transform.position = position;
		this.m_buttonSelectedCB = buttonSelectedCallback;
		this.m_buttonDoubleClickedCB = buttonDoubleClickedCallback;
		if (radioButton == null)
		{
			return;
		}
		if (this.m_buttonSelectedCB == null)
		{
			return;
		}
		this.m_buttonSelectedCB(radioButton.GetButtonID(), radioButton.GetUserData());
	}

	// Token: 0x060074CD RID: 29901 RVA: 0x00227FDB File Offset: 0x002261DB
	public void Hide()
	{
		this.m_buttonContainer.SetActive(false);
	}

	// Token: 0x060074CE RID: 29902 RVA: 0x00227FE9 File Offset: 0x002261E9
	public void SetSpacingFudgeFactor(Vector3 amount)
	{
		this.m_spacingFudgeFactor = amount;
	}

	// Token: 0x060074CF RID: 29903 RVA: 0x00227FF4 File Offset: 0x002261F4
	private FramedRadioButton CreateNewFramedRadioButton()
	{
		FramedRadioButton framedRadioButton = Object.Instantiate<FramedRadioButton>(this.m_framedRadioButtonPrefab);
		framedRadioButton.transform.parent = this.m_buttonContainer.transform;
		framedRadioButton.transform.localPosition = Vector3.zero;
		framedRadioButton.transform.localScale = Vector3.one;
		framedRadioButton.transform.localRotation = Quaternion.identity;
		framedRadioButton.m_radioButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRadioButtonReleased));
		framedRadioButton.m_radioButton.AddEventListener(UIEventType.DOUBLECLICK, new UIEvent.Handler(this.OnRadioButtonDoubleClicked));
		return framedRadioButton;
	}

	// Token: 0x060074D0 RID: 29904 RVA: 0x00228088 File Offset: 0x00226288
	private void OnRadioButtonReleased(UIEvent e)
	{
		RadioButton radioButton = e.GetElement() as RadioButton;
		if (radioButton == null)
		{
			Debug.LogWarning(string.Format("RadioButtonGroup.OnRadioButtonReleased(): UIEvent {0} source is not a RadioButton!", e));
			return;
		}
		bool flag = radioButton.IsSelected();
		foreach (FramedRadioButton framedRadioButton in this.m_framedRadioButtons)
		{
			RadioButton radioButton2 = framedRadioButton.m_radioButton;
			bool selected = radioButton == radioButton2;
			radioButton2.SetSelected(selected);
		}
		if (this.m_buttonSelectedCB == null)
		{
			return;
		}
		this.m_buttonSelectedCB(radioButton.GetButtonID(), radioButton.GetUserData());
		if (UniversalInputManager.Get().IsTouchMode() && flag)
		{
			this.OnRadioButtonDoubleClicked(e);
		}
	}

	// Token: 0x060074D1 RID: 29905 RVA: 0x00228168 File Offset: 0x00226368
	private void OnRadioButtonDoubleClicked(UIEvent e)
	{
		if (this.m_buttonDoubleClickedCB == null)
		{
			return;
		}
		RadioButton radioButton = e.GetElement() as RadioButton;
		if (radioButton == null)
		{
			Debug.LogWarning(string.Format("RadioButtonGroup.OnRadioButtonDoubleClicked(): UIEvent {0} source is not a RadioButton!", e));
			return;
		}
		FramedRadioButton framedRadioButton = null;
		foreach (FramedRadioButton framedRadioButton2 in this.m_framedRadioButtons)
		{
			if (!(radioButton != framedRadioButton2.m_radioButton))
			{
				framedRadioButton = framedRadioButton2;
				break;
			}
		}
		if (framedRadioButton == null)
		{
			Debug.LogWarning(string.Format("RadioButtonGroup.OnRadioButtonDoubleClicked(): could not find framed radio button for radio button ID {0}", radioButton.GetButtonID()));
			return;
		}
		this.m_buttonDoubleClickedCB(framedRadioButton);
	}

	// Token: 0x04005F66 RID: 24422
	public GameObject m_buttonContainer;

	// Token: 0x04005F67 RID: 24423
	public FramedRadioButton m_framedRadioButtonPrefab;

	// Token: 0x04005F68 RID: 24424
	public GameObject m_firstRadioButtonBone;

	// Token: 0x04005F69 RID: 24425
	private List<FramedRadioButton> m_framedRadioButtons = new List<FramedRadioButton>();

	// Token: 0x04005F6A RID: 24426
	private RadioButtonGroup.DelButtonSelected m_buttonSelectedCB;

	// Token: 0x04005F6B RID: 24427
	private RadioButtonGroup.DelButtonDoubleClicked m_buttonDoubleClickedCB;

	// Token: 0x04005F6C RID: 24428
	private Vector3 m_spacingFudgeFactor = Vector3.zero;

	// Token: 0x02000F5D RID: 3933
	public struct ButtonData
	{
		// Token: 0x060074D2 RID: 29906 RVA: 0x00228244 File Offset: 0x00226444
		public ButtonData(int id, string text, object userData, bool selected)
		{
			this.m_id = id;
			this.m_text = text;
			this.m_userData = userData;
			this.m_selected = selected;
		}

		// Token: 0x04005F6D RID: 24429
		public int m_id;

		// Token: 0x04005F6E RID: 24430
		public string m_text;

		// Token: 0x04005F6F RID: 24431
		public bool m_selected;

		// Token: 0x04005F70 RID: 24432
		public object m_userData;
	}

	// Token: 0x02000F5E RID: 3934
	// (Invoke) Token: 0x060074D4 RID: 29908
	public delegate void DelButtonSelected(int buttonID, object userData);

	// Token: 0x02000F5F RID: 3935
	// (Invoke) Token: 0x060074D8 RID: 29912
	public delegate void DelButtonDoubleClicked(FramedRadioButton framedRadioButton);
}
