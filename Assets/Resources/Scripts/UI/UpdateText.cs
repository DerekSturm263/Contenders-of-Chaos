using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

[Serializable]
public class UpdateEvent : UnityEvent<dynamic> { }

public class UpdateText : MonoBehaviour
{
    public UpdateEvent listener;
    private TMP_Text text;
    /*
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
       listener.AddListener(UpdateTextObject);
    }

    private void UpdateTextObject(dynamic value)
    {
        text.text = value.ToString();
        Debug.Log("Value changed to " + value.ToString());
    }*/
}
