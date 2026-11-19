using UnityEngine;

[CreateAssetMenu(fileName = "NetworkData", menuName = "Scriptable Objects/NetworkData")]
public class NetworkData : ScriptableObject
{
    public string port;
    public string address;
}
