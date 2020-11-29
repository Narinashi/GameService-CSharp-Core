// <copyright file="UrlUtil.cs" company="Firoozeh Technology LTD">
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


using FiroozehGameService.Models.BasicApi.Buckets;
using FiroozehGameService.Models.Consts;

namespace FiroozehGameService.Utils
{
    internal static class UrlUtil
    {
        internal static string ParseBucketUrl(string bucketId, BucketOption[] options)
        {
            var first = true;
            var url = Api.Bucket + bucketId;
            if (options == null) return url;

            url += "?";

            foreach (var option in options)
                if (first)
                {
                    // To Remove first &
                    url += option.GetParsedData().Substring(1);
                    first = false;
                }
                else
                {
                    url += option.GetParsedData();
                }

            return url;
        }
    }
}