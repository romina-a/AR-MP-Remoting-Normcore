using UnityEngine;
using UnityEngine.Animations;
using Normal.Realtime;
using static Normal.Realtime.Realtime;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.perceptlab.armultiplayer
{
    [RequireComponent(typeof(Realtime))]
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField]
        private string roomName = "MyRoomName";
        private Realtime _realtime;
        [SerializeField] Transform World = default;
        [SerializeField] List<string> interactablePrefabNames;
        private string originPrefabName = "OriginRT";

        private void Awake()
        {
            // Get the Realtime component on this game object
            _realtime = GetComponent<Realtime>();

            // Notify us when Realtime successfully connects to the room
            _realtime.didConnectToRoom += DidConnectToRoom;
        }

        public void Connect()
        {
            RLogger.Log("NetworkManager calling _realtime.Connect");
            _realtime.Connect(roomName);
            RLogger.Log("NetworkManger _realtime.Connect called and finished");
        }

        private void DidConnectToRoom(Realtime realtime)
        {
            RLogger.Log("NetworkManager DidConnectToRoom called and client id is " + _realtime.room.clientID);
            if (_realtime.room.clientID == 0)
                instantiateObjects(realtime);

            instantiaceOrigin(realtime);
            
        }

        void instantiateObjects(Realtime realtime)
        {
            // Instantiate the Player for this client once we've successfully connected to the room
            Realtime.InstantiateOptions options = new InstantiateOptions
            {
                ownedByClient = true,
                preventOwnershipTakeover = false,
                destroyWhenOwnerLeaves = false,
                destroyWhenLastClientLeaves = true,
                useInstance = realtime
            };

            float x_offset = 0.1f;
            float x_initial = -0.3f;
            int i = 0;
            foreach (string name in interactablePrefabNames)
            {
                GameObject cube = Realtime.Instantiate(prefabName: name, position: Vector3.zero, rotation: Quaternion.identity, options);
                if (World != null)
                {
                    cube.transform.SetParent(World);
                }
                RealtimeTransform realtimeTransform = cube.GetComponent<RealtimeTransform>();
                cube.transform.localPosition = new Vector3(x_initial + x_offset * i, 0.77f, 0.5f);
                i += 1;
                cube.transform.localRotation = Quaternion.identity;
                realtimeTransform.RequestOwnership();
            }
        }

        void instantiaceOrigin(Realtime realtime)
        {
            GameObject origin = Realtime.Instantiate(
                prefabName: originPrefabName, Vector3.zero, Quaternion.identity,
                new InstantiateOptions
                {
                    ownedByClient = true,
                    preventOwnershipTakeover = true,
                    destroyWhenOwnerLeaves = true,
                    destroyWhenLastClientLeaves = true,
                    useInstance = realtime
                });
        }

    }
}