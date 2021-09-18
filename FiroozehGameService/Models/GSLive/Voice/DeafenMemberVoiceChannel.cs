// <copyright file="DeafenMemberVoiceChannel.cs" company="Firoozeh Technology LTD">
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
using Newtonsoft.Json;

namespace FiroozehGameService.Models.GSLive.Voice
{
    /// <summary>
    ///     Represents DeafenMemberVoiceChannel Data Model In GameService Voice System
    /// </summary>
    [Serializable]
    public class DeafenMemberVoiceChannel
    {
        /// <summary>
        ///     Gets the Voice Channel Member Deafen Status
        /// </summary>
        /// <value> the Voice Channel Member Deafen Status</value>
        [JsonProperty("2")] public bool IsDeafened;

        /// <summary>
        ///     Gets the Voice Channel Member
        /// </summary>
        /// <value>the Voice Channel Member</value>
        [JsonProperty("1")] public Member Member;

        /// <summary>
        ///     Gets the Voice Channel
        /// </summary>
        /// <value> the Voice Channel</value>
        [JsonProperty("0")] public VoiceChannel VoiceChannel;
    }
}