using System;
using UnityEngine;

// Token: 0x02000B33 RID: 2867
public class MATSample : MonoBehaviour
{
	// Token: 0x06006206 RID: 25094 RVA: 0x001D24DF File Offset: 0x001D06DF
	private void Awake()
	{
	}

	// Token: 0x06006207 RID: 25095 RVA: 0x001D24E4 File Offset: 0x001D06E4
	private void OnGUI()
	{
		GUIStyle guistyle = new GUIStyle();
		guistyle.fontStyle = 1;
		guistyle.fontSize = 50;
		guistyle.alignment = 4;
		guistyle.normal.textColor = Color.white;
		GUI.Label(new Rect(10f, 5f, (float)(Screen.width - 20), (float)(Screen.height / 10)), "MAT Unity Test App", guistyle);
		GUI.skin.button.fontSize = 40;
		if (GUI.Button(new Rect(10f, (float)(Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Start MAT"))
		{
			MonoBehaviour.print("Start MAT clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(2 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Set Delegate"))
		{
			MonoBehaviour.print("Set Delegate clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(3 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Enable Debug Mode"))
		{
			MonoBehaviour.print("Enable Debug Mode clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(4 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Allow Duplicates"))
		{
			MonoBehaviour.print("Allow Duplicates clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(5 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Measure Session"))
		{
			MonoBehaviour.print("Measure Session clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(6 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Measure Event"))
		{
			MonoBehaviour.print("Measure Event clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(7 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Measure Event With Event Items"))
		{
			MonoBehaviour.print("Measure Event With Event Items clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(8 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Test Setter Methods"))
		{
			MonoBehaviour.print("Test Setter Methods clicked");
		}
		else if (GUI.Button(new Rect(10f, (float)(9 * Screen.height / 10), (float)(Screen.width - 20), (float)(Screen.height / 10)), "Test Getter Methods"))
		{
			MonoBehaviour.print("Test Getter Methods clicked");
		}
	}

	// Token: 0x06006208 RID: 25096 RVA: 0x001D27C1 File Offset: 0x001D09C1
	public static string getSampleiTunesIAPReceipt()
	{
		return "dGhpcyBpcyBhIHNhbXBsZSBpb3MgYXBwIHN0b3JlIHJlY2VpcHQ=";
	}
}
