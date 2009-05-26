using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SlowAndSteadyParser
{
    public class Balancer : IDisposable
    {
        //PerformanceCounter
        private PerformanceCounter m_pc = null;
        //希望保持的CPU占用率
        private float m_atticpatecpuusage = 100;
        //移动平均计算周期数
        private int m_circleofcaclulatation = 20;
        //到目前为止的移动平均值
        private float m_movingaveragecpuusage = 50;
        //CPU利用率采样时间间隔
        private int m_samplinginterval = 5;
        //目前使用Sleep值
        private int m_threadsleepvalue = 50;
        //Sleep增减数
        private int m_sleepvalueincrement = 10;
        //线程
        private Thread m_samplingthread = null;
        //Name
        private string m_name;
        //Enable
        private bool m_enable = false;

        //在程序重复运行的后面放置这个平衡点函数
        public void InvokeBalancePoint()
        {
            if (m_enable)
            {
                Thread.Sleep(m_threadsleepvalue);
                if (m_samplingthread == null)
                {
                    m_samplingthread = new Thread(SamplingThread);
                    m_samplingthread.Name = m_name + "'s SamplingThread";
                    m_samplingthread.Start();
                }
            }
        }

        public int AnticapateCPUUsage
        {
            get { return (int)m_atticpatecpuusage; }
            set { m_atticpatecpuusage = (float)value; }
        }

        public int CircleOfCaclulation
        {
            get { return m_circleofcaclulatation ; }
            set { m_circleofcaclulatation = value; }
        }

        public int SamplingInterval
        {
            get { return m_samplinginterval; }
            set { m_samplinginterval = value; }
        }

        private void SamplingThread()
        {
            while (true)
            {
                float k = 2 / (float)(m_circleofcaclulatation + 1);
                float u = m_pc.NextValue();
                m_movingaveragecpuusage = m_movingaveragecpuusage * (1 - k) + u * k;

                if (m_movingaveragecpuusage < m_atticpatecpuusage - 5)
                {
                    m_threadsleepvalue = m_threadsleepvalue - m_sleepvalueincrement;                    
                }
                else if (m_movingaveragecpuusage > m_atticpatecpuusage + 5)
                    m_threadsleepvalue = m_threadsleepvalue + m_sleepvalueincrement;

                if (m_threadsleepvalue <= 0) m_threadsleepvalue = 1;

                //采样周期
                Thread.Sleep(m_samplinginterval * 1000);
            }
        }

        public Balancer(string name, bool m_enable)
        {
            m_name = name;
            m_enable = m_enable;
            m_pc = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            m_pc.NextValue();            
        }

        #region IDisposable 成员

        private bool IsDisposed = false;

        public virtual void Dispose()  
        {  
            Dispose(true);  
            GC.SuppressFinalize(this);  
        }

        protected void Dispose(bool Disposing)  
        {  
            if(!IsDisposed)  
            {
                if (Disposing)  
                {
                    //清理托管资源
                }
  
                //清理非托管资源
                if (m_samplingthread != null && m_samplingthread.IsAlive)
                {
                    m_samplingthread.Abort();
                    m_samplingthread = null;
                }
            }  
            IsDisposed=true;  
        }

        ~Balancer()
        {  
            Dispose(false);  
        } 

        #endregion
    }

    public class PerformanceBalancer
    {
        private static bool isRunning = false;

        private static Dictionary<string, Balancer> m_dictBalancer = new Dictionary<string, Balancer>();

        public static void Init(bool isrunning)
        {
            isRunning = isrunning;
        }

        public static Balancer GetBalancer(string Name)
        {
            if (m_dictBalancer.ContainsKey(Name))
                return m_dictBalancer[Name];
            else
            {
                Balancer p = new Balancer(Name,isRunning);
                m_dictBalancer.Add(Name, p);
                return p;
            }
        }

        public static void Dispose()
        {
            foreach (Balancer b in m_dictBalancer.Values)
                b.Dispose();
            m_dictBalancer.Clear();
        }
    }
}
