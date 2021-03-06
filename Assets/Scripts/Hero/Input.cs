﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerrainDemo.Hero
{
    public class Input : MonoBehaviour
    {
        void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.W))
                Move(Vector3.forward);
            else if (UnityEngine.Input.GetKey(KeyCode.S))
                Move(Vector3.back);
            else if (UnityEngine.Input.GetKey(KeyCode.A))
                Move(Vector3.left);
            else if (UnityEngine.Input.GetKey(KeyCode.D))
                Move(Vector3.right);
            else if (UnityEngine.Input.GetKey(KeyCode.Q))
                Rotate(-1);
            else if (UnityEngine.Input.GetKey(KeyCode.E))
                Rotate(1);

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                Fire();
            }

            if (UnityEngine.Input.GetMouseButtonUp(1))
            {
                Build();
            }


            //Debug keys
            //Soft restart
            if (UnityEngine.Input.GetKey(KeyCode.R) && UnityEngine.Input.GetKey(KeyCode.LeftShift))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Fired when move. Param: local move direction
        /// </summary>
        public event Action<Vector3> Move = delegate { };

        public event Action<float> Rotate = delegate { };

        public event Action Fire = delegate { };

        public event Action Build = delegate { };
    }
}
