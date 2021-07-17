// <copyright file="TurnBasedHandler.cs" company="Firoozeh Technology LTD">
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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FiroozehGameService.Core;
using FiroozehGameService.Core.Providers.GSLive;
using FiroozehGameService.Core.Socket;
using FiroozehGameService.Handlers.TurnBased.RequestHandlers;
using FiroozehGameService.Handlers.TurnBased.ResponseHandlers;
using FiroozehGameService.Models;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.Enums.GSLive;
using FiroozehGameService.Models.EventArgs;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Models.Internal;
using FiroozehGameService.Utils;
using FiroozehGameService.Utils.Encryptor;

namespace FiroozehGameService.Handlers.TurnBased
{
    internal class TurnBasedHandler
    {
        internal TurnBasedHandler(StartPayload payload)
        {
            switch (payload.Area.Protocol)
            {
                case GsEncryptor.TcpSec:
                    _tcpClient = new GsTcpClient();
                    break;
                case GsEncryptor.WebSocketSec:
                    _tcpClient = new GsWebSocketClient();
                    break;
            }

            CurrentRoom = payload.Room;
            _tcpClient.DataReceived += OnDataReceived;

            _cancellationToken = new CancellationTokenSource();
            _observer = new GsLiveSystemObserver(GSLiveType.TurnBased);
            _callerUtil = new ObjectCallerUtil(TurnBasedConst.ConnectivityCheckInterval, null);
            _isDisposed = false;


            // Set Internal Event Handlers
            TurnBasedEventHandlers.TurnBasedPing += OnPing;
            TurnBasedEventHandlers.TurnBasedAuthorized += OnAuth;
            TurnBasedEventHandlers.LeftDispose += OnLeftDispose;
            TurnBasedEventHandlers.GsTurnBasedClientConnected += OnGsTcpClientConnected;
            TurnBasedEventHandlers.GsTurnBasedClientError += OnGsTcpClientError;
            TurnBasedEventHandlers.TurnBasedMirror += TurnBasedMirror;
            _callerUtil.Caller += RequestPing;


            InitRequestMessageHandlers();
            InitResponseMessageHandlers();

            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "Constructor", "TurnBasedHandler Init");
        }

        private async void RequestPing(object sender, object e)
        {
            if (_isPingRequested)
            {
                DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "RequestPing"
                    , "TurnBasedHandler -> Server Not Response Ping, Reconnecting...");

                TurnBasedEventHandlers.Reconnected?.Invoke(null, ReconnectStatus.Connecting);

                _isPingRequested = false;

                Init();
                return;
            }

            _isPingRequested = true;

            await RequestAsync(MirrorHandler.Signature);
        }

        private void TurnBasedMirror(object sender, Packet packet)
        {
            _isPingRequested = false;
        }


        private void OnLeftDispose(object sender, EventArgs e)
        {
            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnLeftDispose",
                "Connection Gracefully Closed By Server, so Dispose TurnBased...");

            CoreEventHandlers.Dispose?.Invoke(this, new DisposeData
            {
                Type = GSLiveType.TurnBased, IsGraceful = false
            });
        }

        private void OnGsTcpClientError(object sender, GameServiceException exception)
        {
            if (_isDisposed) return;

            exception.LogException<TurnBasedHandler>(DebugLocation.TurnBased, "OnGsTcpClientError");

            if (PlayerHash != null) TurnBasedEventHandlers.Reconnected?.Invoke(null, ReconnectStatus.Connecting);

            _retryConnectCounter++;

            if (_retryConnectCounter >= TurnBasedConst.MaxRetryConnect)
            {
                DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnGsTcpClientError",
                    "TurnBasedHandler Reached to MaxRetryConnect , so dispose TurnBased...");

                CoreEventHandlers.Dispose?.Invoke(this, new DisposeData
                {
                    Type = GSLiveType.TurnBased, IsGraceful = false
                });
                return;
            }

            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnGsTcpClientError",
                "TurnBasedHandler reconnect Retry " + _retryConnectCounter + " , Wait to Connect...");

            Init();
        }

        private async void OnGsTcpClientConnected(object sender, object e)
        {
            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnGsTcpClientConnected",
                "TurnBasedHandler -> Connected,Waiting for Handshakes...");

            _retryConnectCounter = 0;

            _tcpClient?.StartReceiving();

            await RequestAsync(AuthorizationHandler.Signature);

            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnGsTcpClientConnected",
                "TurnBasedHandler Init done");
        }

        private void OnAuth(object sender, string playerHash)
        {
            // this is Reconnect
            if (PlayerHash != null || _isPingRequested)
                TurnBasedEventHandlers.Reconnected?.Invoke(null, ReconnectStatus.Connected);

            PlayerHash = playerHash;
            GsLiveTurnBased.InAutoMatch = false;
            _isPingRequested = false;

            _tcpClient?.StartSending();
            _callerUtil?.Start();

            DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "OnAuth", "TurnBasedHandler OnAuth Done");
        }

        private async void OnPing(object sender, object packet)
        {
            await RequestAsync(PingPongHandler.Signature);
        }

        private void InitRequestMessageHandlers()
        {
            _requestHandlers.Add(AuthorizationHandler.Signature, new AuthorizationHandler());
            _requestHandlers.Add(GetMemberHandler.Signature, new GetMemberHandler());
            _requestHandlers.Add(LeaveRoomHandler.Signature, new LeaveRoomHandler());
            _requestHandlers.Add(PingPongHandler.Signature, new PingPongHandler());
            _requestHandlers.Add(ChooseNextHandler.Signature, new ChooseNextHandler());
            _requestHandlers.Add(AcceptVoteHandler.Signature, new AcceptVoteHandler());
            _requestHandlers.Add(CurrentTurnHandler.Signature, new CurrentTurnHandler());
            _requestHandlers.Add(VoteHandler.Signature, new VoteHandler());
            _requestHandlers.Add(TakeTurnHandler.Signature, new TakeTurnHandler());
            _requestHandlers.Add(PropertyHandler.Signature, new PropertyHandler());
            _requestHandlers.Add(RoomInfoHandler.Signature, new RoomInfoHandler());
            _requestHandlers.Add(SnapshotHandler.Signature, new SnapshotHandler());
            _requestHandlers.Add(MirrorHandler.Signature, new MirrorHandler());
        }

        private void InitResponseMessageHandlers()
        {
            _responseHandlers.Add(AuthResponseHandler.ActionCommand, new AuthResponseHandler());
            _responseHandlers.Add(ErrorResponseHandler.ActionCommand, new ErrorResponseHandler());
            _responseHandlers.Add(JoinRoomResponseHandler.ActionCommand, new JoinRoomResponseHandler());
            _responseHandlers.Add(LeaveRoomResponseHandler.ActionCommand, new LeaveRoomResponseHandler());
            _responseHandlers.Add(MemberDetailsResponseHandler.ActionCommand, new MemberDetailsResponseHandler());
            _responseHandlers.Add(ChooseNextResponseHandler.ActionCommand, new ChooseNextResponseHandler());
            _responseHandlers.Add(AcceptVoteResponseHandler.ActionCommand, new AcceptVoteResponseHandler());
            _responseHandlers.Add(CurrentTurnResponseHandler.ActionCommand, new CurrentTurnResponseHandler());
            _responseHandlers.Add(VoteResponseHandler.ActionCommand, new VoteResponseHandler());
            _responseHandlers.Add(PingResponseHandler.ActionCommand, new PingResponseHandler());
            _responseHandlers.Add(TakeTurnResponseHandler.ActionCommand, new TakeTurnResponseHandler());
            _responseHandlers.Add(PropertyResponseHandler.ActionCommand, new PropertyResponseHandler());
            _responseHandlers.Add(RoomInfoResponseHandler.ActionCommand, new RoomInfoResponseHandler());
            _responseHandlers.Add(SnapShotResponseHandler.ActionCommand, new SnapShotResponseHandler());
            _responseHandlers.Add(MirrorResponseHandler.ActionCommand, new MirrorResponseHandler());
        }


        private async Task RequestAsync(string handlerName, object payload = null)
        {
            await SendAsync(_requestHandlers[handlerName]?.HandleAction(payload));
        }

        internal void Send(string handlerName, object payload = null)
        {
            AddToSendQueue(_requestHandlers[handlerName]?.HandleAction(payload));
        }


        private static async Task SendAsync(Packet packet)
        {
            if (IsAvailable()) await _tcpClient.SendAsync(packet);
        }


        private void AddToSendQueue(Packet packet)
        {
            if (!_observer.Increase(false))
                throw new GameServiceException("Too Many Requests, You Can Send " + TurnBasedConst.TurnBasedLimit +
                                               " Requests Per Second")
                    .LogException<TurnBasedHandler>(DebugLocation.TurnBased, "AddToSendQueue");

            if (!_isDisposed) _tcpClient.AddToSendQueue(packet);
            else
                throw new GameServiceException("TurnBased System Already Disposed")
                    .LogException<TurnBasedHandler>(DebugLocation.TurnBased, "AddToSendQueue");
        }


        public void Init()
        {
            _cancellationToken = new CancellationTokenSource();

            _callerUtil?.Stop();
            _tcpClient.Init(null, GameService.CommandInfo.Cipher);
        }

        public void Dispose(bool isGraceful)
        {
            try
            {
                if (_isDisposed)
                {
                    DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "Dispose",
                        "TurnBased System Already Disposed");
                    return;
                }

                _retryConnectCounter = 0;
                _isDisposed = true;
                _isPingRequested = false;

                _observer?.Dispose();
                _callerUtil?.Dispose();
                _cancellationToken?.Cancel(true);

                _tcpClient?.StopReceiving(isGraceful);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _tcpClient = null;
                CurrentRoom = null;
                PlayerHash = null;

                TurnBasedEventHandlers.TurnBasedAuthorized = null;
                TurnBasedEventHandlers.TurnBasedClientConnected = null;
                TurnBasedEventHandlers.GsTurnBasedClientConnected = null;
                TurnBasedEventHandlers.GsTurnBasedClientError = null;
                TurnBasedEventHandlers.TurnBasedPing = null;
                TurnBasedEventHandlers.LeftDispose = null;
                TurnBasedEventHandlers.TurnBasedMirror = null;

                try
                {
                    GC.SuppressFinalize(this);
                }
                catch (Exception)
                {
                    // ignored
                }

                DebugUtil.LogNormal<TurnBasedHandler>(DebugLocation.TurnBased, "Dispose",
                    "TurnBasedHandler Dispose Done");
            }
        }

        private void OnDataReceived(object sender, SocketDataReceived e)
        {
            try
            {
                if (e.Packet == null) return;

                var packet = (Packet) e.Packet;

                if (ActionUtil.IsInternalAction(packet.Action, GSLiveType.TurnBased))
                    _responseHandlers.GetValue(packet.Action)?.HandlePacket(packet);
                else
                    GameService.SynchronizationContext?.Send(
                        delegate { _responseHandlers.GetValue(packet.Action)?.HandlePacket(packet); }, null);
            }
            catch (Exception exception)
            {
                exception.LogException<TurnBasedHandler>(DebugLocation.TurnBased, "OnDataReceived");
            }
        }

        internal static bool IsAvailable()
        {
            return _tcpClient != null && _tcpClient.IsConnected();
        }

        #region TBHandlerRegion

        private static GTcpClient _tcpClient;
        internal static Room CurrentRoom;
        private readonly GsLiveSystemObserver _observer;
        private readonly ObjectCallerUtil _callerUtil;
        private CancellationTokenSource _cancellationToken;
        private int _retryConnectCounter;
        private bool _isDisposed;
        private bool _isPingRequested;

        internal static string PlayerHash { private set; get; }
        internal static string PlayToken => GameService.PlayToken;

        private readonly Dictionary<int, IResponseHandler> _responseHandlers =
            new Dictionary<int, IResponseHandler>();

        private readonly Dictionary<string, IRequestHandler> _requestHandlers =
            new Dictionary<string, IRequestHandler>();

        #endregion
    }
}