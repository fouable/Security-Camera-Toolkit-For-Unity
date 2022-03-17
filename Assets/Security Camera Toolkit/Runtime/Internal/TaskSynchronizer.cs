﻿// Copyright (c) https://github.com/Bian-Sh
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
namespace zFramework.Media.Internal
{
    /// <summary>
    /// 任务同步器：在主线程中执行 Action 委托
    /// </summary>
    public static class TaskSynchronizer
    {
        static SynchronizationContext context;
        static readonly ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitContext()
        {
            context = SynchronizationContext.Current;
            #region 使用 PlayerLoop 在 Unity 主线程的 Update 中更新本任务同步器
            var playerloop = PlayerLoop.GetCurrentPlayerLoop();
            var loop = new PlayerLoopSystem
            {
                type = typeof(TaskSynchronizer),
                updateDelegate = Update
            };
            //1. 找到 Update Loop System
            int index= Array.FindIndex(playerloop.subSystemList, v =>v.type== typeof(UnityEngine.PlayerLoop.Update));
            //2.  将咱们的 loop 插入到 Update loop 中
            var updateloop = playerloop.subSystemList[index];
             var temp = updateloop.subSystemList.ToList();
            temp.Add(loop);
            updateloop.subSystemList = temp.ToArray();
            playerloop.subSystemList[index] = updateloop;
            //3. 设置自定义的 Loop 到 Unity 引擎
            PlayerLoop.SetPlayerLoop(playerloop);
            //4. 已知：编辑器停止 Play 后依旧会触发，还不知道会有啥影响，持观望态度
            #endregion
        }

        /// <summary>
        ///  在主线程中执行
        /// </summary>
        /// <param name="task">要执行的委托</param>
        public static void Post(Action task)
        {
            if (SynchronizationContext.Current == context)
            {
                task?.Invoke();
            }
            else
            {
                tasks.Enqueue(task);
            }
        }

        static void Update()
        {
            if (tasks.TryDequeue(out var task))
            {
                task?.Invoke();
            }
        }
    }
}