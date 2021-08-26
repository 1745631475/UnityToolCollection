using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Htool
{
    /// <summary>
    /// 单例模式基类（不继承Mono）
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static T instance;
        public static T GetInstance { get { return instance ?? (instance = new T()); } }
    }
}
