﻿using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.Log.Syslog
{
    interface ILogSender
    {
        int LengthLimit { get; }
        
        Task Send(string log);
    }

    class UdpLogSender : ILogSender
    {
        public string Hostname { get; }
        public int Port { get; }

        public UdpLogSender(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public int LengthLimit { get; } = 8000; //Actually 65507 bytes

        public async Task  Send(string log)
        {
            var bin = Encoding.UTF8.GetBytes(log);
            
            using var cl = new UdpClient();
            await cl.SendAsync(bin, bin.Length, Hostname, Port);
        }
    }

    class TcpLogSender : ILogSender
    {
        public string Hostname { get; }
        public int Port { get; }

        public TcpLogSender(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        public int LengthLimit { get; } = -1;

        public async Task Send(string log)
        {
            using var cl = new TcpClient(Hostname, Port);
            
            await using var s = cl.GetStream();
            await using var w = new StreamWriter(s);

            await w.WriteAsync(log);
            
        }
    }

    class UdpLogSenderFactory : ILogSenderFactory
    {
        public ILogSender Create(string hostname, int port)
        {
            return new UdpLogSender(hostname, port);
        }
    }

    class TcpLogSenderFactory : ILogSenderFactory
    {
        public ILogSender Create(string hostname, int port)
        {
            return new TcpLogSender(hostname, port);
        }
    }
}