using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DemoSceneController : MonoBehaviour {
	public static GameObject _demoSceneController;

	int _currentSceneIndex = -1;
	public string[] _sceneList;

	// Use this for initialization
	void Start () {
		if (!_demoSceneController) {
			_demoSceneController = gameObject;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	public void Next(){
		if (_sceneList.Length == 0) {
			return;
		}

		_currentSceneIndex++;
		if (_currentSceneIndex >= _sceneList.Length) {
			_currentSceneIndex = 0;
		}

		UnityEngine.SceneManagement.SceneManager.LoadScene (_sceneList [_currentSceneIndex]);
	}

	public void Previous(){
		if (_sceneList.Length == 0) {
			return;
		}

		_currentSceneIndex--;
		if (_currentSceneIndex < 0) {
			_currentSceneIndex = _sceneList.Length - 1;
		}

		UnityEngine.SceneManagement.SceneManager.LoadScene (_sceneList [_currentSceneIndex]);
	}
}
