using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SlowAndSteadyParser
{
    public interface IVBAScript : ISerializable
    {
        ///// <summary>
        ///// 脚本
        ///// </summary>
        //string Script { get;set;}

        /// <summary>
        /// 保存VBAObject对象的List
        /// </summary>
        Dictionary <string,IVBAObject> VBAObjs { get;set;}        
    }
}
