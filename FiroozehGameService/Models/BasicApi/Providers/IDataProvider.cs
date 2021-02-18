﻿// <copyright file="IDataProvider.cs" company="Firoozeh Technology LTD">
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

using System.Threading.Tasks;
using FiroozehGameService.Models.Internal;

namespace FiroozehGameService.Models.BasicApi.Providers
{
    /// <summary>
    ///     Represents Data Provider In Game Service Basic API
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        ///     This command Gets The Current Times
        /// </summary>
        /// <value> return The Current Times</value>
        Task<GSTime> GetCurrentTime();


        /// <summary>
        ///     This command Gets The Current Game
        /// </summary>
        /// <value> return The Current Game</value>
        Task<Game> GetCurrentGame();
    }
}