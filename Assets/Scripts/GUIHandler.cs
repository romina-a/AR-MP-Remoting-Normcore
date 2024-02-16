using com.perceptlab.armultiplayer;
using Microsoft.MixedReality.OpenXR.Remoting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class GUIHandler : MonoBehaviour
{

    private bool remotingConnected = false;
    private bool aligned = false;
    [SerializeField]
    private HolographicRemotingConnectionHandler remotingHandler;
    [SerializeField]
    private AlignTheWorld align;
    
    [SerializeField, Tooltip("Default IP to use to connect with Holographic Remoting. Must have a HolographicRemoteConnect Component to Connect")]
    string IP = "192.168.0.103";
    string disconnectReason = "";


    private void Start()
    {
        if (remotingHandler != null)
        {
            remotingHandler.onConnectedToDevice.AddListener(delegate () { remotingConnected = true;});
            remotingHandler.onDisconnectedFromDevice.AddListener(delegate (DisconnectReason reason) { disconnectReason = reason.ToString();  remotingConnected = false;});
        }
        align.onDoneAlign.AddListener(()=> { aligned = true; });

    }

    // Start is called before the first frame update
    private void OnGUI()
    {
        if (remotingHandler != null)
        {
            RemotingInput();
        }
        if (aligned == false)
        {
            Align();
        }
    }

    private void RemotingInput()
    {
        GUI.Label(new Rect(10, 10, 150, 30), "Player's IP:");
        if (disconnectReason != "")
            GUI.Label(new Rect(10, 40, 400, 30), "Disconnect reason: "+ disconnectReason);
        if (!remotingConnected)
        {
            IP = GUI.TextField(new Rect(155, 10, 200, 30), IP, 25);
            if (GUI.Button(new Rect(365, 10, 100, 30), "Connect"))
            {
                GUI.Label(new Rect(365, 10, 100, 30), "Connecting...");
                remotingHandler.BlockingConnect(IP);
            }
        }
        else
        {
            GUI.Label(new Rect(215, 10, 200, 30), IP);
            if (GUI.Button(new Rect(430, 10, 100, 30), "Disconnect"))
            {
                remotingHandler.Disconnect();
            }

        }
    }

    private void Align()
    {
        GUI.Label(new Rect(10, 80, 400, 60), "Align the world. When finished, press 'start' on joystick or 'enter' on keyboard.\nUse a,w,s,d, and arrows on keyboard or the joystick to align." );
    }

}
