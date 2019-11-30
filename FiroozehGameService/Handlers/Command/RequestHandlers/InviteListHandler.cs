﻿using FiroozehGameService.Models.Command;
using Newtonsoft.Json;

namespace FiroozehGameService.Handlers.Command.RequestHandlers
{
    internal class InviteListHandler : BaseRequestHandler
    {
        public static string Signature
            => "INVITE_LIST";

        public InviteListHandler(){}

        private static Packet DoAction(RoomDetail inviteOptions)
            => new Packet(
                CommandHandler.PlayerHash,
                Models.Consts.Command.ActionGetInviteList,
                JsonConvert.SerializeObject(inviteOptions, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                );
        protected override bool CheckAction(object payload)
           => payload.GetType() == typeof(RoomDetail);

        protected override Packet DoAction(object payload)
        {
            if (!CheckAction(payload)) throw new System.ArgumentException();
            return DoAction(payload as RoomDetail);
        }
    }
}