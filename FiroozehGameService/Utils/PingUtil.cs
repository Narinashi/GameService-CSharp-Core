// <copyright file="PingUtil.cs" company="Firoozeh Technology LTD">
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
using System.Timers;

namespace FiroozehGameService.Utils
{
    internal static class PingUtil
    {
        
        private const int Interval = 1000;
        private static long _lastPing = -1;
        internal static EventHandler RequestPing;
        private static Timer _timer;
        

        internal static void Init()
        {
            if (_timer != null) return;
            _timer = new Timer
            {
                Interval = Interval,
                Enabled = true
            };
            _timer.Elapsed += (sender, args) => { RequestPing?.Invoke(null, null); };
        }

        internal static long GetLastPing()
        {
            return _lastPing;
        }

        internal static void SetLastPing(long ping)
        {
            _lastPing = ping;
        }

        internal static long Diff(long one, long two)
        {
            return Math.Abs(one - two);
        }
    }
}