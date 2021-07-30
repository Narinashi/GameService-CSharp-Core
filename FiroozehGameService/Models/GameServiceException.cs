﻿// <copyright file="GameServiceException.cs" company="Firoozeh Technology LTD">
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

using System;

/**

* @author Alireza Ghodrati

*/


namespace FiroozehGameService.Models
{
    /// <summary>
    ///     Represents Game Service Exception
    /// </summary>
    public class GameServiceException : Exception
    {
        /// <summary>
        ///     the Game Service Exception Main Constructor
        /// </summary>
        public GameServiceException()
            : base("A GameService Runtime error occurred!")
        {
        }

        /// <summary>
        ///     the Game Service Exception Message Constructor
        /// </summary>
        /// <param name="msg">the Exception Message</param>
        public GameServiceException(string msg)
            : base(msg)
        {
        }
    }
}