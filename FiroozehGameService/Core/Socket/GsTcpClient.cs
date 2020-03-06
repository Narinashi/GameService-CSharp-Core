﻿using FiroozehGameService.Models.EventArgs;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FiroozehGameService.Models.Enums.GSLive;
using FiroozehGameService.Models.GSLive.Command;

namespace FiroozehGameService.Core.Socket
{
    internal class GsTcpClient : GsSocketClient
    {
        private TcpClient _client;
        private NetworkStream _clientStream;
        public bool IsAvailable { get; private set; }

        public GsTcpClient(Area area)
        {
            if (area.Protocol.ToUpper() != "TCP")
                throw new InvalidOperationException("Only TCP Protocol Supported");
            
            Endpoint = area;
        }

        internal override async Task Init()
        {
            _client = new TcpClient();
            OperationCancellationToken = new CancellationTokenSource();
            await _client.ConnectAsync(Endpoint.Ip, Endpoint.Port);
            _clientStream = _client.GetStream();
            IsAvailable = true;
        }

        internal override void UpdatePwd(string newPwd)
        {
            Pwd = newPwd;
        }

        internal override void SetType(GSLiveType type)
        {
            Type = type;
        }


        internal override async Task StartReceiving()
        {
            while (true)
            {
                try
                {
                    BufferReceivedBytes += await _clientStream.ReadAsync(
                        Buffer,
                        BufferOffset,
                        Buffer.Length - BufferOffset,
                        OperationCancellationToken.Token);
                 
                    //var receivedData = PacketDeserializer.Deserialize(Buffer, BufferOffset, BufferReceivedBytes,Pwd,Type);
                    DataBuilder.Append(Encoding.UTF8.GetString(Buffer, BufferOffset, BufferReceivedBytes));
                    var packets = PacketValidator.ValidateDataAndReturn(DataBuilder);
                    foreach (var packet in packets)
                        OnDataReceived(new SocketDataReceived {Data = packet});
                   
                    BufferReceivedBytes = 0;
                }
                catch (Exception e)
                {
                    IsAvailable = false;
                    Pwd = null;
                    if (!(e is OperationCanceledException || e is ObjectDisposedException))
                        OnClosed(new ErrorArg {Error = e.Message});
                    break;
                }
            }
        }
        

        internal override void Send(Packet packet)
            => Task.Run(() =>
            {
                 var buffer = PacketSerializer.Serialize(packet,Pwd,Type);
                _clientStream?.Write(buffer, 0, buffer.Length);
            },OperationCancellationToken.Token);


        internal override async Task SendAsync(Packet packet)
        {
            var buffer = PacketSerializer.Serialize(packet,Pwd,Type);
            await _clientStream.WriteAsync(buffer, 0, buffer.Length);
        }
        


        internal override void StopReceiving()
        {
            try
            {
                OperationCancellationToken?.Cancel(true);
                OperationCancellationToken?.Dispose();
                _client?.Close();
                DataBuilder?.Clear();
                IsAvailable = false;
                Pwd = null;
            }
            catch
            {
                // ignored
            }
        }
    }
}
