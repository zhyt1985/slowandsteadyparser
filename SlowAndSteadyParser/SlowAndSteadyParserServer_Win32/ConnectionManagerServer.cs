using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;

namespace SlowAndSteadyParser
{
    public static class ConnectionManagerServer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int mc_httpport = 8080;
        private const int mc_tcpport = 1371;

        private static IChannel ms_tcpchannel = null;
        private static IChannel ms_httpchannel = null;
        private static TaskTransmissionManager ms_tobj = null;
        private static DomainTransmissionManager ms_sobj = null;

        public static void RegisterHttpChannel(int port)
        {
            //注册通道
            HttpChannel chnl = new HttpChannel(port);
            ChannelServices.RegisterChannel(chnl, false);
            ms_httpchannel = chnl;
            log.Info("Http Service:" + port + " Start!");
        }

        public static void RegisterTcpChannel(int port)
        {
            TcpChannel chnl = new TcpChannel(port);
            ChannelServices.RegisterChannel(chnl, false);
            ms_tcpchannel = chnl;
            log.Info("Tcp Service:" + port + " Start!");
        }

        public static void UnregisterChannel()
        {
            //获得当前已注册的通道；
            IChannel[] channels = ChannelServices.RegisteredChannels;

            //关闭指定通道；
            foreach (IChannel eachChannel in channels)
            {
                if (eachChannel == ms_tcpchannel)
                {
                    TcpChannel tcpChannel = (TcpChannel)eachChannel;
                    //关闭监听；
                    tcpChannel.StopListening(null);
                    //注销通道；
                    ChannelServices.UnregisterChannel(tcpChannel);
                    ms_tcpchannel = null;
                    log.Info("Tcp Service Stop!");
                }

                else if (eachChannel == ms_httpchannel)
                {
                    HttpChannel httpChannel = (HttpChannel)eachChannel;
                    //关闭监听；
                    httpChannel.StopListening(null);
                    //注销通道；
                    ChannelServices.UnregisterChannel(httpChannel);
                    ms_httpchannel = null;
                    log.Info("Http Service Stop!");
                }
            }
        }

        private static void RegisterObjectWellKnown(MarshalByRefObject obj, string servicename)
        {
            ObjRef objrefWellKnown = RemotingServices.Marshal(obj, servicename);
            log.Debug("RegisterService: " + obj.ToString()+" as "+servicename);
        }

        private static void UnregisterObjectWellKnown(MarshalByRefObject obj)
        {
            if (obj!=null) 
                log.Debug("UnregisterService: " + obj.ToString()+" "+RemotingServices.Disconnect(obj).ToString());
        }

        public static bool StartHttpServer(TaskTransmissionManager ttm, ref string errormessage)
        {
            return StartHttpServer(mc_httpport, ttm, ref errormessage);
        }

        public static bool StartHttpServer(int port, TaskTransmissionManager ttm, ref string errormessage)
        {
            try
            {
                RegisterHttpChannel(port);
            }
            catch (Exception e)
            {
                errormessage = e.Message;
                return false;
            }

            try
            {
                ms_tobj = ttm;
                try
                {
                    RegisterObjectWellKnown(ms_tobj, "TaskTransmissionManager");

                }
                catch (Exception e)
                {
                    errormessage = e.Message;
                    return false;
                }

                ms_sobj = new DomainTransmissionManager();
                try
                {
                    RegisterObjectWellKnown(ms_sobj, "ScriptTransmissionManager");
                }
                catch (Exception e)
                {
                    errormessage = e.Message;
                    UnregisterObjectWellKnown(ms_tobj);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                errormessage = e.Message;
                UnregisterChannel();
                return false;
            }


        }

        public static bool StartTcpServer(TaskTransmissionManager ttm, ref string errormessage)
        {
            return StartTcpServer(mc_tcpport,ttm, ref errormessage);
        }

        public static bool StartTcpServer(int port, TaskTransmissionManager ttm, ref string errormessage)
        {
            if (ms_tcpchannel == null)
            {
                errormessage = "缺少必要TCP端口, 请关闭后重新运行服务器程序!";
                return false;
            }

            try
            {
                ms_tobj = ttm;
                try
                {
                    RegisterObjectWellKnown(ms_tobj, "TaskTransmissionManager");

                }
                catch (Exception e)
                {
                    errormessage = e.Message;
                    return false;
                }

                ms_sobj = new DomainTransmissionManager();
                try
                {
                    RegisterObjectWellKnown(ms_sobj, "DomainTransmissionManager");
                }
                catch (Exception e)
                {
                    errormessage = e.Message;
                    UnregisterObjectWellKnown(ms_tobj);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                errormessage = e.Message;
                return false;
            }
        }

        public static void StopServer()
        {
            UnregisterObjectWellKnown(ms_tobj);
            UnregisterObjectWellKnown(ms_sobj);
            ms_sobj = null;
            ms_tobj = null;
        }
    }
}
