using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCounters : MonoBehaviour {
	
    // TODO remove this method
	public void OnEpidemicCountClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
