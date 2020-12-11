// <copyright file="Social.cs" company="Firoozeh Technology LTD">
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


using System.Collections.Generic;
using System.Threading.Tasks;
using FiroozehGameService.Core.ApiWebRequest;
using FiroozehGameService.Models;
using FiroozehGameService.Models.BasicApi;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Utils;



namespace FiroozehGameService.Core.Social
{
    /// <summary>
    ///  Represents GameService Social System
    /// </summary>
    public class Social
    {
        /// <summary>
        /// The GameService Friend System
        /// </summary>
        public Friend Friend;
        
        /// <summary>
        /// The GameService Party System
        /// </summary>
        public Party Party;


        /// <summary>
        /// With this command you can get All Event Data
        /// </summary>
        /// <returns></returns>
        public async Task<Results<Event>> GetAllEvents()
        {
            if (!GameService.IsAuthenticated()) throw new GameServiceException("GameService Not Available").LogException(typeof(Social),DebugLocation.Social,"GetAllEvents");
            return await ApiRequest.GetAllEvents();
        }
        
        internal Social()
        {
            Friend = new Friend();
            Party  = new Party();
        }
    }
}