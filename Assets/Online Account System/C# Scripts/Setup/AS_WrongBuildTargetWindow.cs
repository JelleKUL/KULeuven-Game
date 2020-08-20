#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;


public class AS_WrongBuildTargetWindow :EditorWindow {

	public static void ShowWindow(AS_SetupWindow caller)
	{
		//Show existing window instance. If one doesn't exist, make one.
		AS_WrongBuildTargetWindow window = (AS_WrongBuildTargetWindow) EditorWindow.GetWindow(typeof(AS_WrongBuildTargetWindow));

		window.minSize = new Vector2 (460, 130);
		window.name = "Web player platform active";
#if UNITY_4 || UNITY_5 || UNITY_6
        window.titleContent.text = "Web player platform active";
#else
		window.title = "Web player platform active";
#endif

		window.caller = caller;
		window.position = new Rect (600,	400,  	window.minSize.x, window.minSize.y);        
		
	}

	public AS_SetupWindow caller;
	public AS_SetupState stateToLoad;
	
	
	void OnGUI()
	{
		GUILayout.Label ("Web player platform active", EditorStyles.boldLabel);
		GUILayout.Label ("You are currently using the Web Player platform." +
		                 "\n\nTo use the Online Account System window please switch platform to\n" +
		                 "PC, Mac and Linux Standalone from File->Build Settings..");
		GUILayout.Label ("", GUILayout.Height(8));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ( "Switch my Active Platform", GUILayout.Width(300))){

			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, EditorUserBuildSettings.selectedStandaloneTarget);
			
			if (caller != null){

				caller.Repaint();
				caller.Show();
				
			}
			this.Close ();	
		}
		if (GUILayout.Button ( "Cancel", GUILayout.Width(100))){
			this.Close ();	
			if (caller != null)
					caller.Close();
		}
		GUILayout.EndHorizontal();

		
	}

}
#endif