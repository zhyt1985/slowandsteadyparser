using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;

namespace SlowAndSteadyParser
{
    public static class ConnectionManagerClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string m_protocol = null;
        private static string m_address = "localhost";
        private static int m_port = 0;

        private static IChannel ms_tcpchannel = null;
        private static IChannel ms_httpchannel = null;

        public static bool IsTcpChannelRegistered
        {
            get { return !(ms_tcpchannel == null); }
        }

        public static void RegisterHttpChannel()
        {
            try
            {
                //注册通道
                HttpChannel chnl = new HttpChannel();
                ChannelServices.RegisterChannel(chnl, false);
                ms_httpchannel = chnl;
                //设置协议
                m_protocol = "http";
                //设置默认端口
                if (m_port == 0) m_port = 8080;
            }
            catch (Exception e)
            {
                log.Error("注册Http通道错误: "+ e.Message);
            }
        }

        public static void RegisterTcpChannel()
        {
            try
            {
                TcpChannel chnl = new TcpChannel();
                ChannelServices.RegisterChannel(chnl, false);
                ms_tcpchannel = chnl;
                m_protocol = "tcp";
                //设置tcp默认端口
                if (m_port == 0) m_port = 1371;
            }
            catch (Exception e)
            {
                log.Error("注册Tcp通道错误: "+ e.Message);
            }
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
                }

                else if (eachChannel == ms_httpchannel)
                {
                    HttpChannel httpChannel = (HttpChannel)eachChannel;
                    //关闭监听；
                    httpChannel.StopListening(null);
                    //注销通道；
                    ChannelServices.UnregisterChannel(httpChannel);
                    ms_httpchannel = null;
                }
            }
        }

        public static string Address
        {
            get { return m_address; }
            set { m_address = value; }
        }

        public static int Port
        {
            get { return m_port; }
            set { m_port = value; }
        }

        public static IDomainTransmissionManager GetDomainTransmissionManager()
        {
            try
            {
                return (IDomainTransmissionManager)Activator.GetObject(typeof(IDomainTransmissionManager), m_protocol + "://" + m_address + ":" + m_port + "/DomainTransmissionManager");
            }
            catch(Exception e)
            {
                log.Error("实例化远程DomianTransmissionManager模块错误: "+m_protocol + "://" + m_address + ":" + m_port + "/DomainTransmissionManager", e);
                throw e;
            }
        }

        public static ITaskTransmissionManager GetTaskTransmissionManager()
        {
            try
            {
                return (ITaskTransmissionManager)Activator.GetObject(typeof(ITaskTransmissionManager), m_protocol + "://" + m_address + ":" + m_port + "/TaskTransmissionManager");
            }
            catch(Exception e)
            {
                log.Error("实例化远程TaskTransmissionManager模块错误: " + m_protocol + "://" + m_address + ":" + m_port + "/TaskTransmissionManager " + e.Message);
                throw e;
            }
        }

        public static void FetchServerStatus()
        {
            try
            {
                ServerStatus s = GetTaskTransmissionManager().GetServerStatus();
                switch (s)
                {
                    case ServerStatus.Online:
                        ServerStatusMonitor.SetServerStatusEnable();
                        break;
                    case ServerStatus.Offline:
                        ServerStatusMonitor.SetServerStatusDiabled();
                        break;
                    case ServerStatus.UnknownError:
                        ServerStatusMonitor.SetServerStatusErrorAddressAndPort();
                        break;
                }
            }
            catch (System.Net.WebException e)
            {
                ServerStatusMonitor.SetServerStatusErrorAddressAndPort();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                ServerStatusMonitor.SetServerStatusErrorAddressAndPort();
            }
            catch 
            {
                ServerStatusMonitor.SetServerStatusErrorAddressAndPort();
            }
        }

        public static IRemoteTask TTMFeachATask()
        {
            try
            {
                return GetTaskTransmissionManager().FetchAClientTask();
            }
            catch (System.Net.WebException e)
            {
                log.Error("连接远程服务器错误: "+e.Message);
                return null;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                log.Error("连接远程服务器错误: "+e.Message);
                return null;
            }
            catch (Exception ee)
            {
                log.Error(ee);
                return null;
            }
        }

        public static List<IRemoteTask> TTMFeachTasks(int number)
        {
            try
            {
                return GetTaskTransmissionManager().FetchClientTasks(number);
            }
            catch (System.Net.WebException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return null;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return null;
            }
            catch (Exception ee)
            {
                log.Error(ee);
                return null;
            }
        }

        public static bool TTMSendBackATask(IRemoteTask task)
        {
            try
            {
                return GetTaskTransmissionManager().SendbackAClientTask(task);
            }
            catch (System.Net.WebException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return false;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return false;
            }
            catch (Exception ee)
            {
                log.Error(ee);
                return false;
            }
        }

        public static bool TTMSendBackTasks(List<IRemoteTask> tasks)
        {
            try
            {
                return GetTaskTransmissionManager().SendbackClientTasks(tasks);
            }
            catch (System.Net.WebException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return false;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                log.Error("连接远程服务器错误: " + e.Message);
                return false;
            }
            catch (Exception ee)
            {
                log.Error(ee);
                return false;
            }
        }
    }
}
