// <copyright file="GsLiveVoice.cs" company="Firoozeh Technology LTD">
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

using FiroozehGameService.Core.GSLive;
using FiroozehGameService.Handlers.Command.RequestHandlers.Voice;
using FiroozehGameService.Models;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Models.GSLive.Providers;
using FiroozehGameService.Utils;

namespace FiroozehGameService.Core.Providers.GSLive
{
    public class GsLiveVoice : GsLiveVoiceProvider
    {
        public override void CreateChannel(GSLiveMediaOption.VoiceOption option)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "CreateChannel");

            if (option == null)
                throw new GameServiceException("option Cant Be Null").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "CreateChannel");

            GameService.GSLive.GetGsHandler().CommandHandler.Send(CreateVoiceChannelHandler.Signature, option);
        }

        public override void JoinChannel(string channelKey)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "JoinChannel");

            if (string.IsNullOrEmpty(channelKey))
                throw new GameServiceException("channelKey Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "JoinChannel");

            if (channelKey.Length < VoiceConst.MinChannelKeyLength ||
                channelKey.Length > VoiceConst.MaxChannelKeyLength)
                throw new GameServiceException("channelKey must between " + VoiceConst.MinChannelKeyLength + " and " +
                                               VoiceConst.MaxChannelKeyLength + " Characters.")
                    .LogException<GsLiveVoice>(DebugLocation.Voice, "JoinChannel");


            GameService.GSLive.GetGsHandler().CommandHandler.Send(JoinVoiceChannelHandler.Signature, channelKey);
        }

        public override void LeaveChannel(string channelId)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "LeaveChannel");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "LeaveChannel");

            // TODO Must Joined Before , so check it here


            GameService.GSLive.GetGsHandler().CommandHandler.Send(LeaveVoiceChannelHandler.Signature, channelId);
        }

        public override void KickMember(string channelId, string memberId, bool permanent)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "KickMember");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "KickMember");

            if (string.IsNullOrEmpty(memberId))
                throw new GameServiceException("memberId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "KickMember");

            // TODO Must Joined Before , so check it here

            GameService.GSLive.GetGsHandler().CommandHandler.Send(KickMemberVoiceChannelHandler.Signature,
                new VoicePayload(channelId, memberId: memberId, isPermanent: permanent));
        }

        public override void DestroyChannel(string channelId)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "DestroyChannel");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "DestroyChannel");

            // TODO Must Joined Before , so check it here

            GameService.GSLive.GetGsHandler().CommandHandler.Send(DestroyVoiceChannelHandler.Signature, channelId);
        }

        public override void GetChannelInfo(string channelKey)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "GetChannelInfo");

            if (string.IsNullOrEmpty(channelKey))
                throw new GameServiceException("channelKey Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "GetChannelInfo");

            GameService.GSLive.GetGsHandler().CommandHandler.Send(GetVoiceChannelInfoHandler.Signature, channelKey);
        }

        public override void MuteSelf(string channelId, bool isMuted)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "MuteSelf");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "MuteSelf");

            // TODO Must Joined Before , so check it here


            GameService.GSLive.GetGsHandler().CommandHandler.Send(MuteLocalVoiceChannelHandler.Signature,
                new VoicePayload(channelId, isMute: isMuted));
        }

        public override void DeafenSelf(string channelId, bool isDeafened)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "DeafenSelf");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "DeafenSelf");

            // TODO Must Joined Before , so check it here

            GameService.GSLive.GetGsHandler().CommandHandler.Send(DestroyVoiceChannelHandler.Signature,
                new VoicePayload(channelId, isDeafen: isDeafened));
        }

        public override void Trickle(string channelId, string ice)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Trickle");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Trickle");

            if (string.IsNullOrEmpty(ice))
                throw new GameServiceException("ice Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Trickle");


            // TODO Must Joined Before , so check it here

            GameService.GSLive.GetGsHandler().CommandHandler.Send(TrickleVoiceChannelHandler.Signature,
                new VoicePayload(channelId, ice: ice));
        }

        public override void Offer(string channelId, string sdp)
        {
            if (GameService.IsGuest)
                throw new GameServiceException("This Function Not Working In Guest Mode").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Offer");

            if (string.IsNullOrEmpty(channelId))
                throw new GameServiceException("channelId Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Offer");

            if (string.IsNullOrEmpty(sdp))
                throw new GameServiceException("sdp Cant Be NullOrEmpty").LogException<GsLiveVoice>(
                    DebugLocation.Voice, "Offer");


            // TODO Must Joined Before , so check it here

            GameService.GSLive.GetGsHandler().CommandHandler.Send(OfferVoiceChannelHandler.Signature,
                new VoicePayload(channelId, sdp: sdp));
        }
    }
}