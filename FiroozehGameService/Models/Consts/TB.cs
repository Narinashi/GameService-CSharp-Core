namespace FiroozehGameService.Models.Consts
{
    internal static class TB
    {
        // Packet Actions
        public const int ActionAuth = 1;
        public const int ActionPingPong = 2;
        public const int OnTakeTurn = 4;
        public const int OnChooseNext = 5;
        public const int OnLeave = 6;
        public const int OnFinish = 7;
        public const int OnComplete = 8;
        public const int GetUsers = 9;
        public const int OnJoin = 11;



        public const int Errors = 100;


        public const int TurnBasedLimit = 10; // 10 Request per sec
        public const int RestLimit = 1000; //  RestLimit per sec in long
    }
}