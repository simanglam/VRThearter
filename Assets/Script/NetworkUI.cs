
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class NetworkUI : MonoBehaviour
{
    [Header("UI")]
    public Button hostButton;
    public Button clientButton;
    public Button disconnectButton;

    public GameObject disconnectButtonObject;
    public GameObject connectUI;

    public TMP_Text statusText;
    public TMP_Text address;
    public TMP_Text port;

    [Header("Network Settings")]
    public ushort defaultPort = 7777;
    public string defaultAddress = "127.0.0.1";

    private NetworkManager nm;
    private UnityTransport transport;

    private int playerInWaiting;

    private void Start()
    {
        nm = NetworkManager.Singleton;

        if (nm == null)
        {
            Debug.LogError("[NetworkUI] NetworkManager not found!");
            return;
        }

        transport = nm.GetComponent<UnityTransport>();

        if (transport == null)
        {
            Debug.LogError("[NetworkUI] UnityTransport not found!");
            return;
        }

        // UI initialization
        disconnectButtonObject.SetActive(false);
        connectUI.SetActive(true);

        // Button listeners
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        disconnectButton.onClick.AddListener(Disconnect);

        // Network callbacks
        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;
        nm.OnTransportFailure += OnTransportFailure;

        Debug.Log(
            $"[NetworkUI] Initialized. " +
            $"Instance ID: {GetEntityId()}, " +
            $"Process ID: {System.Diagnostics.Process.GetCurrentProcess().Id}"
        );

        statusText.text = "Ready";
    }

    // -------------------------
    // HOST
    // -------------------------

    public void StartHost()
    {
        if (nm == null || nm.IsListening)
            return;

        Debug.Log("[HOST] Starting...");

        transport.SetConnectionData(
            defaultAddress,
            defaultPort,
            "0.0.0.0"
        );

        Debug.Log(
            $"[HOST] Address: {defaultAddress}, " +
            $"Port: {defaultPort}"
        );

        bool result = nm.StartHost();

        Debug.Log($"[HOST] StartHost result: {result}");
        Debug.Log($"[HOST] IsListening: {nm.IsListening}");

        if (result)
        {
            statusText.text = "Host Started";

            connectUI.SetActive(false);
            disconnectButtonObject.SetActive(true);

            UpdatePlayerCount();
        }
        else
        {
            statusText.text = "Host Failed";
        }
    }

    // -------------------------
    // CLIENT
    // -------------------------

    public void StartClient()
    {
        if (nm == null || nm.IsListening)
            return;

        string targetAddress = defaultAddress;

        if (address != null &&
            !string.IsNullOrWhiteSpace(address.text))
        {
            targetAddress = address.text.Trim();
        }

        Debug.Log("[CLIENT] Starting...");

        transport.SetConnectionData(
            true,
            targetAddress,
            defaultPort
        );

        Debug.Log(
            $"[CLIENT] Target: {targetAddress}:{defaultPort}"
        );

        statusText.text = "Connecting...";

        bool result = nm.StartClient();

        Debug.Log($"[CLIENT] StartClient result: {result}");
        Debug.Log($"[CLIENT] IsListening: {nm.IsListening}");

        if (result)
        {
            connectUI.SetActive(false);
            disconnectButtonObject.SetActive(true);
        }
        else
        {
            statusText.text = "Client Start Failed";

            connectUI.SetActive(true);
            disconnectButtonObject.SetActive(false);
        }
    }

    // -------------------------
    // DISCONNECT
    // -------------------------

    public void Disconnect()
    {
        Debug.Log("[NETWORK] Shutdown requested.");

        if (nm != null && nm.IsListening)
        {
            nm.Shutdown();
        }

        statusText.text = "Disconnected";

        disconnectButtonObject.SetActive(false);
        connectUI.SetActive(true);

        playerInWaiting = 0;
    }

    // -------------------------
    // CALLBACKS
    // -------------------------

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[NETWORK] Client Connected: {clientId}");

        UpdatePlayerCount();

        if (clientId == nm.LocalClientId)
        {
            statusText.text = nm.IsHost
                ? "Host Connected"
                : "Connected!";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[NETWORK] Client Disconnected: {clientId}");

        UpdatePlayerCount();

        if (clientId == nm.LocalClientId)
        {
            Debug.LogError(
                $"[CLIENT] Disconnect Reason: {nm.DisconnectReason}"
            );

            statusText.text = "Disconnected";

            connectUI.SetActive(true);
            disconnectButtonObject.SetActive(false);
        }
    }

    private void OnTransportFailure()
    {
        Debug.LogError("[TRANSPORT] Transport Failure!");

        statusText.text = "Transport Failure";

        connectUI.SetActive(true);
        disconnectButtonObject.SetActive(false);
    }

    // -------------------------
    // PLAYER COUNT
    // -------------------------

    private void UpdatePlayerCount()
    {
        if (nm == null || !nm.IsListening)
            return;

        playerInWaiting = nm.ConnectedClients.Count;

        statusText.text = "Players: " + playerInWaiting;

        Debug.Log(
            $"[NETWORK] Connected Players: {playerInWaiting}"
        );
    }

    // -------------------------
    // CLEANUP
    // -------------------------

    private void OnDestroy()
    {
        if (hostButton != null)
            hostButton.onClick.RemoveListener(StartHost);

        if (clientButton != null)
            clientButton.onClick.RemoveListener(StartClient);

        if (disconnectButton != null)
            disconnectButton.onClick.RemoveListener(Disconnect);

        if (nm != null)
        {
            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectCallback -= OnClientDisconnected;
            nm.OnTransportFailure -= OnTransportFailure;
        }
    }
}
