using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Frames.AtCommands;

namespace XBee
{
    internal class SerialConnection : IDisposable
    {
        private readonly AutoResetEvent _closeCompleteAutoResetEvent = new AutoResetEvent(true);
        private readonly FrameSerializer _frameSerializer = new FrameSerializer();

        private readonly SerialPort _serialPort;
        private CancellationTokenSource _readCancellationTokenSource;

        public SerialConnection(string port, int baudRate)
        {
            _serialPort = new SerialPort(port, baudRate);

            _frameSerializer.MemberSerializing += OnMemberSerializing;
            _frameSerializer.MemberSerialized += OnMemberSerialized;
            _frameSerializer.MemberDeserializing += OnMemberDeserializing;
            _frameSerializer.MemberDeserialized += OnMemberDeserialized;
        }

        public HardwareVersion? CoordinatorHardwareVersion
        {
            get { return _frameSerializer.ControllerHardwareVersion; }
            set { _frameSerializer.ControllerHardwareVersion = value; }
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        ///     Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        public async Task Send(FrameContent frameContent)
        {
            byte[] data = _frameSerializer.Serialize(new Frame(frameContent));
            await _serialPort.BaseStream.WriteAsync(data, 0, data.Length);
        }

        public event EventHandler<FrameReceivedEventArgs> FrameReceived;

        public void Open()
        {
            _serialPort.Open();

            _readCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _readCancellationTokenSource.Token;

            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Frame frame = _frameSerializer.Deserialize(_serialPort.BaseStream);

                        if (FrameReceived != null)
                            FrameReceived(this, new FrameReceivedEventArgs(frame.Payload.Content));
                    }
                    catch (IOException)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            throw;
                    }
                }
// ReSharper disable MethodSupportsCancellation
            }, cancellationToken).ContinueWith(new Action<Task>(task => _closeCompleteAutoResetEvent.Set()));
// ReSharper restore MethodSupportsCancellation
        }

        public void Close()
        {
            _readCancellationTokenSource.Cancel();

            _closeCompleteAutoResetEvent.WaitOne();

            _readCancellationTokenSource.Dispose();
            _serialPort.Close();
        }

        private void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberSerialized;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            EventHandler<MemberSerializedEventArgs> handler = MemberDeserialized;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberSerializing;
            if (handler != null)
                handler(sender, e);
        }

        private void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
        {
            EventHandler<MemberSerializingEventArgs> handler = MemberDeserializing;
            if (handler != null)
                handler(sender, e);
        }
    }
}