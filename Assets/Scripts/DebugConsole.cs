using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugConsole : MonoBehaviour {
	private Text text;

	void Start () {
		text = GetComponent<Text>();
		text.text = "";
		Application.logMessageReceived += LogMessage;
	}

	void LogMessage(string message, string stackTrace, LogType type) {
		string color;

        switch(type) {
            case LogType.Log:
                color = "white";
                break;
            case LogType.Warning:
                color = "yellow";
                break;
            default:
                color = "red";
                break;
        }

        text.text += string.Format("\n<color={0}>{1}</color>", color, message);
	}
}
