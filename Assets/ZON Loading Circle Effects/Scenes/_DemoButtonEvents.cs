using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DemoButtonEvents : MonoBehaviour {
	_DemoSceneController _sceneController;
	void Start(){
		_sceneController = GameObject.FindObjectOfType <_DemoSceneController>();
        if (!_sceneController) {
            gameObject.SetActive(false);
        }
	}

	public void Next(){
		if (_sceneController) {
			_sceneController.Next ();
		}
	}

	public void Previous(){
		if (_sceneController) {
			_sceneController.Previous();
		}
	}
}
