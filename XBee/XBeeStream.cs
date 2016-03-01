using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XBee
{
    public class XBeeStream : Stream
    {
        private readonly XBeeNode _node;
        private readonly BlockingCollection<byte[]> _receivedDataQueue = new BlockingCollection<byte[]>();
        private Stream _partialBlockStream;

        public XBeeStream(XBeeNode node)
        {
            _node = node;
            node.DataReceived += NodeOnDataReceived;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadImpl(buffer, offset, count, CancellationToken.None);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var data = new byte[count];
            Array.Copy(buffer, offset, data, 0, count);
            return _node.TransmitDataAsync(data, cancellationToken, !DisableAck);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.FromResult(ReadImpl(buffer, offset, count, cancellationToken));
        }

        private int ReadImpl(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();

            if (_partialBlockStream != null)
            {
                if (_partialBlockStream.Length - _partialBlockStream.Position <= count)
                {
                    _partialBlockStream.CopyTo(stream);
                    _partialBlockStream = null;
                }
                else
                {
                    _partialBlockStream.Read(buffer, offset, count);
                    return count;
                }
            }

            var timeoutCancellationTokenSource = new CancellationTokenSource(ReadTimeout);
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                timeoutCancellationTokenSource.Token, cancellationToken);

            while (stream.Length < count)
            {
                var remaining = count - (int)stream.Length;

                var block = _receivedDataQueue.Take(linkedTokenSource.Token);

                if (block.Length < remaining)
                {
                    stream.Write(block, 0, block.Length);
                }
                else
                {
                    _partialBlockStream = new MemoryStream(block);
                    stream.Write(block, 0, remaining);
                }
            }

            stream.Write(buffer, 0, count);
            return count;
        }

        public bool DisableAck { get; set; }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_node != null)
                        _node.DataReceived -= NodeOnDataReceived;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override int ReadTimeout { get; set; }
        public override int WriteTimeout { get; set; }

        private void NodeOnDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            _receivedDataQueue.Add(dataReceivedEventArgs.Data);
        }
    }
}
