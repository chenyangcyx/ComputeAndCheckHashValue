using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckHash3
{
    internal class MergedMemoryStream : Stream
    {
        public static int MAX_SINGLE_STREAM_SIZE = 512 * 1024 * 1024;       // 默认值：1024*1024*1024 = 512MB
        Queue<Stream> streams;
        List<Stream> allStreams;
        long totalLength = 0;

        public MergedMemoryStream(FileInfo fileInfo)
        {
            this.allStreams = initStreamsFromFile(fileInfo);
            foreach (Stream item in allStreams)
            {
                totalLength += item.Length;
            }
            resetStream();
        }

        public MergedMemoryStream(string filePath)
        {
            this.allStreams = initStreamsFromFile(new FileInfo(filePath));
            foreach (Stream item in allStreams)
            {
                totalLength += item.Length;
            }
            resetStream();
        }

        private List<Stream> initStreamsFromFile(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
            {
                throw new FileNotFoundException(fileInfo.FullName);
            }
            List<Stream> streams = new List<Stream>();
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                long fileSize = fileStream.Length;
                int streamCount = Convert.ToInt32(Math.Ceiling((double)fileSize / MAX_SINGLE_STREAM_SIZE));
                if (fileSize % MAX_SINGLE_STREAM_SIZE == 0)
                {
                    streamCount++;
                }
                byte[] buffer = new byte[MAX_SINGLE_STREAM_SIZE];
                for (int i = 1; i <= streamCount; i++)
                {
                    fileStream.Read(buffer, 0, MAX_SINGLE_STREAM_SIZE);
                    MemoryStream item = new MemoryStream();
                    if (i != streamCount)
                    {
                        item.Write(buffer, 0, MAX_SINGLE_STREAM_SIZE);
                    }
                    else
                    {
                        item.Write(buffer, 0, Convert.ToInt32(fileSize % MAX_SINGLE_STREAM_SIZE));
                    }
                    streams.Add(item);
                }
            }

            return streams;
        }

        public void resetStream()
        {
            foreach (var item in allStreams)
            {
                item.Seek(0, SeekOrigin.Begin);
            }
            this.streams = new Queue<Stream>(this.allStreams);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length { get => this.totalLength; }

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (count > 0 && streams.Count > 0)
            {
                int bytesRead = streams.Peek().Read(buffer, offset, count);
                if (bytesRead == 0)
                {
                    streams.Dequeue();
                    continue;
                }

                totalBytesRead += bytesRead;
                offset += bytesRead;
                count -= bytesRead;
            }

            return totalBytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
