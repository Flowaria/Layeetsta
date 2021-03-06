﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LaChartzone.Web
{
    public class WebAPI
    {
        public WebAPI()
        {
            ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Ssl3;
        }

        private Auth auth = null;
        public async Task Login(string id, string password)
        {
            HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(@"https://la.schwarzer.wang/auth/login");
            r.Method = "GET";
            r.Timeout = 10000;
            r.ContentType = "application/json";
            r.Headers.Add("Authorization", String.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(id + ":" + password))));

            var response = await r.GetResponseAsync();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = await sr.ReadToEndAsync();
                var respond = JsonConvert.DeserializeObject<AuthRespond>(json);
                if(respond.Succeed)
                {
                    var a = new Auth();
                    a.Id = respond.Id;
                    a.AccessToken = respond.AccessToken;
                    auth = a;
                }
                else
                {
                    throw new LayestaWebAPIException(respond.ErrorCode);
                }
            }
        }

        public async Task<LevelListRespond> GetUserLevelList()
        {
            if(auth != null)
            {
                HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(@"https://la.schwarzer.wang/layestalevel/list/current");
                r.Method = "GET";
                r.Timeout = 10000;
                r.ContentType = "application/json";
                r.Headers.Add("Authorization", String.Format("Bearer {0}", auth.AccessToken));

                var response = await r.GetResponseAsync();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var json = await sr.ReadToEndAsync();
                    var respond = JsonConvert.DeserializeObject<LevelListRespond>(json);
                    if(respond.Succeed)
                    {
                        return respond;
                    }
                    else if(respond.ErrorCode == (int)ErrorCode.InvalidAuth)
                    {
                        throw new LayestaWebAPINeedLoginException();
                    }
                    else
                    {
                        throw new LayestaWebAPIException(respond.ErrorCode);
                    }
                }
            }
            else
            {
                throw new LayestaWebAPINeedLoginException();
            }
        }
    }

    public class Auth
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
    }

    public class AuthRespond
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public bool Succeed { get; set; }
        public int ErrorCode { get; set; }
    }

    public class LevelListRespond
    {
        public bool Succeed { get; set; }
        public List<LayestaLevel> Levels { get; set; }
        public int ErrorCode { get; set; }
    }

    public class LayestaLevel
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public string SongArtist { get; set; }
        public string Difficulties { get; set; }
        public int DownloadCount { get; set; }
        public bool ShouldDisplay { get; set; }
        public string Designer { get; set; }
    }

    public enum ErrorCode
    {
        NotSet = 0,
        InvalidAuth = -1,
        Banned = -2,

        //Auth
        SchwarzerUserNotExist = 100,
        WrongPassword = 101,
        UserIsNotLevelCreator = 102,
        OSSAuthError = 103,
        InvalidType = 104,
        LevelNotFound = 105,
        MissingInfo = 106,

        IntervalLimit = 201,
        LayestaUserNotFound = 202,
        InvalidDto = 203
    }

    public class LayestaWebAPIException : Exception
    {
        public int ErrorCode;

        public override string Message
        {
            get
            {
                return String.Format("'LayestaWebAPIException' has been Thrown - Error Code {0}", ErrorCode);
            }
        }

        public LayestaWebAPIException(int errorcode)
        {
            ErrorCode = errorcode;
        }
    }
    public class LayestaWebAPINeedLoginException : Exception
    {
        public override string Message
        {
            get
            {
                return String.Format("'LayestaWebAPINeedLoginException' has been Thrown");
            }
        }
    }
}
