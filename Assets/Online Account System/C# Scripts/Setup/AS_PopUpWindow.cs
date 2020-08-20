#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

public class AS_PopUpWindow : EditorWindow
{
	public static void ShowWindow(string name, string _message, AS_SetupWindow caller = null, AS_SetupState _stateToLoad = AS_SetupState.CredentialsAndPreferences)
	{
		//Show existing window instance. If one doesn't exist, make one.
		AS_PopUpWindow window = (AS_PopUpWindow) EditorWindow.GetWindow(typeof(AS_PopUpWindow));
		
		window.minSize = new Vector2 (460, 110);
		window.name = name;
#if UNITY_4 || UNITY_5 || UNITY_6
		window.titleContent.text = name;
#else
		window.title = name;
#endif
		window.caller = caller;
		window.stateToLoad = _stateToLoad;
		window.position = new Rect (600,	400,  	window.minSize.x, window.minSize.y);        
		window.message = _message;   

	}

	public string message = "";
	public AS_SetupWindow caller;
	public AS_SetupState stateToLoad;


	void OnGUI()
	{

		GUI.Label (new Rect (10, 10, 450, 100), message);

		if (GUI.Button (new Rect (this.position.width / 2 - 50, this.position.height - 40, 100, 25), "OK")) {
			
			this.Close ();	
			if (caller != null){
				if (stateToLoad == AS_SetupState.Done)
					caller.Close();
				else{
					caller.state = stateToLoad;
					caller.Repaint();
					caller.Show();
				}
			}

		}

	}

}
#endif