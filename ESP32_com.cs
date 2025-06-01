using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ESP32_com : MonoBehaviour
{
    public string portName = "COM12";
    public int baudRate = 115200;
    public KeyCode reconnectKey = KeyCode.F2; 
    public bool showPortMenu = false;

    private SerialPort stream;
    private Thread serialThread;
    private volatile bool keepReading = true;
    private string[] availablePorts;
    private Vector2 scrollPosition;
    private Rect portMenuRect = new Rect(220, 10, 200, 300);

    private string latestLine = "";
    private readonly object dataLock = new object();

    public static float qw, qx, qy, qz, serce, yuzuk, orta, isaret, bas;

    void Start()
    {
        RefreshPortList();
        InitializeSerialPort();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        string dataLine = "";
        lock (dataLock)
        {
            dataLine = latestLine;
        }

        if (!string.IsNullOrEmpty(dataLine))
        {
            string[] strData = dataLine.Split(',');
            if (strData.Length == 9)
            {
                try
                {
                    qw = float.Parse(strData[0]);
                    qx = float.Parse(strData[1]);
                    qy = float.Parse(strData[2]);
                    qz = float.Parse(strData[3]);

                    serce = float.Parse(strData[4]);
                    yuzuk = float.Parse(strData[5]);
                    orta = float.Parse(strData[6]);
                    isaret = float.Parse(strData[7]);
                    bas = float.Parse(strData[8]);
                }
                catch (FormatException ex)
                {
                    Debug.LogWarning("Parse error: " + ex.Message);
                }
            }
        }
    }

    void ReadSerial() 
    {
        while (keepReading)
        {
            try
            {
                string line = stream.ReadLine();
                lock (dataLock)
                {
                    latestLine = line;
                }
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                Debug.LogWarning("Serial Exception: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        keepReading = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }

        if (stream.IsOpen)
            stream.Close();
    }

    void RefreshPortList()
    {
        availablePorts = SerialPort.GetPortNames();
    }

    void ReconnectToPort()
    {
        keepReading = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }

        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }

        keepReading = true;
        InitializeSerialPort();
    }

    void InitializeSerialPort()
    {
        stream = new SerialPort(portName, baudRate);
        stream.Open();
        stream.ReadTimeout = 1000;

        serialThread = new Thread(ReadSerial);
        serialThread.Start();
    }

    void OnGUI()
    {
        if (showPortMenu)
        {
            GUI.skin.window.fontSize = 14;
            GUI.skin.button.fontSize = 14;
            GUI.skin.label.fontSize = 14;
            
            portMenuRect = GUI.Window(1, portMenuRect, DrawPortMenu, "ESP32 Port Seçimi");
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
                ReconnectToPort();
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
}