using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class TextField : PegUIElement
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x060038A6 RID: 14502 RVA: 0x00114E5B File Offset: 0x0011305B
	// (remove) Token: 0x060038A7 RID: 14503 RVA: 0x00114E74 File Offset: 0x00113074
	public event Action<Event> Preprocess;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x060038A8 RID: 14504 RVA: 0x00114E8D File Offset: 0x0011308D
	// (remove) Token: 0x060038A9 RID: 14505 RVA: 0x00114EA6 File Offset: 0x001130A6
	public event Action<string> Changed;

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x060038AA RID: 14506 RVA: 0x00114EBF File Offset: 0x001130BF
	// (remove) Token: 0x060038AB RID: 14507 RVA: 0x00114ED8 File Offset: 0x001130D8
	public event Action<string> Submitted;

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x060038AC RID: 14508 RVA: 0x00114EF1 File Offset: 0x001130F1
	// (remove) Token: 0x060038AD RID: 14509 RVA: 0x00114F0A File Offset: 0x0011310A
	public event Action Canceled;

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x060038AE RID: 14510 RVA: 0x00114F23 File Offset: 0x00113123
	// (set) Token: 0x060038AF RID: 14511 RVA: 0x00114F2A File Offset: 0x0011312A
	public static Rect KeyboardArea { get; private set; }

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x060038B0 RID: 14512 RVA: 0x00114F32 File Offset: 0x00113132
	public bool Active
	{
		get
		{
			return this == TextField.instance;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x060038B1 RID: 14513 RVA: 0x00114F3F File Offset: 0x0011313F
	// (set) Token: 0x060038B2 RID: 14514 RVA: 0x00114F46 File Offset: 0x00113146
	public string Text
	{
		get
		{
			return TextField.GetTextFieldText();
		}
		set
		{
			TextField.SetTextFieldText(value);
		}
	}

	// Token: 0x060038B3 RID: 14515 RVA: 0x00114F50 File Offset: 0x00113150
	protected override void Awake()
	{
		base.Awake();
		base.gameObject.name = string.Format("TextField_{0:000}", TextField.nextId++);
		if (base.gameObject.GetComponent<BoxCollider>() == null)
		{
			base.gameObject.AddComponent<BoxCollider>();
		}
		this.UpdateCollider();
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x00114FCC File Offset: 0x001131CC
	private void OnDestroy()
	{
		if (this.Active)
		{
			this.Deactivate();
		}
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x00115004 File Offset: 0x00113204
	private void Update()
	{
		if (this.lastTopLeft != this.inputTopLeft.position || this.lastBottomRight != this.inputBottomRight.position)
		{
			this.UpdateCollider();
			this.lastTopLeft = this.inputTopLeft.position;
			this.lastBottomRight = this.inputBottomRight.position;
		}
		if (!this.Active)
		{
			return;
		}
		Rect rect = this.ComputeBounds();
		if (rect != this.lastBounds)
		{
			this.lastBounds = rect;
			TextField.SetTextFieldBounds(rect);
		}
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x001150A5 File Offset: 0x001132A5
	public void SetInputFont(Font font)
	{
		this.m_InputFont = font;
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x001150B0 File Offset: 0x001132B0
	public void Activate()
	{
		Log.Cameron.Print("TextField::Activate", new object[0]);
		if (TextField.instance != null && TextField.instance != this)
		{
			TextField.instance.Deactivate();
		}
		TextField.instance = this;
		this.lastBounds = this.ComputeBounds();
		TextField.KeyboardArea = TextField.ActivateTextField(base.gameObject.name, this.lastBounds, (!this.autocorrect) ? 0 : 1, (uint)this.keyboardType, (uint)this.returnKeyType);
		TextField.SetTextFieldColor(this.textColor.r, this.textColor.g, this.textColor.b, this.textColor.a);
		TextField.SetTextFieldMaxCharacters(512);
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x0011518C File Offset: 0x0011338C
	public void Deactivate()
	{
		Log.Cameron.Print("TextField::Deactivate", new object[0]);
		if (this == TextField.instance)
		{
			TextField.KeyboardArea = default(Rect);
			TextField.DeactivateTextField();
			TextField.instance = null;
		}
	}

	// Token: 0x060038B9 RID: 14521 RVA: 0x001151D7 File Offset: 0x001133D7
	protected override void OnRelease()
	{
		if (this.Active)
		{
			return;
		}
		this.Activate();
	}

	// Token: 0x060038BA RID: 14522 RVA: 0x001151EB File Offset: 0x001133EB
	private bool OnPreprocess(Event e)
	{
		if (this.Preprocess != null)
		{
			this.Preprocess.Invoke(e);
		}
		return false;
	}

	// Token: 0x060038BB RID: 14523 RVA: 0x00115205 File Offset: 0x00113405
	private void OnChanged(string text)
	{
		if (this.Changed != null)
		{
			this.Changed.Invoke(text);
		}
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x0011521E File Offset: 0x0011341E
	private void OnSubmitted(string text)
	{
		if (this.Submitted != null)
		{
			this.Submitted.Invoke(text);
		}
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x00115237 File Offset: 0x00113437
	private void OnCanceled()
	{
		if (this.Canceled != null)
		{
			this.Canceled.Invoke();
		}
		this.Deactivate();
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x00115255 File Offset: 0x00113455
	private void OnKeyboardAreaChanged(Rect area)
	{
		TextField.KeyboardArea = area;
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x0011525D File Offset: 0x0011345D
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.Deactivate();
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x00115268 File Offset: 0x00113468
	private void UpdateCollider()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		Vector3 vector = base.transform.InverseTransformPoint(this.inputTopLeft.transform.position);
		Vector3 vector2 = base.transform.InverseTransformPoint(this.inputBottomRight.transform.position);
		component.center = (vector + vector2) / 2f;
		component.size = VectorUtils.Abs(vector2 - vector);
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x001152E0 File Offset: 0x001134E0
	private Rect ComputeBounds()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		Vector2 vector = camera.WorldToScreenPoint(this.inputTopLeft.transform.position);
		Vector2 vector2 = camera.WorldToScreenPoint(this.inputBottomRight.transform.position);
		vector.y = (float)Screen.height - vector.y;
		vector2.y = (float)Screen.height - vector2.y;
		Vector2 vector3 = Vector2.Min(vector, vector2);
		Vector2 vector4 = Vector2.Max(vector, vector2);
		return Rect.MinMaxRect(Mathf.Round(vector3.x), Mathf.Round(vector3.y), Mathf.Round(vector4.x), Mathf.Round(vector4.y));
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x001153A8 File Offset: 0x001135A8
	private static TextField.PluginRect Plugin_ActivateTextField(string name, [MarshalAs(27)] TextField.PluginRect bounds, int autocorrect, uint keyboardType, uint returnKeyType)
	{
		return default(TextField.PluginRect);
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x001153BE File Offset: 0x001135BE
	private static void Plugin_DeactivateTextField()
	{
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x001153C0 File Offset: 0x001135C0
	private static void Plugin_SetTextFieldBounds([MarshalAs(27)] TextField.PluginRect bounds)
	{
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x001153C2 File Offset: 0x001135C2
	private static string Plugin_GetTextFieldText()
	{
		return string.Empty;
	}

	// Token: 0x060038C6 RID: 14534 RVA: 0x001153C9 File Offset: 0x001135C9
	private static void Plugin_SetTextFieldText(string text)
	{
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x001153CB File Offset: 0x001135CB
	private static void Plugin_SetTextFieldColor(float r, float g, float b, float a)
	{
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x001153CD File Offset: 0x001135CD
	private static void Plugin_SetTextFieldMaxCharacters(int maxCharacters)
	{
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x001153CF File Offset: 0x001135CF
	private void Unity_TextInputChanged(string text)
	{
		if (this.Active)
		{
			this.OnChanged(text);
		}
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x001153E3 File Offset: 0x001135E3
	private void Unity_TextInputSubmitted(string text)
	{
		if (this.Active)
		{
			this.OnSubmitted(text);
		}
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x001153F7 File Offset: 0x001135F7
	private void Unity_TextInputCanceled(string unused)
	{
		if (this.Active)
		{
			this.OnCanceled();
		}
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x0011540C File Offset: 0x0011360C
	private void Unity_KeyboardAreaChanged(string rectString)
	{
		if (this.Active)
		{
			Match match = Regex.Match(rectString, string.Format("x\\: (?<x>{0})\\, y\\: (?<y>{0})\\, width\\: (?<width>{0})\\, height\\: (?<height>{0})", "[-+]?[0-9]*\\.?[0-9]+"));
			Rect area;
			area..ctor(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value), float.Parse(match.Groups["width"].Value), float.Parse(match.Groups["height"].Value));
			this.OnKeyboardAreaChanged(area);
		}
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x001154B0 File Offset: 0x001136B0
	private static bool UseNativeKeyboard()
	{
		return false;
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x001154B4 File Offset: 0x001136B4
	private static TextField.PluginRect ActivateTextField(string name, TextField.PluginRect bounds, int autocorrect, uint keyboardType, uint returnKeyType)
	{
		Log.Cameron.Print(string.Concat(new object[]
		{
			"activate text field ",
			name,
			" ",
			bounds
		}), new object[0]);
		if (TextField.UseNativeKeyboard())
		{
			return TextField.Plugin_ActivateTextField(name, bounds, autocorrect, keyboardType, returnKeyType);
		}
		if (UniversalInputManager.Get() == null)
		{
			return default(TextField.PluginRect);
		}
		TextField.instance.inputParams = new UniversalInputManager.TextInputParams
		{
			m_owner = TextField.instance.gameObject,
			m_preprocessCallback = new UniversalInputManager.TextInputPreprocessCallback(TextField.instance.OnPreprocess),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(TextField.instance.OnSubmitted),
			m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(TextField.instance.OnChanged),
			m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(TextField.instance.InputCanceled),
			m_font = TextField.instance.m_InputFont,
			m_maxCharacters = TextField.instance.maxCharacters,
			m_inputKeepFocusOnComplete = TextField.instance.inputKeepFocusOnComplete,
			m_touchScreenKeyboardHideInput = TextField.instance.hideInput,
			m_useNativeKeyboard = TextField.UseNativeKeyboard()
		};
		UniversalInputManager.Get().UseTextInput(TextField.instance.inputParams, false);
		TextField.SetTextFieldBounds(bounds);
		if (TextField.instance.Active)
		{
			return new Rect(0f, (float)Screen.height, (float)Screen.width, (float)Screen.height * 0.5f);
		}
		return default(TextField.PluginRect);
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x00115648 File Offset: 0x00113848
	private static void DeactivateTextField()
	{
		Log.Cameron.Print("deactivating text field " + TextField.instance.name, new object[0]);
		if (TextField.UseNativeKeyboard())
		{
			TextField.Plugin_DeactivateTextField();
			return;
		}
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().CancelTextInput(TextField.instance.gameObject, false);
		}
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x001156B0 File Offset: 0x001138B0
	private static void SetTextFieldBounds(TextField.PluginRect bounds)
	{
		Log.Cameron.Print("TextField::SetTextFieldBounds " + bounds, new object[0]);
		if (TextField.UseNativeKeyboard())
		{
			TextField.Plugin_SetTextFieldBounds(bounds);
			return;
		}
		bounds.x /= (float)Screen.width;
		bounds.y /= (float)Screen.height;
		bounds.width /= (float)Screen.width;
		bounds.height /= (float)Screen.height;
		if (TextField.instance.inputParams.m_rect != bounds)
		{
			TextField.instance.inputParams.m_rect = bounds;
			TextField.instance.UpdateTextInput();
		}
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x0011577C File Offset: 0x0011397C
	private static string GetTextFieldText()
	{
		if (TextField.UseNativeKeyboard())
		{
			return TextField.Plugin_GetTextFieldText();
		}
		return UniversalInputManager.Get().GetInputText();
	}

	// Token: 0x060038D2 RID: 14546 RVA: 0x00115798 File Offset: 0x00113998
	private static void SetTextFieldText(string text)
	{
		if (TextField.UseNativeKeyboard())
		{
			TextField.Plugin_SetTextFieldText(text);
			return;
		}
		UniversalInputManager.Get().SetInputText(text);
	}

	// Token: 0x060038D3 RID: 14547 RVA: 0x001157B6 File Offset: 0x001139B6
	private static void SetTextFieldColor(float r, float g, float b, float a)
	{
		if (TextField.UseNativeKeyboard())
		{
			TextField.Plugin_SetTextFieldColor(r, g, b, a);
			return;
		}
	}

	// Token: 0x060038D4 RID: 14548 RVA: 0x001157CC File Offset: 0x001139CC
	private static void SetTextFieldMaxCharacters(int maxCharacters)
	{
		if (TextField.UseNativeKeyboard())
		{
			TextField.Plugin_SetTextFieldMaxCharacters(maxCharacters);
			return;
		}
		if (maxCharacters != TextField.instance.maxCharacters)
		{
			TextField.instance.maxCharacters = maxCharacters;
			TextField.instance.UpdateTextInput();
		}
	}

	// Token: 0x060038D5 RID: 14549 RVA: 0x00115810 File Offset: 0x00113A10
	private void UpdateTextInput()
	{
		UniversalInputManager.Get().UseTextInput(TextField.instance.inputParams, true);
		UniversalInputManager.Get().FocusTextInput(TextField.instance.gameObject);
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00115846 File Offset: 0x00113A46
	private void InputCanceled(bool userRequested, GameObject requester)
	{
		this.OnCanceled();
	}

	// Token: 0x0400246F RID: 9327
	public Transform inputTopLeft;

	// Token: 0x04002470 RID: 9328
	public Transform inputBottomRight;

	// Token: 0x04002471 RID: 9329
	public Color textColor;

	// Token: 0x04002472 RID: 9330
	public int maxCharacters;

	// Token: 0x04002473 RID: 9331
	public bool autocorrect;

	// Token: 0x04002474 RID: 9332
	public TextField.KeyboardType keyboardType;

	// Token: 0x04002475 RID: 9333
	public TextField.KeyboardReturnKeyType returnKeyType;

	// Token: 0x04002476 RID: 9334
	public bool hideInput = true;

	// Token: 0x04002477 RID: 9335
	public bool useNativeKeyboard;

	// Token: 0x04002478 RID: 9336
	public bool inputKeepFocusOnComplete;

	// Token: 0x04002479 RID: 9337
	private static uint nextId;

	// Token: 0x0400247A RID: 9338
	private static TextField instance;

	// Token: 0x0400247B RID: 9339
	private Rect lastBounds = default(Rect);

	// Token: 0x0400247C RID: 9340
	private Vector3 lastTopLeft;

	// Token: 0x0400247D RID: 9341
	private Vector3 lastBottomRight;

	// Token: 0x0400247E RID: 9342
	private Font m_InputFont;

	// Token: 0x0400247F RID: 9343
	private UniversalInputManager.TextInputParams inputParams;

	// Token: 0x020004AB RID: 1195
	private struct PluginRect
	{
		// Token: 0x06003928 RID: 14632 RVA: 0x00116B4B File Offset: 0x00114D4B
		public PluginRect(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x00116B6C File Offset: 0x00114D6C
		public override string ToString()
		{
			return string.Format("[x: {0}, y: {1}, width: {2}, height: {3}]", new object[]
			{
				this.x,
				this.y,
				this.width,
				this.height
			});
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x00116BC4 File Offset: 0x00114DC4
		public static implicit operator TextField.PluginRect(Rect rect)
		{
			return new TextField.PluginRect(rect.x, rect.y, rect.width, rect.height);
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x00116BF2 File Offset: 0x00114DF2
		public static implicit operator Rect(TextField.PluginRect rect)
		{
			return new Rect(rect.x, rect.y, rect.width, rect.height);
		}

		// Token: 0x04002490 RID: 9360
		public float x;

		// Token: 0x04002491 RID: 9361
		public float y;

		// Token: 0x04002492 RID: 9362
		public float width;

		// Token: 0x04002493 RID: 9363
		public float height;
	}

	// Token: 0x020004AC RID: 1196
	public enum KeyboardType
	{
		// Token: 0x04002495 RID: 9365
		Default,
		// Token: 0x04002496 RID: 9366
		ASCIICapable,
		// Token: 0x04002497 RID: 9367
		NumbersAndPunctuation,
		// Token: 0x04002498 RID: 9368
		URL,
		// Token: 0x04002499 RID: 9369
		NumberPad,
		// Token: 0x0400249A RID: 9370
		PhonePad,
		// Token: 0x0400249B RID: 9371
		NamePhonePad,
		// Token: 0x0400249C RID: 9372
		EmailAddress,
		// Token: 0x0400249D RID: 9373
		DecimalPad,
		// Token: 0x0400249E RID: 9374
		Twitter
	}

	// Token: 0x020004AD RID: 1197
	public enum KeyboardReturnKeyType
	{
		// Token: 0x040024A0 RID: 9376
		Default,
		// Token: 0x040024A1 RID: 9377
		Go,
		// Token: 0x040024A2 RID: 9378
		Google,
		// Token: 0x040024A3 RID: 9379
		Join,
		// Token: 0x040024A4 RID: 9380
		Next,
		// Token: 0x040024A5 RID: 9381
		Route,
		// Token: 0x040024A6 RID: 9382
		Search,
		// Token: 0x040024A7 RID: 9383
		Send,
		// Token: 0x040024A8 RID: 9384
		Yahoo,
		// Token: 0x040024A9 RID: 9385
		Done,
		// Token: 0x040024AA RID: 9386
		EmergencyCall
	}
}
