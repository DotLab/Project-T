using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public abstract class Streamable
    {

    }

    public sealed class Packet
    {
        private long _id;
        private Streamable _streamable;

        public long Id { get => _id; set => _id = value; }
        public Streamable Streamable { get => _streamable; set => _streamable = value; }
    }

    public interface IPacketListener
    {
        void PacketReceived(Streamable streamable);
    }

    public abstract class Network
    {
        public event EventHandler<NetworkBrokenEventArgs> OnNetworkBroken;

        public abstract void SendPacket(Packet packet);
        public abstract void AddPacketListener(long id, IPacketListener listener);
        public abstract void Update();
        
    }

    public class NetworkBrokenEventArgs : EventArgs
    {

    }
}
