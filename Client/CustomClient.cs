using ClientCommon;
using NetCommen;

namespace Client
{
    public class CustomClient : ClientSystem
    {
        public CustomClient() : base()
        {
            NetworkCallbacks.checkChalange = CheckChallange;
        }

        private int CheckChallange(int answer)
        {
            return (answer * 5) << 2;
        }

        public override void ovr_HandleData(int clientId, Packet packet, int PacketId)
        {
            Console.WriteLine(PacketId);
        }
    }
}
