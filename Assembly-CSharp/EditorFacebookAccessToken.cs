using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

// Token: 0x02000B20 RID: 2848
public class EditorFacebookAccessToken : MonoBehaviour
{
	// Token: 0x0600617C RID: 24956 RVA: 0x001D1424 File Offset: 0x001CF624
	private IEnumerator Start()
	{
		if (EditorFacebookAccessToken.fbSkin != null)
		{
			yield break;
		}
		string downloadUrl = IntegratedPluginCanvasLocation.FbSkinUrl;
		WWW www = new WWW(downloadUrl);
		yield return www;
		if (www.error != null)
		{
			FbDebugOverride.Error("Could not find the Facebook Skin: " + www.error);
			yield break;
		}
		EditorFacebookAccessToken.fbSkin = (www.assetBundle.mainAsset as GUISkin);
		www.assetBundle.Unload(false);
		yield break;
	}

	// Token: 0x0600617D RID: 24957 RVA: 0x001D1438 File Offset: 0x001CF638
	private void OnGUI()
	{
		float num = (float)(Screen.height / 2) - this.windowHeight / 2f;
		float num2 = (float)(Screen.width / 2) - 296f;
		if (EditorFacebookAccessToken.fbSkin != null)
		{
			GUI.skin = EditorFacebookAccessToken.fbSkin;
			this.greyButton = EditorFacebookAccessToken.fbSkin.GetStyle("greyButton");
		}
		else
		{
			this.greyButton = GUI.skin.button;
		}
		GUI.ModalWindow(this.GetHashCode(), new Rect(num2, num, 592f, this.windowHeight), new GUI.WindowFunction(this.OnGUIDialog), "Unity Editor Facebook Login");
	}

	// Token: 0x0600617E RID: 24958 RVA: 0x001D14E0 File Offset: 0x001CF6E0
	private void OnGUIDialog(int windowId)
	{
		GUI.enabled = !this.isLoggingIn;
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(10f);
		GUILayout.Label("User Access Token:", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		this.accessToken = GUILayout.TextField(this.accessToken, GUI.skin.textArea, new GUILayoutOption[]
		{
			GUILayout.MinWidth(400f)
		});
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Find Access Token", new GUILayoutOption[0]))
		{
			Application.OpenURL(string.Format("https://developers.facebook.com/tools/accesstoken/?app_id={0}", FB.AppId));
		}
		GUILayout.FlexibleSpace();
		GUIContent guicontent = new GUIContent("Login");
		Rect rect = GUILayoutUtility.GetRect(guicontent, GUI.skin.button);
		if (GUI.Button(rect, guicontent))
		{
			EditorFacebook component = FBComponentFactory.GetComponent<EditorFacebook>(0);
			component.AccessToken = this.accessToken;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["batch"] = "[{\"method\":\"GET\", \"relative_url\":\"me?fields=id\"},{\"method\":\"GET\", \"relative_url\":\"app?fields=id\"}]";
			dictionary["method"] = "POST";
			dictionary["access_token"] = this.accessToken;
			FB.API("/", HttpMethod.GET, new FacebookDelegate(component.MockLoginCallback), dictionary);
			this.isLoggingIn = true;
		}
		GUI.enabled = true;
		GUIContent guicontent2 = new GUIContent("Cancel");
		Rect rect2 = GUILayoutUtility.GetRect(guicontent2, this.greyButton);
		if (GUI.Button(rect2, guicontent2, this.greyButton))
		{
			FBComponentFactory.GetComponent<EditorFacebook>(0).MockCancelledLoginCallback();
			Object.Destroy(this);
		}
		GUILayout.EndHorizontal();
		if (Event.current.type == 7)
		{
			this.windowHeight = rect2.y + rect2.height + (float)GUI.skin.window.padding.bottom;
		}
	}

	// Token: 0x040048BA RID: 18618
	private const float windowWidth = 592f;

	// Token: 0x040048BB RID: 18619
	private float windowHeight = 200f;

	// Token: 0x040048BC RID: 18620
	private string accessToken = string.Empty;

	// Token: 0x040048BD RID: 18621
	private bool isLoggingIn;

	// Token: 0x040048BE RID: 18622
	private static GUISkin fbSkin;

	// Token: 0x040048BF RID: 18623
	private GUIStyle greyButton;
}
