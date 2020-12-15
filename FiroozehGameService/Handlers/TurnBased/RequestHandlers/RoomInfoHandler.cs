﻿// <copyright file="RoomInfoHandler.cs" company="Firoozeh Technology LTD">
// Copyright (C) 2020 Firoozeh Technology LTD. All Rights Reserved.
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
using FiroozehGameService.Models;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Models.GSLive.TB;
using Newtonsoft.Json;

namespace FiroozehGameService.Handlers.TurnBased.RequestHandlers
{
    internal class RoomInfoHandler : BaseRequestHandler
    {
        public static string Signature => "ROOM_INFO_HANDLER";

        private static Packet DoAction()
        {
            return new Packet(TurnBasedHandler.PlayerHash, TurnBasedConst.OnRoomInfo);
        }

        protected override Packet DoAction(object payload)
        {
            if (!TurnBasedHandler.IsAvailable) throw new GameServiceException("GSLiveTurnBased Not Available yet");
            if (!CheckAction(payload)) throw new ArgumentException();
            return DoAction();
        }

        protected override bool CheckAction(object payload)
        {
            return true;
        }
    }
}