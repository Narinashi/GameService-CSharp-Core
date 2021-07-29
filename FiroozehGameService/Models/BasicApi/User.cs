﻿// <copyright file="User.cs" company="Firoozeh Technology LTD">
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
using FiroozehGameService.Models.BasicApi.Providers;
using Newtonsoft.Json;

namespace FiroozehGameService.Models.BasicApi
{
    /// <summary>
    ///     Represents User Data Model In Game Service Basic API
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary>
        ///     Gets the User Email.(NULLABLE)
        /// </summary>
        /// <value>the User Email</value>
        [JsonProperty("email")] public string Email;

        /// <summary>
        ///     Gets the User id.
        /// </summary>
        /// <value>the User id</value>
        [JsonProperty("_id")] public string Id;


        /// <summary>
        ///     get this User Is Guest or Not.
        ///     (Note : Only Works on <see cref="IPlayerProvider.GetCurrentPlayer" />)
        /// </summary>
        /// <value>this User Is Guest or Not</value>
        [JsonProperty("guest")] public bool IsGuest;


        /// <summary>
        ///     get this User Is Yours or Not.
        /// </summary>
        /// <value>this User Is Yours or Not</value>
        [JsonProperty("isMe")] public bool IsMe;

        /// <summary>
        ///     Gets the User Logo URL.
        /// </summary>
        /// <value>the User Logo URL</value>
        [JsonProperty("logo")] public string Logo;


        /// <summary>
        ///     Gets the User Name.
        /// </summary>
        /// <value>the User Name</value>
        [JsonProperty("name")] public string Name;


        /// <summary>
        ///     Gets the User Phone Number.(NULLABLE)
        /// </summary>
        /// <value>the User Phone Number</value>
        [JsonProperty("mobile")] public string PhoneNumber;


        /// <summary>
        ///     Gets the User Point.
        /// </summary>
        /// <value>the User Point</value>
        [JsonProperty("point")] public int Point;

        internal User()
        {
        }
    }
}