using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NtpClock : IDisposable
{
    private readonly string _server;
    private IPAddress _address;
    private Socket _socket;
    private bool disposedValue;

    public static DateTime baseTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public NtpClock(string server = "pool.ntp.org")
    {
        _server = server;
        _address = Dns.GetHostEntry(_server).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, 0));

        var localEndPoint = (IPEndPoint)_socket.LocalEndPoint;
        Debug.Log($"소켓이 바인딩된 포트 번호: {localEndPoint.Port}");
    }

    private uint SwapEndianness(uint x)
    {
        return (uint)(((x & 0x000000FF) << 24) | ((x & 0x0000FF00) << 8) |
                      ((x & 0x00FF0000) >> 8) | ((x & 0xFF000000) >> 24));
    }


    public DateTime GetNetworkTime()
    {
        try
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            IPEndPoint remoteEndPoint = new IPEndPoint(_address, 123);
            EndPoint serverEndPoint = remoteEndPoint;

            _socket.ReceiveTimeout = 5000;

            DateTime t1 = DateTime.UtcNow;
            _socket.SendTo(ntpData, remoteEndPoint);
            _socket.ReceiveFrom(ntpData, ref serverEndPoint);
            DateTime t4 = DateTime.UtcNow;

            // 서버 응답 시간 추출 (Transmit Timestamp)
            const int serverReplyTime = 40;
            uint intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            uint fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            ulong milliseconds = ((ulong)intPart * 1000) + ((ulong)fractPart * 1000 / 0x100000000L);
            DateTime t3 = baseTime.AddMilliseconds((long)milliseconds);

            TimeSpan rtt = t4 - t1;
            DateTime correctedTime = t3.AddMilliseconds(rtt.TotalMilliseconds / 2.0);

            return correctedTime;
        }
        catch (Exception ex)
        {
            Debug.LogException(new InvalidOperationException("Failed to get network time.", ex));
            return DateTime.UtcNow;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                try { _socket?.Shutdown(SocketShutdown.Both); } catch { }
                _socket?.Close();
                _socket?.Dispose();
            }
            _socket = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
