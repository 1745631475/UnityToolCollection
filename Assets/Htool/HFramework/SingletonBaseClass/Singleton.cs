using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Htool
{
    /// <summary>
    /// 单例模式基类（不继承Mono）
    /// </summary>
    /// <typeparam name="T">返回的实例类型T</typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T instance;
        public static T GetInstance { get { return instance ?? (instance = new T()); } }
    }
}
