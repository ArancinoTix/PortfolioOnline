namespace U9.OSC
{
    public interface ISender
    {
        void SendMessageToClient(U9OscMessage message, bool handshake);

        void SendMessageToClient(string message, bool handshake);

        void SendMessageToClient(string command, object payload, bool handshake);

        void SendMessageToClient(string messageCommand, bool handshake, params string[] message);
    }
}