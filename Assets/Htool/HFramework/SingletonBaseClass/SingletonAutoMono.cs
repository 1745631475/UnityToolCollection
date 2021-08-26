using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Htool
{
    /// <summary>
    /// 单例模式基类（继承Mono）(自动创建)
    /// </summary>
    /// <typeparam name="T">返回的实例类型</typeparam>
    public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                }
                return instance;
            }
        }
    }
}
