using ElpisOpcServer.Configurations;
using ElpisOpcServer.Extensions;
using ElpisOpcServer.Models;
using ElpisOpcServer.SocketService.Net.Formatters;
using ElpisOpcServer.SunPowerGen;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using Elpis.Windows.OPC.Server;
using System.Diagnostics;
using ElpisOpcServer.Diagnostics;

namespace ElpisOpcServer.SocketService.Net
{
    public class SocketConnection : IDisposable
    {
        public bool IsConnected;
        private bool isRunning;
        private readonly TcpEndPointSettings settings;
        private CancellationTokenSource cts;
        public static Socket socket;
        private System.Net.EndPoint remoteEndPoint;
        private BlockingCollection<MessagePacket> entryQueue;
        private Task keepAliveTask;
        public Task dispatchTask;
        public Task recieveTask;
        public int lastCommTime;
        private Task DataLogg;
        private MessageIdGenerator idGenerator;
        private Dispatcher.PacketDispatcher dispatcher;
        public static int packetlength;
        public static int readDataCount = 0;
        public IPacketWriter Writer { get; set; }

        public IPacketReader Reader { get; set; }
        public static ConcurrentQueue<string> concurrentQueue = new ConcurrentQueue<string>();
        private static readonly ILogger _logger;
        static SocketConnection()
        {
            _logger = new Logger();
        }
        public SocketConnection()
        {
            entryQueue = new BlockingCollection<MessagePacket>(32);
            Writer = new CharWriter();
            Reader = new CharReader();
            idGenerator = new MessageIdGenerator();
            dispatcher = new Dispatcher.PacketDispatcher();
            Pump_Test.configPacketWithSelectedTagNames();
        }

        public async Task<bool> StartClient(CancellationToken stopToken)
        {
            try
            {
                if (isRunning)
                {
                    _logger.LogInfo($"Connection already started");
                    throw new InvalidOperationException("Connection already started");
                }
                #region E_remos code
                //if (settings.Enabled == false)
                //    return false;
                //if (settings.TryReadAddress(out var address) == false && settings.Port <= 0)
                //{
                //    throw new Exception("Could not start Tcp Connection");
                //}
                //IPEndPoint endPoint = new IPEndPoint(address, settings.Port);
                //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.98"), 5008);
                #endregion E_remos code

                var tcpdevice = SunPowerGenMainPage.DeviceObject as TcpSocketDevice;
                IPEndPoint endPoint1 = new IPEndPoint(IPAddress.Parse(tcpdevice.IPAddress), tcpdevice.Port);
                // IPEndPoint endPoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5008);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (await socket.ConnectAsync(endPoint1, 200000) == false)
                {
                    _logger.LogInfo($"Unable to connect to socket { endPoint1}");
                    MessageBox.Show($"Unable to connect to socket {endPoint1}");
                    IsConnected = false;
                    return false;
                }
                _logger.LogInfo($"Connected to socket { endPoint1}");
                remoteEndPoint = endPoint1;
                IsConnected = true;
                cts = new CancellationTokenSource();
                idGenerator.Reset();
                dispatchTask = Task.Factory.StartNew(BeginDispatch, creationOptions: TaskCreationOptions.LongRunning);
                keepAliveTask = Task.Factory.StartNew(KeepAlive, creationOptions: TaskCreationOptions.LongRunning);
                recieveTask = Task.Factory.StartNew(BegineRecieve, creationOptions: TaskCreationOptions.LongRunning);

                return true;
            }
            catch (Exception ex)
            {
                
                _logger.LogError($"Unable to connect to socket" + ex.Message);
                MessageBox.Show(ex.Message);
                return false;
            }

        }
        void BegineRecieve()
        {
            var token = cts.Token;
            var reader = Reader;
            try
            {

                byte[] headerBytes = reader.CreateHeader();
                for (; ; )
                {
                    if (token.IsCancellationRequested)
                        return;
                    int res = Read(headerBytes, 0, headerBytes.Length);

                    if (res == headerBytes.Length)
                    {

                        var header = reader.ReadHeader(headerBytes);
                        if (header.Length < 0)
                            return;
                        var data = new byte[header.Length];
                        res = Read(data, 0, header.Length);


                        if (res == header.Length)
                        {

                            OnReciveMessage(header, data);
                            continue;
                        }
                    }
                    if (res == 0)
                    {
                        _logger.LogInfo($"Closing socket" + remoteEndPoint);
                        MessageBox.Show("Closing Socket");
                        return;
                    }
                }
            }
            catch (SocketException ex)
            {
                _logger.LogError("SocketRecieve() SocketException : " + ex.Message);
                MessageBox.Show("Recieve() : " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError("SocketRecieve() IndexOutofRange : " + ex.Message);
                MessageBox.Show("Recieve() : " + ex.Message);
            }
            finally
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    _logger.LogInfo("SocketReceive() Finally Block Executed cts CancellationRequested");
                    cts.Cancel();
                }
            }
        }

        private void OnReciveMessage(MessageHeader header, byte[] data)
        {
            switch (header.Type)
            {
                case MessageType.DeviceReplay:
                    dispatcher.Dispatch(header.Id);
                    break;
                case MessageType.DeviceData:
                    ParseData(data).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
            string val = Encoding.Default.GetString(data);
        }

        public async Task ParseData(byte[] data)
        {

            string RawData = Encoding.Default.GetString(data);
            
            if (RawData.StartsWith("getsignallist"))
            {
               
                _logger.LogInfo($"getsignallist : " + RawData);
               
               
                Pump_Test.ExtratConfigPacket();
                await SendSignalListDataToDeviceAsync(Pump_Test.channelConfigJsonString).ConfigureAwait(false);
                Pump_Test.PumpTest.btnStartTestEnable();
            }
            else
            {
                concurrentQueue.Enqueue(RawData);
                _logger.LogInfo("Enque DataPacket  : " + RawData);
                Debug.Print($"concurrentQueue.Enqueue:{DateTime.Now.Millisecond}:{concurrentQueue.Count.ToString()}{RawData}");

            }

        }
        /// <summary>
        /// I need to implement the functions sathish
        /// 
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public async Task SendSignalListDataToDeviceAsync(string rawData)
        {
            try
            {
                var message = CreateJson(rawData);
                var packet = new MessagePacket
                {
                    Message = message,
                    Type = MessageType.FromClient,
                    Quality = DeliveryQuality.ExactlyOnce | DeliveryQuality.Synchronous,
                    Retain = true
                };
                var res = await SendAndWait(packet);
            }
            catch (Exception ex)
            {
                _logger.LogError("SendSignalListDataToDeviceAsync :" + ex.Message);
                MessageBox.Show(ex.Message);
            }

        }

        public async Task<SocketResponse> SendAndWait(MessagePacket packet)
        {
            packet.Id = idGenerator.GetNextPacketIdentifier();
            var awaiter = dispatcher.AddAwaiter(packet.Id);
            try
            {
                var enqueStatus = await Enqueue(packet);
                if (enqueStatus == SocketResponse.Success
                    && await awaiter.WaitOneAsync(TimeSpan.FromMilliseconds(30000)))//await awaiter.WaitOneAsync(TimeSpan.FromMilliseconds(settings.SendTimeout))) 
                {
                    return SocketResponse.Success;
                }
                return SocketResponse.FailedToPublish;
            }
            finally
            {
                awaiter.Dispose();
            }
        }

        int Read(byte[] buffer, int offset, int count)
        {
            int read = socket.Receive(buffer, offset, count - offset, SocketFlags.None);
            var dat = Encoding.UTF8.GetString(buffer);
            if (read == 0)
                return 0;
            offset += read;
            if (offset < count)
            {
                return Read(buffer, offset, count);
            }
            return offset;
        }

        async void KeepAlive()
        {
            var token = cts.Token;
            //var keepAlivePeriod = settings.KeepAliveInterval * 1000;
            var keepAlivePeriod = 120 * 1000;
            var wait = keepAlivePeriod;

            var message = JsonDocument.Parse("{}");
            try
            {
                for (; ; )
                {
                    if (token.IsCancellationRequested)
                        return;
                    await Task.Delay(wait, token);
                    var delta = Environment.TickCount - lastCommTime;
                    // If timeout
                    if (delta >= keepAlivePeriod)
                    {
                        await Enqueue(new MessagePacket { Type = MessageType.Ping, Message = message.RootElement });
                        wait = keepAlivePeriod;
                    }
                    else
                    {
                        // update waiting time
                        wait = keepAlivePeriod - delta;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"KeepAlive() : " + ex.Message);
                MessageBox.Show($"Stoped KeepAlive(){ex.Message}");
            }
        }

        public async ValueTask<SocketResponse> Enqueue(MessagePacket packet)
        {
            if (!isRunning && !await StartClient(cts.Token))
            {

                return SocketResponse.NotReachable;
                _logger.LogInfo("Enqueue() :SocketReponse. NotReachable.");
            }
            if (!entryQueue.IsAddingCompleted)
            {
                try
                {
                    entryQueue.Add(packet);
                    return SocketResponse.Success;
                }
                catch (InvalidOperationException)
                {

                }
            }
            return SocketResponse.FailedToPublish;

        }

        async void BeginDispatch()
        {
            // Writer = new CharWriter();
            isRunning = true;
            var writer = Writer;
            // writer.Reset(0);

            var enumerator = entryQueue
                .GetConsumingEnumerable(cts.Token).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    writer.Reset(Pump_Test.packetlength);
                    var da = "#" + Encoding.UTF8.GetString(writer.Encode(enumerator.Current)) + "$";
                    _logger.LogInfo($"DataPost From App to Device :" + da);
                    byte[] buffer = Encoding.UTF8.GetBytes(da);
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None, out var errorCode);
                    if (errorCode != SocketError.Success)
                    {
                        if (errorCode == SocketError.ConnectionReset
                            || errorCode == SocketError.ConnectionAborted)
                        {
                            socket.Disconnect(true);
                        }
                        // try one more time
                        await ConnectSend(buffer);
                    }
                    lastCommTime = Environment.TickCount;
                }
            }
            catch (SocketException ex)
            {

                
                _logger.LogError("Despatch() Socket ex : " + ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Despatch() OperationCanceledException ex : " + ex.Message);
            } // supress cancel exception

            catch (Exception ex)
            {
                _logger.LogError("Despatch()  ex : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                enumerator.Dispose();
                DisconnectConnection();
                _logger.LogInfo("Despatch() FinalBlock Executed ");
            }
            //throw new NotImplementedException();
        }

        public void DisconnectConnection()
        {

            isRunning = false;
            dispatcher.CancelAll();
            socket.Disconnect(false);
            socket.Close();
            cts.Dispose();
            cts = null;

        }

        public async Task ConnectSend(byte[] payload)
        {
            if (await socket.ConnectAsync(remoteEndPoint, 20000) == false)
            {
                _logger.LogInfo($"ConnectSend() : Unable to connect to socket " + remoteEndPoint);
                MessageBox.Show("Unable to connect to socket " + remoteEndPoint);
                return;
            }
            socket.Send(payload, SocketFlags.None);
        }
        public async Task Stop(CancellationToken cancellationToken)
        {
            entryQueue.CompleteAdding();
            cts?.Cancel();
            await Task.WhenAny(keepAliveTask, dispatchTask, recieveTask, Task.Delay(-1, cancellationToken));
        }
        public void Dispose()
        {
            cts?.Dispose();
        }

       
        private JsonElement CreateJson(string input)
        {
            System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(input);
            System.Text.Json.JsonElement result = doc.RootElement;
            return result;
        }

    }
}
