using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WLoading_Element : MonoBehaviour {
    Image[] _graphicList;
    Transform _targetObject;
    float _followSpeed = 1;

    float _angleBetween2Elements;
    Color _targetColor;

    public void ResetAnimation(Transform target, float angleBetween2Elements, float followSpeed){
        _graphicList = GetComponentsInChildren<Image>(true);
        _targetColor = GetColor();

        _targetObject = target;
        _followSpeed = followSpeed;
        _angleBetween2Elements = angleBetween2Elements;
    }

    public void SetTargetColor(Color targetColor){
        _targetColor = targetColor;
    }

    public void SetColor(Color toColor){
        for(int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
    }

    Color GetColor(){
        Color returnColor = Color.white;
        if(_graphicList.Length > 0){
            returnColor = _graphicList[0].color;
        }

        return returnColor;
    }

    public void UpdateAnimation()
    {
		Quaternion _targetRotation = Quaternion.AngleAxis(_targetObject.rotation.eulerAngles.z + _angleBetween2Elements, Vector3.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, _followSpeed * Time.deltaTime);

        Color toColor = Color.Lerp(GetColor(), _targetColor, _followSpeed * Time.deltaTime);
        SetColor(toColor);
	}
}