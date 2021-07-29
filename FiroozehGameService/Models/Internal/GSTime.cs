// <copyright file="GSTime.cs" company="Firoozeh Technology LTD">
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

using System;

/**
* @author Alireza Ghodrati
*/

namespace FiroozehGameService.Models.Internal
{
    /// <summary>
    ///     Represents GameServiceTime Class
    /// </summary>
    [Serializable]
    public class GSTime
    {
        internal GSTime()
        {
        }

        /// <summary>
        ///     Gets the Current Server Time
        /// </summary>
        /// <value>the Current Server Time</value>
        public DateTimeOffset ServerTime { get; internal set; }


        /// <summary>
        ///     Gets the Current Device Time
        /// </summary>
        /// <value>the Current Device Time</value>
        public DateTimeOffset DeviceTime { get; internal set; }


        /// <summary>
        ///     Check DeviceTimeValid
        /// </summary>
        /// <value>Returns True if Device Time Is Valid(not grater than 2 sec With Server)</value>
        public bool IsDeviceTimeValid()
        {
            return Math.Abs(DeviceTime.Second - ServerTime.Second) < 2;
        }
    }
}