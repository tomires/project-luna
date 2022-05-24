using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class IpDisplay : MonoBehaviour
{
    void Start()
    {
        if (!Initializer.Instance.supervisor)
            GetComponent<Text>().text = GetLocalIPv4();
    }

    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }
}
