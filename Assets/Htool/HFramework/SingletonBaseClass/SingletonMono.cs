using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Htool
{
    /// <summary>
    /// 单例模式基类（继承Mono）
    /// 使用此基类时需要重写Awake方法，并且保留基类方法
    /// </summary>
    /// <typeparam name="T">返回的实例类型T</typeparam>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T GetInstance { get; private set; }
        protected virtual void Awake()
        {
            GetInstance = this as T;
        }
    }
}