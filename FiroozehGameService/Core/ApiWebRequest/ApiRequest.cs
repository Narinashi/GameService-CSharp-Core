// <copyright file="ApiRequest.cs" company="Firoozeh Technology LTD">
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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FiroozehGameService.Builder;
using FiroozehGameService.Core.Social;
using FiroozehGameService.Models;
using FiroozehGameService.Models.BasicApi;
using FiroozehGameService.Models.BasicApi.DBaaS;
using FiroozehGameService.Models.BasicApi.DBaaS.Options;
using FiroozehGameService.Models.BasicApi.FaaS;
using FiroozehGameService.Models.BasicApi.Social;
using FiroozehGameService.Models.BasicApi.TResponse;
using FiroozehGameService.Models.Consts;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.Internal;
using FiroozehGameService.Utils;
using Newtonsoft.Json;
using EditUserProfile = FiroozehGameService.Models.BasicApi.EditUserProfile;
using Game = FiroozehGameService.Models.BasicApi.Game;
using Party = FiroozehGameService.Models.BasicApi.Social.Party;

namespace FiroozehGameService.Core.ApiWebRequest
{
    internal static class ApiRequest
    {
        private const string Tag = "ApiRequest";

        private static string Pt => GameService.PlayToken;
        private static string Ut => GameService.UserToken;
        private static GameServiceClientConfiguration Configuration => GameService.Configuration;

        internal static async Task<AssetInfo> GetAssetInfo(string gameId, string tag)
        {
            var url = Api.BaseUrl2 + "/game/" + gameId + "/datapack/?tag=" + tag;
            var body = JsonConvert.SerializeObject(CreateDataPackDictionary());
            var response = await GsWebRequest.Post(url, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                        .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetAssetInfo");

                var info = JsonConvert.DeserializeObject<AssetInfo>(await reader.ReadToEndAsync());
                return info;
            }
        }


        internal static async Task<Login> Authorize()
        {
            var body = JsonConvert.SerializeObject(CreateAuthorizationDictionary(Ut));
            var response = await GsWebRequest.Post(Api.Start, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "Authorize");
            }
        }


        internal static async Task<Login> LoginAsGuest()
        {
            var body = JsonConvert.SerializeObject(CreateLoginDictionary(null, null, null, true));
            var response = await GsWebRequest.Post(Api.LoginUser, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "LoginAsGuest");
            }
        }


        internal static async Task<Login> Login(string email, string password)
        {
            var body = JsonConvert.SerializeObject(CreateLoginDictionary(email, password, null, false));
            var response = await GsWebRequest.Post(Api.LoginUser, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "Login");
            }
        }


        internal static async Task<Login> LoginWithGoogle(string idToken)
        {
            var body = JsonConvert.SerializeObject(CreateGoogleLoginDictionary(idToken));
            var response = await GsWebRequest.Post(Api.LoginWithGoogle, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "LoginWithGoogle");
            }
        }


        internal static async Task<Login> LoginWithPhoneNumber(string name, string phoneNumber, string code)
        {
            var body = JsonConvert.SerializeObject(CreatePhoneLoginDictionary(name, code, phoneNumber));
            var response = await GsWebRequest.Post(Api.LoginWithPhoneNumber + "/callback", body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "LoginWithPhoneNumber");
            }
        }


        internal static async Task<bool> SendLoginCodeWithSms(string phoneNumber)
        {
            var body = JsonConvert.SerializeObject(CreateSendSmsDictionary(phoneNumber));
            var response = await GsWebRequest.Post(Api.LoginWithPhoneNumber, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SendLoginCodeWithSms");
            }
        }


        internal static async Task<Login> SignUp(string nickName, string email, string password)
        {
            var body = JsonConvert.SerializeObject(CreateLoginDictionary(email, password, nickName, false));
            var response = await GsWebRequest.Post(Api.LoginUser, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Login>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SignUp");
            }
        }


        internal static async Task<List<LeaderBoard>> GetLeaderBoard()
        {
            var response = await GsWebRequest.Get(Api.GetLeaderBoard, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TLeaderBoard>(await reader.ReadToEndAsync())
                        .LeaderBoards;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetLeaderBoard");
            }
        }


        internal static async Task<List<Achievement>> GetAchievements()
        {
            var response = await GsWebRequest.Get(Api.GetAchievements, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TAchievement>(await reader.ReadToEndAsync())
                        .Achievements;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetAchievements");
            }
        }


        internal static async Task<T> GetSaveGame<T>(string saveName)
        {
            var response = await GsWebRequest.Get(Api.SaveGame + saveName, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetSaveGame");
            }
        }


        internal static async Task<Member> GetCurrentPlayer()
        {
            var response = await GsWebRequest.Get(Api.GetMemberData, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TMember>(await reader.ReadToEndAsync())
                        .Member;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetCurrentPlayer");
            }
        }


        internal static async Task<Game> GetCurrentGame()
        {
            var response = await GsWebRequest.Get(Api.GetCurrentGame, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Game>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetCurrentGame");
            }
        }


        internal static async Task<Score> GetCurrentPlayerScore(string leaderBoardId)
        {
            var response = await GsWebRequest.Get(Api.UseLeaderBoard + leaderBoardId + "/me", CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Score>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetCurrentPlayerScore");
            }
        }


        internal static async Task<User> GetUserData(string userId)
        {
            var response = await GsWebRequest.Get(Api.GetUserData + userId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<User>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetUserData");
            }
        }


        internal static async Task<Member> GetMemberData(string memberId)
        {
            var response = await GsWebRequest.Get(Api.GetMemberData + memberId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Member>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetMemberData");
            }
        }


        internal static async Task<MemberInfo> GetLastLoginMemberInfo()
        {
            var body = JsonConvert.SerializeObject(CreateLastLoginDictionary());
            var response = await GsWebRequest.Post(Api.GetLastLoginInfo, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<MemberInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetLastLoginMemberInfo");
            }
        }


        internal static async Task<MemberInfo> EditCurrentPlayer(EditUserProfile editUserProfile)
        {
            if (editUserProfile.Logo != null)
                await ImageUtil.UploadProfileImage(editUserProfile.Logo);

            var body = JsonConvert.SerializeObject(new Models.Internal.EditUserProfile
            {
                NickName = editUserProfile.NickName,
                PhoneNumber = editUserProfile.PhoneNumber,
                Email = editUserProfile.Email
            }, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Put(Api.GetMemberData, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<MemberInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "EditCurrentPlayer");
            }
        }


        internal static async Task<MemberInfo> SetCurrentPlayerTags(List<string> tags)
        {
            var body = JsonConvert.SerializeObject(new Models.Internal.EditUserProfile
            {
                Tags = tags
            }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Put(Api.GetMemberData, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<MemberInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetCurrentPlayerTags");
            }
        }

        internal static async Task<MemberInfo> SetCurrentPlayerLabel(string label)
        {
            var body = JsonConvert.SerializeObject(new Models.Internal.EditUserProfile
            {
                Label = label
            }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Put(Api.GetMemberData, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<MemberInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetCurrentPlayerLabel");
            }
        }


        internal static async Task<MemberInfo> SetCurrentPlayerGlobalProperty(string globalProperty)
        {
            var body = JsonConvert.SerializeObject(new Models.Internal.EditUserProfile
            {
                GlobalProperty = globalProperty
            }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Put(Api.GetMemberData, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<MemberInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetCurrentPlayerGlobalProperty");
            }
        }


        internal static async Task<List<TItem>> GetTableItems<TItem>(string tableId, bool isGlobal,
            TableOption[] options)
        {
            var url = UrlUtil.ParseDBaaSUrl(tableId, isGlobal, options);
            var response = await GsWebRequest.Get(url, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<TItem>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetTableItems");
            }
        }


        internal static async Task<TableResult<TItem>> GetTableItems<TItem>(TableAggregation aggregation)
        {
            var response = await GsWebRequest.Post(Api.Table + aggregation.Builder.TableId + "/aggregation",
                aggregation.Builder.AggregationData, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TableResult<TItem>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetTableItems");
            }
        }


        internal static async Task<TItem> GetTableItem<TItem>(string tableId, string itemId, bool isGlobal)
        {
            string url;
            if (isGlobal) url = Api.TableNonPermission + tableId + '/' + itemId;
            else url = Api.Table + tableId + '/' + itemId;

            var response = await GsWebRequest.Get(url, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ItemT<TItem>>(await reader.ReadToEndAsync()).ItemData;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetTableItem");
            }
        }


        internal static async Task<LeaderBoardDetails> GetLeaderBoardDetails(string leaderBoardId, int scoreLimit = 10,
            bool onlyFriends = false)
        {
            var url = Api.UseLeaderBoard + leaderBoardId;
            if (onlyFriends) url += "/friends";
            url += "?limit=" + scoreLimit;

            var response = await GsWebRequest.Get(url, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<LeaderBoardDetails>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetLeaderBoardDetails");
            }
        }


        internal static async Task<SaveDetails> SaveGame(string saveGameName, object saveGameObject)
        {
            var body = JsonConvert.SerializeObject(CreateSaveGameDictionary(saveGameName
                , JsonConvert.SerializeObject(saveGameObject, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore})));

            var response = await GsWebRequest.Post(Api.SaveGame, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TSave>(await reader.ReadToEndAsync()).SaveDetails;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SaveGame");
            }
        }


        internal static async Task<SubmitScoreResponse> SubmitScore(string leaderBoardId, int scoreValue)
        {
            var url = Api.UseLeaderBoard + leaderBoardId;
            var body = JsonConvert.SerializeObject(CreateSubmitScoreDictionary(scoreValue));

            var response = await GsWebRequest.Post(url, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TSubmitScore>(await reader.ReadToEndAsync())
                        .SubmitScoreResponse;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SubmitScore");
            }
        }


        internal static async Task<Achievement> UnlockAchievement(string achievementId)
        {
            var url = Api.UnlockAchievements + achievementId;
            var response = await GsWebRequest.Post(url, headers: CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TUnlockAchievement>(await reader.ReadToEndAsync())
                        .Achievement;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "UnlockAchievement");
            }
        }


        internal static async Task<TItem> UpdateTableItem<TItem>(string tableId, string itemId,
            TItem editedItem)
        {
            var url = Api.Table + tableId + '/' + itemId;
            var body = JsonConvert.SerializeObject(editedItem, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Put(url, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ItemT<TItem>>(await reader.ReadToEndAsync()).ItemData;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "UpdateTableItem");
            }
        }

        internal static async Task<TItem> AddItemToTable<TItem>(string tableId, TItem newItem)
        {
            var url = Api.Table + tableId;
            var body = JsonConvert.SerializeObject(newItem, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var response = await GsWebRequest.Post(url, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ItemT<TItem>>(await reader.ReadToEndAsync()).ItemData;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "AddItemToTable");
            }
        }


        internal static async Task<bool> RemoveSave(string saveName)
        {
            var response = await GsWebRequest.Delete(Api.SaveGame + saveName, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TSave>(await reader.ReadToEndAsync())
                        .Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "RemoveLastSave");
            }
        }

        internal static async Task<bool> DeleteAllTableItems(string tableId)
        {
            var url = Api.Table + tableId;

            var response = await GsWebRequest.Delete(url, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TSave>(await reader.ReadToEndAsync())
                        .Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteAllTableItems");
            }
        }

        internal static async Task<bool> DeleteTableItem(string tableId, string itemId)
        {
            var url = Api.Table + tableId + '/' + itemId;

            var response = await GsWebRequest.Delete(url, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TSave>(await reader.ReadToEndAsync())
                        .Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteTableItem");
            }
        }

        internal static async Task<ImageUploadResult> UploadUserProfileLogo(byte[] imageBuffer)
        {
            const string url = Api.UserProfileLogo;

            var response = await GsWebRequest.DoMultiPartPost(url, imageBuffer, CreateUserTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                var data = await reader.ReadToEndAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ImageUploadResult>(data);
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(data).Message)
                    .LogException(typeof(ApiRequest), DebugLocation.Http, "UploadUserProfileLogo");
            }
        }

        internal static async Task<string> GetCurrentServerTime()
        {
            var response = await GsWebRequest.Get(Api.CurrentTime);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return await reader.ReadToEndAsync();
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetCurrentServerTime");
            }
        }


        internal static async Task<FaaSResponse<TOutput>> ExecuteCloudFunction<TOutput, TInput>(string functionId,
            TInput functionInputClass, bool isPublic) where TInput : FaaSCore where TOutput : FaaSCore
        {
            var body = JsonConvert.SerializeObject(functionInputClass, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            HttpResponseMessage response;
            if (isPublic)
                response = await GsWebRequest.Post(Api.FaaS + Configuration.ClientId + "/" + functionId, body);
            else
                response = await GsWebRequest.Post(Api.FaaS + Configuration.ClientId + "/" + functionId, body,
                    CreatePlayTokenHeader());


            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<FaaSResponse<TOutput>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert
                    .DeserializeObject<FaaSResponse<TOutput>>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "ExecuteCloudFunction");
            }
        }


        internal static async Task<bool> CheckPhoneLoginStatus()
        {
            var body = JsonConvert.SerializeObject(CreateSendSmsDictionary());
            var response = await GsWebRequest.Put(Api.LoginWithPhoneNumber, body);

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
            }
        }


        internal static async Task<List<Event>> GetAllEvents()
        {
            var response = await GsWebRequest.Get(Api.GetEvents, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<Event>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetAllEvents");
            }
        }


        internal static async Task<Results<Member>> GetAllMembers(string urlQuery)
        {
            var response = await GsWebRequest.Get(Api.Friends + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Results<Member>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetAllMembers");
            }
        }


        internal static async Task<Results<FriendData>> GetMyFriends(string urlQuery)
        {
            var response = await GsWebRequest.Get(Api.GetMyFriends + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Results<FriendData>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetMyFriends");
            }
        }


        internal static async Task<Results<FriendData>> GetFriendPendingRequests(string urlQuery)
        {
            var response = await GsWebRequest.Get(Api.GetFriendRequests + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Results<FriendData>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetFriendPendingRequests");
            }
        }


        internal static async Task<bool> FriendRequest(string memberId)
        {
            var response = await GsWebRequest.Post(Api.Friends + memberId, null, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "FriendRequest");
            }
        }


        internal static async Task<bool> AcceptFriendRequest(string memberId)
        {
            var response = await GsWebRequest.Put(Api.Friends + memberId, null, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "AcceptFriendRequest");
            }
        }


        internal static async Task<bool> DeleteFriend(string memberId)
        {
            var response = await GsWebRequest.Delete(Api.Friends + memberId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteFriend");
            }
        }


        internal static async Task<Results<Party>> GetAllParties(string urlQuery)
        {
            var response = await GsWebRequest.Get(Api.GetAllParties + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Results<Party>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetAllParties");
            }
        }


        internal static async Task<Results<Party>> GetMyParties(string urlQuery)
        {
            var response = await GsWebRequest.Get(Api.GetMyParties + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Results<Party>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetMyParties");
            }
        }


        internal static async Task<PartyInfo> GetPartyInfo(string partyId)
        {
            var response = await GsWebRequest.Get(Api.Parties + partyId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<PartyInfo>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetPartyInfo");
            }
        }

        internal static async Task<ImageUploadResult> UploadPartyLogo(byte[] imageBuffer, string partyId)
        {
            var response = await GsWebRequest.DoMultiPartPost(Api.PartyImage + partyId + "/image", imageBuffer,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                var data = await reader.ReadToEndAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<ImageUploadResult>(data);
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(data).Message)
                    .LogException(typeof(ApiRequest), DebugLocation.Http, "UploadPartyLogo");
            }
        }

        internal static async Task<Party> CreateParty(SocialOptions.PartyOption option)
        {
            var body = JsonConvert.SerializeObject(new CreateParty
            {
                Name = option.Name,
                Description = option.Description,
                Max = option.MaxMember
            }, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });


            var response = await GsWebRequest.Post(Api.GetAllParties, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (!response.IsSuccessStatusCode)
                    throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                        .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "CreateParty");

                var party = JsonConvert.DeserializeObject<Party>(await reader.ReadToEndAsync());
                if (option.Logo == null) return party;
                await ImageUtil.UploadPartyLogo(option.Logo, party.Id);
                return party;
            }
        }

        internal static async Task<bool> SetOrUpdateRole(string partyId, string memberId, string role)
        {
            var body = JsonConvert.SerializeObject(new SetOrUpdateRole {Role = role});

            var response = await GsWebRequest.Put(Api.Parties + partyId + "/member/" + memberId, body,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await reader.ReadToEndAsync();
                    Console.WriteLine(data);
                    return JsonConvert.DeserializeObject<Error>(data).Status;
                }

                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetOrUpdateRole");
            }
        }

        internal static async Task<Party> SetOrUpdateVariable(string partyId, KeyValuePair<string, string> valuePair)
        {
            var body = JsonConvert.SerializeObject(new SetOrUpdateVariable
            {
                Name = valuePair.Key,
                Value = valuePair.Value
            });

            var response = await GsWebRequest.Put(Api.Parties + partyId + "/values", body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Party>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetOrUpdateVariable");
            }
        }

        internal static async Task<bool> SetOrUpdateMemberVariable(string partyId,
            KeyValuePair<string, string> valuePair)
        {
            var body = JsonConvert.SerializeObject(new SetOrUpdateVariable
            {
                Name = valuePair.Key,
                Value = valuePair.Value
            }, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            var response =
                await GsWebRequest.Put(Api.Parties + partyId + "/values/member", body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SetOrUpdateMemberVariable");
            }
        }

        internal static async Task<Party> DeleteVariable(string partyId, string variableKey)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/values/global/" + variableKey,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Party>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteVariable");
            }
        }

        internal static async Task<bool> DeleteMemberVariable(string partyId, string variableKey)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/values/member/" + variableKey,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteMemberVariable");
            }
        }


        internal static async Task<bool> DeleteMemberVariable(string partyId, string memberId, string variableKey)
        {
            var response = await GsWebRequest.Delete(
                Api.Parties + partyId + "/member/" + memberId + "/values/" + variableKey,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteMemberVariable");
            }
        }

        internal static async Task<Party> DeleteVariables(string partyId)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/values", CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Party>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteVariables");
            }
        }


        internal static async Task<bool> DeleteMemberVariables(string partyId)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/values/member", CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteMemberVariables");
            }
        }


        internal static async Task<bool> DeleteMemberVariables(string partyId, string memberId)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/member/" + memberId + "/values",
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteMemberVariables");
            }
        }


        internal static async Task<Party> EditParty(string partyId, SocialOptions.PartyOption option)
        {
            if (option.Logo != null)
                await ImageUtil.UploadPartyLogo(option.Logo, partyId);

            var body = JsonConvert.SerializeObject(new CreateParty
            {
                Name = option.Name,
                Description = option.Description
            }, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });


            var response = await GsWebRequest.Put(Api.Parties + partyId, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Party>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "EditParty");
            }
        }


        internal static async Task<bool> KickMember(string partyId, string memberId)
        {
            var response =
                await GsWebRequest.Delete(Api.Parties + partyId + "/member/" + memberId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "KickMember");
            }
        }


        internal static async Task<bool> AcceptJoinToParty(string partyId, string memberId)
        {
            var response = await GsWebRequest.Post(Api.Parties + partyId + "/member/" + memberId + "/accept", null,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "AcceptJoinToParty");
            }
        }


        internal static async Task<bool> RejectJoinToParty(string partyId, string memberId)
        {
            var response = await GsWebRequest.Post(Api.Parties + partyId + "/member/" + memberId + "/reject", null,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "RejectJoinToParty");
            }
        }


        internal static async Task<bool> SendJoinRequestToParty(string partyId)
        {
            var response = await GsWebRequest.Post(Api.Parties + partyId, null, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "SendJoinRequestToParty");
            }
        }


        internal static async Task<bool> LeaveParty(string partyId)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "LeaveParty");
            }
        }


        internal static async Task<bool> DeleteParty(string partyId)
        {
            var response = await GsWebRequest.Delete(Api.Parties + partyId + "/delete", CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "DeleteParty");
            }
        }


        internal static async Task<bool> AddFriendToParty(string partyId, string memberId)
        {
            var response = await GsWebRequest.Post(Api.Parties + partyId + "/member/" + memberId + "/join", null,
                CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "AddFriendToParty");
            }
        }


        internal static async Task<List<Member>> GetPartyPendingRequests(string partyId, string urlQuery)
        {
            var response =
                await GsWebRequest.Get(Api.Parties + partyId + "/pending" + urlQuery, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<Member>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetPartyPendingRequests");
            }
        }


        internal static async Task<Dictionary<string, string>> GetMemberVariables(string partyId)
        {
            var response = await GsWebRequest.Get(Api.Parties + partyId + "/values/member", CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetMemberVariables");
            }
        }


        internal static async Task<List<ActiveDevice>> GetActiveDevices()
        {
            var response = await GsWebRequest.Get(Api.Devices, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<ActiveDevice>>(await reader.ReadToEndAsync());
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "GetActiveDevices");
            }
        }


        internal static async Task<bool> RevokeDevice(string deviceId)
        {
            var response = await GsWebRequest.Delete(Api.Devices + '/' + deviceId, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "RevokeDevice");
            }
        }


        internal static async Task<bool> ChangePassword(string currentPass, string newPass)
        {
            var body = JsonConvert.SerializeObject(new ChangePassword
            {
                CurrentPass = currentPass, NewPass = newPass
            });

            var response = await GsWebRequest.Post(Api.ChangePassword, body, CreatePlayTokenHeader());

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync()).Status;
                throw new GameServiceException(JsonConvert.DeserializeObject<Error>(await reader.ReadToEndAsync())
                    .Message).LogException(typeof(ApiRequest), DebugLocation.Http, "ChangePassword");
            }
        }

        private static Dictionary<string, object> CreateLoginDictionary(string email, string password, string nickname,
            bool isGuest)
        {
            var param = new Dictionary<string, object>();

            if (isGuest)
            {
                param.Add("mode", "guest");
            }
            else if (nickname == null)
            {
                param.Add("mode", "login");
                param.Add("email", email);
                param.Add("password", password);
            }
            else
            {
                param.Add("name", nickname);
                param.Add("email", email);
                param.Add("password", password);
                param.Add("mode", "register");
            }

            param.Add("device_id", Configuration.SystemInfo.DeviceUniqueId);
            param.Add("client_id", Configuration.ClientId);
            return param;
        }


        private static Dictionary<string, object> CreateGoogleLoginDictionary(string idToken)
        {
            return new Dictionary<string, object>
                {{"token", idToken}, {"device_id", Configuration.SystemInfo.DeviceUniqueId}};
        }


        private static Dictionary<string, object> CreateSendSmsDictionary(string phoneNumber = null)
        {
            return new Dictionary<string, object>
            {
                {"game", Configuration.ClientId},
                {"secret", Configuration.ClientSecret},
                {"phone_number", phoneNumber}
            };
        }


        private static Dictionary<string, object> CreateLastLoginDictionary()
        {
            return new Dictionary<string, object>
            {
                {"game", Configuration.ClientId},
                {"secret", Configuration.ClientSecret},
                {"device_id", Configuration.SystemInfo.DeviceUniqueId}
            };
        }


        private static Dictionary<string, object> CreatePhoneLoginDictionary(string name, string code,
            string phoneNumber)
        {
            return new Dictionary<string, object>
            {
                {"device_id", Configuration.SystemInfo.DeviceUniqueId},
                {"phone_number", phoneNumber},
                {"code", code},
                {"name", name}
            };
        }


        private static Dictionary<string, object> CreateDataPackDictionary()
        {
            return new Dictionary<string, object>
            {
                {"secret", Configuration.ClientSecret}
            };
        }

        private static Dictionary<string, object> CreateAuthorizationDictionary(string userToken)
        {
            var param = new Dictionary<string, object>
            {
                {"token", userToken},
                {"game", Configuration.ClientId},
                {"secret", Configuration.ClientSecret},
                {"system_info", JsonConvert.SerializeObject(Configuration.SystemInfo)}
            };

            switch (Configuration.CommandConnectionType)
            {
                case ConnectionType.NotSet:
                    param.Add("connectionType", "not-set");
                    break;
                case ConnectionType.Native:
                    param.Add("connectionType", "tcp-sec");
                    break;
                case ConnectionType.WebSocket:
                    param.Add("connectionType", "wss");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return param;
        }

        private static Dictionary<string, object> CreateSaveGameDictionary(string name, string data)
        {
            return new Dictionary<string, object>
            {
                {"name", name},
                {"data", data}
            };
        }

        private static Dictionary<string, object> CreateSubmitScoreDictionary(int score)
        {
            return new Dictionary<string, object>
            {
                {"value", score}
            };
        }

        private static Dictionary<string, string> CreatePlayTokenHeader()
        {
            return new Dictionary<string, string>
            {
                {"x-access-token", Pt}
            };
        }

        private static Dictionary<string, string> CreateUserTokenHeader()
        {
            return new Dictionary<string, string>
            {
                {"x-access-token", Ut}, {"client-id", Configuration.ClientId}
            };
        }
    }
}