using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace com.perceptlab.armultiplayer
{
    public class AlignTheWorld : MonoBehaviour
    {
        [Serializable]
        struct ButtonHandler
        {
            public string name;
            public UnityEvent onPressed;
        }

        [SerializeField]
        GameObject room;
        [SerializeField]
        GameObject player;

        [SerializeField]
        private float move_speed = 0.003f;

        [SerializeField]
        private float rotatoin_speed = 0.01f;

        private bool active = true;
        private bool done = false;

        [SerializeField]
        public UnityEvent onDoneAlign;

        [SerializeField]
        private List<ButtonHandler> OptionalButtons;


        //List<string> xboxButtons = new List<string> { "X", "Y", "A", "B", "Left Stick Button", "Right Stick Button", "Start", "Back", "RB", "LB", };
        //List<string> xboxButtons = new List<string> { "BeginEndAlign", "DoneAlign" };
        //List<string> xboxAxes = new List<string> { "Left Stick X", "Left Stick Y", "Right Stick X", "Right Stick Y", "D-pad X", "D-pad Y", "RT", "LT", "Triggers" };
        //List<string> xboxAxes = new List<string> { "MoveX", "MoveZ", "MoveY", "RotateY" };

        //void LogButtons()
        //{
        //    //Debug.Log("printing buttons");
        //    foreach (string name in xboxButtons)
        //    {
        //        //Debug.Log("checking " + name);
        //        if (Input.GetButtonDown(name))
        //            RLogger.Log("Button Down: " + name.ToString());
        //    }
        //}

        //void LogAxes()
        //{
        //    foreach (string name in xboxAxes)
        //    {
        //        if (Input.GetAxis(name) != 0)
        //            RLogger.Log("Axis: " + name + " is: " + Input.GetAxis(name).ToString());
        //    }
        //}
        
        // Moves the player instead of the world. Player is in the same real position, thus, the virtual world is being moved!
        void Update()
        {
            if (done)
            {
                Destroy(this);
            }
            if (Input.GetButtonDown("BeginEndAlign"))
            {
                RLogger.Log("BeginEndAlign was pressed");
                active = !active;
            }
            if (Input.GetButtonDown("DoneAlign") && !done)
            {
                RLogger.Log("DoneAlign was pressed, world position is: " + room.transform.position.ToString() + " player posisiont is: "+ player.transform.position);
                active = false;
                done = true;
                onDoneAlign?.Invoke();
            }
            if (active)
            {
                movePlayer();
            }
            handleExtraButtons();
        }

        void movePlayer()
        {
            float keyboard_movex = Input.GetKey(KeyCode.A) ? 1f : Input.GetKey(KeyCode.D) ? -1f : 0f;
            float keyboard_movez = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
            float keyboard_movey = Input.GetKey(KeyCode.UpArrow) ? 1f : Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;
            float keyboard_rotatey = Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
            // The -1s are applied because we're moving the player not the world if I move myself right, it appears the world is moving left
            player.transform.Translate(-1f*new Vector3(Input.GetAxis("MoveX") + keyboard_movex,
                                                    -Input.GetAxis("MoveY") + keyboard_movey,
                                                    -Input.GetAxis("MoveZ") + keyboard_movez) * move_speed, Space.World);
            // if we assume objects are renders straight up, which we did!, we won't need the room.transform.rotation multiplication. 
            player.transform.Rotate( room.transform.rotation * Vector3.up * -1f *(Input.GetAxis("RotateY") + keyboard_rotatey) * rotatoin_speed);
        }

        void handleExtraButtons()
        {
            foreach (ButtonHandler b in OptionalButtons)
            {
                if (Input.GetButtonDown(b.name))
                {
                    b.onPressed.Invoke();
                }
            }
        }


        //private void PerformRelocatin()
        //{
        //    // a_pos = diff_pos + b_pos => diff_pos = a_pos - b_pos => diff_pos = new_a_pos - new_b_pos => new_a_pos = diff_pos + new_b_pos
        //    // a_rot = diff_rot * b_rot => diff_rot = a_rot * inverse(b_rot) => diff_rot = new_a_rot * inverse(new_b_rot) => new_a_rot = diff_rot*new_b_rot
        //    RLogger.Log("performing relocation");
        //    Vector3 diff_pos = player.transform.position - room.transform.position;
        //    Quaternion diff_rot = player.transform.rotation * Quaternion.Inverse(room.transform.rotation);
        //    room.transform.position = Vector3.zero;
        //    room.transform.rotation = Quaternion.identity;
        //    player.transform.position = diff_pos;
        //    player.transform.rotation = diff_rot;
        //    RLogger.Log("relocation performed");
        //}


        /*********************************************
         * list of all required defined buttons and axes:
         * BegnEndAlign : button
         * DoneAlign: button
         * MoveX: axis -1 to 1
         * MoveZ: axis -1 to 1
         * MoveY: axix -1 to 1
         * RotateY: axis -1 to 1
         *********************************************/
    }
}