using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public enum LoginResponse
    {
        Success,
        EmptyLogin,
        UserDoesNotExist,
        WrongPassword,
        AlreadyLoggedIn,
        ServerError
    }

    public enum RegisterResponse
    {
        Success,
        EmptyName,
        EmptyPassword,
        InvalidPassword,
        NameAlreadyTaken,
        ServerError
    }

    public static class CurrentPlayer
    {
        private static LeaderboardPlayer _leaderboardPlayer;

        public static LeaderboardPlayer LeaderboardPlayer => _leaderboardPlayer;

        public static async UniTask<bool> TrySetAvatar(PlayerAvatar playerAvatar)
        {
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("action", "setAvatar"),
                new MultipartFormDataSection("hue", playerAvatar.Hue.ToString("0.0000")),
                new MultipartFormDataSection("saturation", playerAvatar.Saturation.ToString("0.0000"))
            };
            var setAvatarRequest = UnityWebRequest.Post(ServerSettings.ActionUri, formData);

            await setAvatarRequest.SendWebRequest();

            if (setAvatarRequest.downloadHandler.text == "0")
            {
                return true;
            }

            Debug.LogError("Add points failed: " + setAvatarRequest.downloadHandler.text);
            return false;
        }

        public static async UniTask<bool> TryAddPoints(int pointsToAdd)
        {
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("action", "addScore"),
                new MultipartFormDataSection("addScore", pointsToAdd.ToString())
            };
            var addPointsRequest = UnityWebRequest.Post(ServerSettings.ActionUri, formData);

            await addPointsRequest.SendWebRequest();

            if (addPointsRequest.downloadHandler.text == "0")
            {
                return true;
            }

            Debug.LogError("Add points failed: " + addPointsRequest.downloadHandler.text);
            return false;
        }

        public static async UniTask<LoginResponse> TryLogin(string login, string password)
        {
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("action", "login"),
                new MultipartFormDataSection("username", login),
                new MultipartFormDataSection("password", password)
            };
            var loginRequest = UnityWebRequest.Post(ServerSettings.ActionUri, formData);

            await loginRequest.SendWebRequest();

            if (loginRequest.result == UnityWebRequest.Result.ConnectionError ||
                loginRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new Exception(loginRequest.error);
            }

            var serverResponse = loginRequest.downloadHandler.text;

            if (LeaderboardPlayer.TryParseFromServerResponse(serverResponse, out var leaderboardPlayer))
            {
                _leaderboardPlayer = leaderboardPlayer;
                SessionHandler.Create();
                return LoginResponse.Success;
            }

            switch (serverResponse)
            {
                case "0":
                    return LoginResponse.EmptyLogin;
                case "1":
                    return LoginResponse.WrongPassword;
                case "2":
                    return LoginResponse.UserDoesNotExist;
                case "4":
                    return LoginResponse.AlreadyLoggedIn;
                default:
                    Debug.LogError("Server login response: " + serverResponse);
                    return LoginResponse.ServerError;
            }
        }

        public static async UniTask<RegisterResponse> TryRegister(string login, string password)
        {
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("action", "register"),
                new MultipartFormDataSection("username", login),
                new MultipartFormDataSection("password", password)
            };

            var registerRequest = UnityWebRequest.Post(ServerSettings.ActionUri, formData);

            await registerRequest.SendWebRequest();

            if (registerRequest.result == UnityWebRequest.Result.ConnectionError ||
                registerRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new Exception(registerRequest.error);
            }

            var serverResponse = registerRequest.downloadHandler.text;

            switch (serverResponse)
            {
                case "10":
                    return RegisterResponse.Success;
                case "0":
                    return RegisterResponse.EmptyName;
                case "1":
                    return RegisterResponse.NameAlreadyTaken;
                case "2":
                    return RegisterResponse.EmptyPassword;
                case "3":
                    return RegisterResponse.InvalidPassword;
                default:
                    Debug.LogError("Server login response: " + serverResponse);
                    return RegisterResponse.ServerError;
            }
        }
    }
}