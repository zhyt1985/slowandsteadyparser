using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SlowAndSteadyParser
{
    public class VBAStaticEngine : IDisposable
    {
        #region static
        //Log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, Queue<VBAStaticEngine>> ms_dictenginequeue = new Dictionary<string, Queue<VBAStaticEngine>>();
        
        public static VBAStaticEngine RentVBAEngine(string script)
        {
            lock (ms_dictenginequeue)
            {
                Queue<VBAStaticEngine> enginequeue = null;
                if (ms_dictenginequeue.ContainsKey(script))
                {
                    enginequeue = ms_dictenginequeue[script];
                }
                else
                {
                    enginequeue = new Queue<VBAStaticEngine>();
                    ms_dictenginequeue.Add(script, enginequeue);
                }

                if (enginequeue.Count > 0)
                {
                    return enginequeue.Dequeue();
                }
                else
                {
                    return new VBAStaticEngine(script);
                }
            }
            
        }

        public static void ReturnVBAEngine(VBAStaticEngine engine)
        {
            lock (ms_dictenginequeue)
            {
                Queue<VBAStaticEngine> enginequeue = null;
                if (ms_dictenginequeue.ContainsKey(engine.Script))
                {
                    enginequeue = ms_dictenginequeue[engine.Script];
                }
                else
                {
                    enginequeue = new Queue<VBAStaticEngine>();
                    ms_dictenginequeue.Add(engine.Script, enginequeue);
                }
                enginequeue.Enqueue(engine);
            }
        }

        public static void DisposeAll()
        {
            foreach (Queue<VBAStaticEngine> q in ms_dictenginequeue.Values)
                while (q.Count > 0)
                    q.Dequeue().Dispose();
        }

        #endregion

        private string m_script = null;
        private VBAEngineBase m_engine = new VBAEngineBase();
        private Dictionary<string, IVBAObjectHost> m_vbaobjhosts = new Dictionary<string, IVBAObjectHost>();
        private static Dictionary<string, IVBAObject> m_vbaobjstop = new Dictionary<string, IVBAObject>();

        public string Script
        {
            get { return m_script; }
        }

        private void VBAStaticEngine_BeforeComplie(object sender, EventArgs e)
        {
            m_engine.AddSourceCode("Module1", m_script);
            foreach (IVBAObjectHost iv in m_vbaobjhosts.Values)
                m_engine.AddGlobalObject(iv.Name, iv, "object");
        }
        
        public void Injection(Dictionary<string, IVBAObject> vbaobjs)
        {
            foreach (IVBAObject o in vbaobjs.Values)
                if (m_vbaobjhosts.ContainsKey(o.Name))
                    m_vbaobjhosts[o.Name].Tenant = o;            
        }

        private void Complie()
        {
            //Script脚本不能被修改, 这里仅仅为了注入VBAObjects
            if (!m_engine.CompileAndRun())
            {
                throw new Exception("脚本编译错误:" + m_engine.CompilerError.LineText);
            }      
        }

        public void Run()
        {
            if (m_script != null)
                m_engine.RunScriptMethod("main");
        }

        public void Stop()
        {
            this.Injection(m_vbaobjstop);
            Thread.Sleep(100);
        }

        VBAStaticEngine(string script)
        {
            m_script = script;

            //vbaobject host
            IVBAObjectHost i = new VBALogHost();
            m_vbaobjhosts.Add(i.Name, i);
            i = new VBAIEHost();
            m_vbaobjhosts.Add(i.Name, i);
            i = new VBATaskHost();
            m_vbaobjhosts.Add(i.Name, i);
            i = new VBAUtilityHost();
            m_vbaobjhosts.Add(i.Name, i);
            i = new VBAHtmlHost();
            m_vbaobjhosts.Add(i.Name, i);

            //vbaobjstop
            if (m_vbaobjstop.Count == 0)
            {                
                IVBAObject o = new VBALogStop();
                m_vbaobjstop.Add(o.Name, o);
                o = new VBAIEStop();
                m_vbaobjstop.Add(o.Name, o);
                o = new VBATaskStop();
                m_vbaobjstop.Add(o.Name, o);
                o = new VBAUtilityStop();
                m_vbaobjstop.Add(o.Name, o);
                o = new VBAHtmlStop();
                m_vbaobjstop.Add(o.Name, o);
            }

            m_engine.BeforeCompile += new EventHandler(VBAStaticEngine_BeforeComplie);

            Complie();
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
                if(Disposing)  
                {
                    //清理托管资源
                }  
                //清理非托管资源
                m_engine.Close();
                m_engine = null;
            }  
            IsDisposed=true;  
        }

        ~VBAStaticEngine()  
        {  
            Dispose(false);  
        } 

        #endregion
    }
}
