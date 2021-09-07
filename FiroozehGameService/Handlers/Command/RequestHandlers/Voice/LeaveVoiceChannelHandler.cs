﻿// <copyright file="LeaveVoiceChannelHandler.cs" company="Firoozeh Technology LTD">
// Copyright (C) 2021 Firoozeh Technology LTD. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

/**
* @author Alireza Ghodrati
*/


using System;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.GSLive.Command;
using Newtonsoft.Json;

namespace FiroozehGameService.Handlers.Command.RequestHandlers.Voice
{
    internal class LeaveVoiceChannelHandler : BaseRequestHandler
    {
        public static string Signature
            => "LEAVE_VOICE_CHANNEL";

        private static Packet DoAction(string channelId)
        {
            return new Packet(
                CommandHandler.PlayerHash,
                CommandConst.ActionLeaveVoiceChannel,
                JsonConvert.SerializeObject(new VoicePayload(
                        channelId
                    )
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    }));
        }

        protected override bool CheckAction(object payload)
        {
            return payload is string;
        }

        protected override Packet DoAction(object payload)
        {
            if (!CheckAction(payload)) throw new ArgumentException();
            return DoAction(payload as string);
        }
    }
}