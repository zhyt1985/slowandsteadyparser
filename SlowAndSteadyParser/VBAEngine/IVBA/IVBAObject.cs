using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SlowAndSteadyParser

{
    public interface IVBAObject : ISerializable, IDisposable
    {
        /// <summary>
        /// 使用Name来识别该VBA类, 在脚本中使用该名称来调用本类
        /// </summary>
        string Name { get;}

        /// <summary>
        /// 是否需要序列化(能够被储存与传输)
        /// </summary>
        bool BeSerializable { get;}

        /// <summary>
        /// 得到该VBAObject的生命周期
        /// </summary>
        VBAObjectLife Life { get;}

        /// <summary>
        /// 重置该VBAObject
        /// </summary>
        void Reset();

    }
}
