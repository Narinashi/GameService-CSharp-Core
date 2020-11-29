﻿// <copyright file="AuthorizationHandler.cs" company="Firoozeh Technology LTD">
// Copyright (C) 2019 Firoozeh Technology LTD. All Rights Reserved.
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

using FiroozehGameService.Models;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive.RT;
using Newtonsoft.Json;

namespace FiroozehGameService.Handlers.RealTime.RequestHandlers
{
    internal class AuthorizationHandler : BaseRequestHandler
    {
        public static string Signature =>
            "AUTHORIZATION";

        protected override Packet DoAction(object payload)
        {
            if (!RealTimeHandler.IsAvailable) throw new GameServiceException("GSLiveRealTime Not Available yet");
            return new Packet(
                0,
                RealTimeConst.ActionAuth,
                GProtocolSendType.Reliable, GetBuffer(JsonConvert.SerializeObject(
                    new AuthPayload(RealTimeHandler.CurrentRoom?.Id, RealTimeHandler.PlayToken)
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    }))
            );
        }

        protected override bool CheckAction(object payload)
        {
            return true;
        }
    }
}