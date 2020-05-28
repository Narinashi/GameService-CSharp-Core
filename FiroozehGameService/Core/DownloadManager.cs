// <copyright file="DownloadManager.cs" company="Firoozeh Technology LTD">
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
using System.Net;
using System.Threading.Tasks;
using FiroozehGameService.Builder;
using FiroozehGameService.Core.ApiWebRequest;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models.EventArgs;
using FiroozehGameService.Models.Internal;

/**
* @author Alireza Ghodrati
*/

namespace FiroozehGameService.Core
{
    /// <summary>
    ///     Represents Game Service DownloadManager
    /// </summary>
    internal class DownloadManager
    {
        internal DownloadManager(GameServiceClientConfiguration config)
        {
            _configuration = config;
        }

        internal async Task StartDownload(string tag)
        {
            try
            {
                var assetInfo = await ApiRequest.GetAssetInfo(_configuration.ClientId, tag);
                await StartDownloadWithInfo(assetInfo);
            }
            catch (Exception e)
            {
                DownloadEventHandlers.DownloadError?.Invoke(this, new ErrorArg
                {
                    Error = e.Message
                });
            }
        }

        internal async Task StartDownload(string tag, string path)
        {
            try
            {
                var assetInfo = await ApiRequest.GetAssetInfo(_configuration.ClientId, tag);
                StartDownloadWithInfo(assetInfo, path);
            }
            catch (Exception e)
            {
                DownloadEventHandlers.DownloadError?.Invoke(this, new ErrorArg
                {
                    Error = e.Message
                });
            }
        }


        internal async Task StartDownloadWithInfo(AssetInfo info)
        {
            try
            {
                _client = new WebClient();
                // Set Events
                _client.DownloadProgressChanged += (s, progress) =>
                {
                    DownloadEventHandlers.DownloadProgress?.Invoke(this, new DownloadProgressArgs
                    {
                        FileTag = info.AssetInfoData.Name,
                        BytesReceived = progress.BytesReceived,
                        TotalBytesToReceive = progress.TotalBytesToReceive,
                        ProgressPercentage = progress.ProgressPercentage
                    });
                };

                _client.DownloadDataCompleted += (sender, args) =>
                {
                    _client?.Dispose();
                    DownloadEventHandlers.DownloadCompleted?.Invoke(this, new DownloadCompleteArgs
                    {
                        DownloadedAssetAsBytes = args.Result
                    });
                };

                await _client?.DownloadDataTaskAsync(info.AssetInfoData.Link);
            }
            catch (Exception e)
            {
                DownloadEventHandlers.DownloadError?.Invoke(this, new ErrorArg
                {
                    Error = e.Message
                });
            }
        }

        internal void StartDownloadWithInfo(AssetInfo info, string path)
        {
            var completeAddress = path + '/' + info.AssetInfoData.Name;
            try
            {
                _client = new WebClient();
                // Set Events
                _client.DownloadProgressChanged += (s, progress) =>
                    DownloadEventHandlers.DownloadProgress?.Invoke(this, new DownloadProgressArgs
                    {
                        FileTag = info.AssetInfoData.Name,
                        BytesReceived = progress.BytesReceived,
                        TotalBytesToReceive = progress.TotalBytesToReceive,
                        ProgressPercentage = progress.ProgressPercentage
                    });


                _client.DownloadFileCompleted += (s, e) =>
                {
                    _client?.Dispose();
                    DownloadEventHandlers.DownloadCompleted?.Invoke(this, new DownloadCompleteArgs
                    {
                        DownloadedAssetPath = completeAddress
                    });
                };


                _client?.DownloadFileAsync(new Uri(info.AssetInfoData.Link), completeAddress);
            }
            catch (Exception e)
            {
                DownloadEventHandlers.DownloadError?.Invoke(this, new ErrorArg
                {
                    Error = e.Message
                });
            }
        }


        internal void StopAllDownloads()
        {
            _client.CancelAsync();
        }

        #region DownloadRegion

        private readonly GameServiceClientConfiguration _configuration;
        private WebClient _client;

        #endregion
    }
}