﻿using FiroozehGameService.Models.Command;

namespace FiroozehGameService.Handlers.Command.ResponseHandlers
{
    internal class AuthResponseHandler : BaseResponseHandler
    {
        public static int ActionCommand 
            => Models.Consts.Command.ActionAuth;

        protected override void HandleResponse(Packet packet)
        {
            CommandHandler.PlayerHash = packet.Token;
            CoreEventHandlers.OnAuth?.Invoke(null,packet.Token);
        }
    }
}
