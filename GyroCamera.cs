using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections;
using System.Threading;

public class GyroCamera : MonoBehaviour
{
    [Header("Serial Port Settings")]
    public string portName = "COM5";
    public int baudRate = 115200;
    public KeyCode reconnectKey = KeyCode.F1;
    public bool showDebugLogs = false;
    public bool showPortMenu = false;

    private SerialPort serialPort;
    private bool isConnected = false;
    private bool isReading = false;
    private Coroutine readCoroutine;
    private string[] availablePorts;
    private Vector2 scrollPosition;
    private Rect portMenuRect = new Rect(10, 10, 200, 300);

    private Camera mainCamera;

    public static float qw, qx, qy, qz;

    void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        mainCamera = Camera.main;
        RefreshPortList();
        ConnectToArduino();
    }

    void Update()
    {
        if (Input.GetKeyDown(reconnectKey))
        {
            showPortMenu = !showPortMenu;
            if (showPortMenu)
            {
                RefreshPortList();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (isConnected && !showPortMenu)
        {
            mainCamera.transform.rotation = new Quaternion(-qy, -qz, qx, qw);
        }
    }

    void OnGUI()
    {
        if (showPortMenu)
        {
            GUI.skin.window.fontSize = 14;
            GUI.skin.button.fontSize = 14;
            GUI.skin.label.fontSize = 14;
            
            portMenuRect = GUI.Window(0, portMenuRect, DrawPortMenu, "Gyro Port Seçimi");
        }
    }

    void DrawPortMenu(int windowID)
    {
        GUILayout.BeginVertical();
        
        GUILayout.Label("Mevcut Portlar:", GUILayout.Height(30));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        foreach (string port in availablePorts)
        {
            if (GUILayout.Button(port, GUILayout.Height(30)))
            {
                portName = port;
                ConnectToArduino();
                showPortMenu = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        GUILayout.EndScrollView();
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Yenile", GUILayout.Height(30)))
        {
            RefreshPortList();
        }
        
        if (GUILayout.Button("Kapat", GUILayout.Height(30)))
        {
            showPortMenu = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        GUILayout.EndVertical();
        
        GUI.DragWindow();
    }

    void RefreshPortList()
    {
        availablePorts = SerialPort.GetPortNames();
    }

    void ConnectToArduino()
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                DebugLog("Mevcut bağlantı kapatıldı.");
                Thread.Sleep(1000);
            }

            serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 100,
                WriteTimeout = 100,
                DtrEnable = true,
                RtsEnable = true,
                NewLine = "\n"
            };

            serialPort.Open();
            DebugLog("Seri bağlantı açıldı: " + portName);
            isConnected = true;

            if (!isReading)
            {
                isReading = true;
                readCoroutine = StartCoroutine(ReadSerialData());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Bağlantı hatası: " + e.Message);
            isConnected = false;
        }
    }

    IEnumerator ReadSerialData()
    {
        while (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    string data = serialPort.ReadLine();
                    ProcessData(data);
                }
            }
            catch (TimeoutException) { }
            catch (Exception e)
            {
                Debug.LogWarning("Veri okuma hatası: " + e.Message);
                isConnected = false;
                isReading = false;
                break;
            }

            yield return null;
        }

        isReading = false;
        DebugLog("Veri okuma durdu.");
    }

    void ProcessData(string data)
    {
        if (string.IsNullOrEmpty(data)) return;

        string[] values = data.Split(',');

        if (values.Length == 4 &&
            float.TryParse(values[0], out qw) &&
            float.TryParse(values[1], out qx) &&
            float.TryParse(values[2], out qy) &&
            float.TryParse(values[3], out qz))
        {
            DebugLog($"Quaternion -> qw:{qw}, qx:{qx}, qy:{qy}, qz:{qz}");
        }
    }

    void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log("[GyroCamera] " + message);
        }
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    void OnDestroy()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                DebugLog("Seri bağlantı kapatıldı.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Port kapatma hatası: " + e.Message);
        }

        isConnected = false;
    }
}