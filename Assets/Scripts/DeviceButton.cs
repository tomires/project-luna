using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class DeviceButton : MonoBehaviour
{
    [SerializeField] private Text serverLabel;
    [SerializeField] private Button button;

    public void Initialize(Uri uri, Action<Uri> onSelect)
    {
        serverLabel.text = uri.ToString();
        button.onClick.AddListener(() => onSelect.Invoke(uri));
    }
}
