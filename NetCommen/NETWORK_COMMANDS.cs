namespace NetCommen
{
    public static class NETWORK_COMMANDS
    {
        public const string NETWORK_VERSION = "1.0.0";

        public const int KEEPALIVE = 20;

        public const int CS = 400;
        public const int SC = 1000;

        public const int ES = 1600;
        public const int SE = ES + 200;

        #region Client_Server
        public const int CS_Handshake       = CS + 1;
        public const int CS_Disconect       = CS + 2;

        public const int CS_ALIVE           = CS + 3;

        public const int CS_SYNC            = CS + 9;

        public const int CS_Player          = CS + 20;
        public const int CS_PlayerAction    = CS_Player + 1;
        public const int CS_PlayerWarp      = CS_Player + 2;
        public const int CS_PlayerDie       = CS_Player + 3;

        public const int CS_Action          = CS_Player + 4;

        public const int CS_Chat            = CS + 25;
        public const int CS_Command         = CS + 26;
        #endregion

        #region Server_Client
        public const int SC_Handshake       = SC + 1;
        public const int SC_PlayerDisconect = SC + 2;
        public const int SC_Disconect       = SC + 3;
        public const int SC_ACK             = SC + 4;
        public const int SC_UDP             = SC + 5;

        public const int SC_SYNC            = SC + 9;

        public const int SC_Update          = SC + 10;

        public const int SC_Player          = SC + 20;
        public const int SC_PlayerAction    = SC_Player + 1;
        public const int SC_PlayerWarp      = SC_Player + 2;
        public const int SC_PlayerDie       = SC_Player + 3;

        public const int SC_Action          = SC_Player + 4;

        public const int SC_NPC             = SC + 40;
        public const int SC_NPCSpawn        = SC_NPC + 1;
        public const int SC_NPCDespawn      = SC_NPC + 2;
        public const int SC_NPCAction       = SC_NPC + 3;
        public const int SC_NPCSync         = SC_NPC + 3;
        #endregion

        #region Server_Event
        public const int SE_Handshake       = SE + 0;
        public const int SE_EventDetail     = SE + 14;
        #endregion

        #region Event_Server
        public const int ES_Handshake       = ES + 0;
        public const int ES_Welcome         = ES + 1;

        public const int ES_StartEvent      = ES + 10;
        public const int ES_EndEvent        = ES + 11;
        public const int ES_EventSuccess    = ES + 12;
        public const int ES_EventFailed     = ES + 13;
        public const int ES_EventDetail     = ES + 14;

        public const int ES_UpdateObjective = ES + 20;
        #endregion
    }
}
