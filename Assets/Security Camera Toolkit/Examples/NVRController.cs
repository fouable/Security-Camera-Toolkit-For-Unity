﻿// Copyright (c) https://github.com/Bian-Sh
// Licensed under the MIT License.
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

namespace zFramework.Media.Demo
{
    //For Demo
    public class NVRController : MonoBehaviour
    {
        public Button login_bt;
        public Button logout_bt;
        public Dropdown dropdown;
        SecurityCamera cam;
        Text login, logout;

        private void Start()
        {
            cam = GetComponent<SecurityCamera>();
            login = login_bt.GetComponentInChildren<Text>();
            logout = logout_bt.GetComponentInChildren<Text>();

            login_bt.onClick.AddListener(Login);
            logout_bt.onClick.AddListener(Logout);
            dropdown.onValueChanged.AddListener(OnSteamTypeChanged);
        }

        private void OnSteamTypeChanged(int arg0)
        {
            cam.Stop();
            cam.steamType = (STREAM)arg0;
            cam.PlayReal();
            Debug.Log($"{nameof(NVRController)}: 完成主辅流的切换");
        }

        void Login() => _ = LoginAsync();
        void Logout() => _ = LogoutAsync();

        async Task LoginAsync()
        {
            login.text = "登录中";
            login_bt.interactable = false;
            await NVRManager.LoginAllAsync();
            login.text = "已登录";
            logout.text = "登出";
            login_bt.interactable = true;
        }

        async Task LogoutAsync()
        {
            logout.text = "登出中";
            logout_bt.interactable = false;
            await NVRManager.LogoutAllAsync();
            logout.text = "已登出";
            login.text = "登录";
            logout_bt.interactable = true;
        }
    }
}
