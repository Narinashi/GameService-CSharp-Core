﻿// <copyright file="MemberInfo.cs" company="Firoozeh Technology LTD">
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

using System;
using FiroozehGameService.Core;
using Newtonsoft.Json;

namespace FiroozehGameService.Models.BasicApi
{
    /// <summary>
    ///     Represents User Data Model In Game Service Basic API
    /// </summary>
    [Serializable]
    public class MemberInfo
    {
        /// <summary>
        ///     Gets the Member ID
        ///     You Can Use it In MultiPlayer Functions that Needs Member id
        /// </summary>
        /// <value>the Member ID</value>
        [JsonProperty("_id")] public string Id;

        /// <summary>
        ///     Gets the Member Logo URL.
        /// </summary>
        /// <value>the Member Logo URL</value>
        [JsonProperty("logo")] public string Logo;


        /// <summary>
        ///     Gets the Member Name.
        /// </summary>
        /// <value>the Member Name</value>
        [JsonProperty("name")] public string Name;

        
        /// <summary>
        ///     Gets the Member Phone Number.
        /// </summary>
        /// <value>the Member Phone Number</value>
        [JsonProperty("phone_number")] public string PhoneNumber;
        
        
        /// <summary>
        ///     Gets the Member Email.
        /// </summary>
        /// <value>the Member Email</value>
        [JsonProperty("email")] public string Email;
        

        public override bool Equals(object obj) => Id == (obj as MemberInfo)?.Id;
    }
}