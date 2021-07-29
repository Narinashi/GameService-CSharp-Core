// <copyright file="GameServiceClientConfiguration.cs" company="Firoozeh Technology LTD">
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
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.Internal;
using FiroozehGameService.Utils;

namespace FiroozehGameService.Builder
{
    /// <summary>
    ///     Represents ClientConfiguration For Game Service
    /// </summary>
    public class GameServiceClientConfiguration
    {
        internal readonly string ClientId;
        internal readonly string ClientSecret;
        internal readonly ConnectionType CommandConnectionType;
        internal readonly SystemInfo SystemInfo;
        internal readonly ConnectionType TurnBasedConnectionType;

        private GameServiceClientConfiguration()
        {
        }

        /// <summary>
        ///     Set GameServiceClientConfiguration Values
        /// </summary>
        /// <param name="clientId">Set The ClientId From Developers Panel</param>
        /// <param name="clientSecret">Set The ClientSecret From Developers Panel</param>
        /// <param name="systemInfo">Set systemInfo</param>
        /// <param name="commandConnectionType">Set Command Connection Type</param>
        /// <param name="turnBasedConnectionType">Set TurnBased Connection Type</param>
        /// <exception cref="GameServiceException">May GameServiceException Occur</exception>
        public GameServiceClientConfiguration(string clientId, string clientSecret, SystemInfo systemInfo,
            ConnectionType commandConnectionType = ConnectionType.NotSet,
            ConnectionType turnBasedConnectionType = ConnectionType.NotSet)
        {
            CommandConnectionType = commandConnectionType;
            TurnBasedConnectionType = turnBasedConnectionType;

            ClientId = string.IsNullOrEmpty(clientId)
                ? throw new GameServiceException("ClientId Cant Be Empty").LogException<GameServiceClientConfiguration>(
                    DebugLocation.Internal, "Constructor")
                : ClientId = clientId;
            ClientSecret = string.IsNullOrEmpty(clientSecret)
                ? throw new GameServiceException("ClientSecret Cant Be Empty")
                    .LogException<GameServiceClientConfiguration>(DebugLocation.Internal, "Constructor")
                : ClientSecret = clientSecret;
            SystemInfo = systemInfo == null
                ? throw new GameServiceException("SystemInfo Cant Be NULL")
                    .LogException<GameServiceClientConfiguration>(DebugLocation.Internal, "Constructor")
                : SystemInfo = systemInfo;
            if (systemInfo.DeviceUniqueId == null)
                throw new GameServiceException("DeviceUniqueId In SystemInfo Cant Be NULL")
                    .LogException<GameServiceClientConfiguration>(DebugLocation.Internal, "Constructor");
        }
    }
}