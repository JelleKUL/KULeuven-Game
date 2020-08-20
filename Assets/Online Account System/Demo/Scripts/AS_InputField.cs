using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Helper class attached to an Input Field to provide callbacks to the AS_CanvasUI
/// </summary>
public class AS_InputField : MonoBehaviour {
	 
	public Text nameText;
	public InputField inputField;
    public Image background;

	// We'll be editing this directly (it's a reference)
	AS_MySQLField field;

	public void Initialize(AS_MySQLField field)
	{ 
		this.field = field;
		nameText.text = (field.isRequired ? "<b>" :"" )+ field.name.UppercaseFirst() + (field.isRequired ? " *</b>": "") + ":"; 
		inputField.onValueChanged.AddListener (OnValueChanged);
		if (field.name.ToLower ().Contains ("password"))
			inputField.contentType = InputField.ContentType.Password;
		else if (field.name.ToLower ().Contains ("email"))
			inputField.contentType = InputField.ContentType.EmailAddress;
		else
			inputField.contentType = InputField.ContentType.Alphanumeric;
	}
	 
	void OnValueChanged (string newVal)
	{ 
		field.stringValue = newVal;
	} 
}
