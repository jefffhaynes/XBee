using System;
using System.Threading;
using System.Threading.Tasks;
using BinarySerialization;
using XBee.Frames.AtCommands;

using System.IO.Ports;

namespace XBee
{
    internal class SerialConnection : IDisposable
    {
        private readonly FrameSerializer _frameSerializer = new FrameSerializer();

        private readonly SerialPort _serialPort;

        private CancellationTokenSource _readCancellationTokenSource;

        private readonly object _openCloseLock = new object();
        private readonly SemaphoreSlim _writeSemaphoreSlim = new SemaphoreSlim(1);


        public SerialConnection(string port, int baudRate)
        {
            _serialPort = new SerialPort(port, baudRate);
        }

        public HardwareVersion? CoordinatorHardwareVersion
        {
            get => _frameSerializer.ControllerHardwareVersion;
            set => _frameSerializer.ControllerHardwareVersion = value;
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized
        {
            add => _frameSerializer.MemberSerialized += value;
            remove => _frameSerializer.MemberSerialized -= value;
        }

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized
        {
            add => _frameSerializer.MemberDeserialized += value;
            remove => _frameSerializer.MemberDeserialized -= value;
        }

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing
        {
            add => _frameSerializer.MemberSerializing += value;
            remove => _frameSerializer.MemberSerializing -= value;
        }

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing
        {
            add => _frameSerializer.MemberDeserializing += value;
            remove => _frameSerializer.MemberDeserializing -= value;
        }

        public bool IsOpen => _serialPort.IsOpen;

        public Task Send(FrameContent frameContent)
        {
            return Send(frameContent, CancellationToken.None);
        }

        public async Task Send(FrameContent frameContent, CancellationToken cancellationToken)
        {
            byte[] data = _frameSerializer.Serialize(new Frame(frameContent));


            await _writeSemaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await _serialPort.BaseStream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _writeSemaphoreSlim.Release();
            }
        }

        public event EventHandler<FrameReceivedEventArgs> FrameReceived;

        private Task _receiveTask;

        public void Open()
        {
            lock (_openCloseLock)
            {
                _serialPort.Open();

                _readCancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = _readCancellationTokenSource.Token;

                _receiveTask = Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            Frame frame = _frameSerializer.Deserialize(_serialPort.BaseStream);

                            var handler = FrameReceived;
                            if (handler != null)
                                Task.Run(() =>
                                {
                                    handler(this, new FrameReceivedEventArgs(frame.Payload.Content));
                                },cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            if (!cancellationToken.IsCancellationRequested && !_isClosing)
                                throw;
                        }
                    }
// ReSharper disable MethodSupportsCancellation
                }, cancellationToken);
// ReSharper restore MethodSupportsCancellation

                _receiveTask.ConfigureAwait(false);
            }
        }

        private bool _isClosing;

        public void Close()
        {
            lock (_openCloseLock)
            {
                _isClosing = true;

                if (_receiveTask == null)
                    return;

                _readCancellationTokenSource.Cancel();
                _serialPort.Close();

                try
                {
                    _receiveTask.Wait();
                }
                catch (AggregateException)
                {
                }

                _readCancellationTokenSource.Dispose();

                _receiveTask = null;

                _isClosing = false;
            }
        }
    }
}