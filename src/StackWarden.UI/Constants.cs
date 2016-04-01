using System;
using System.Collections.Generic;
using System.Configuration;

namespace StackWarden.UI
{
    public static class Constants
    {
        public static readonly string ApplicationName = "StackWarden";
        public static readonly string OrganizationName = ConfigurationManager.AppSettings["Organization.Name"] ?? "Unknown Organization";

        public static class Icons
        {
            public static IDictionary<string, string> Map => new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { nameof(Default), "bolt" },
                { nameof(Start), "play" },
                { nameof(Stop), "stop" },
                { nameof(Restart), "refresh" },
                { nameof(Application), "desktop" },
                { nameof(Server), "server" },
                { nameof(Service), "terminal" },
                { nameof(Queue), "envelope-o" },
                { nameof(Loading), "spinner fa-spin" },
                { nameof(Database), "database" },
                { nameof(Http), "globe" },
                { nameof(Machine), "server" },
                { nameof(Log), "file-text-o" },
                { nameof(MessageQueue), "envelope-o" }
            };

            public static string Default => Map[nameof(Default)];
            public static string Start => Map[nameof(Start)];
            public static string Stop => Map[nameof(Stop)];
            public static string Restart => Map[nameof(Restart)];
            public static string Application => Map[nameof(Application)];
            public static string Server => Map[nameof(Server)];
            public static string Service => Map[nameof(Service)];
            public static string Queue => Map[nameof(Queue)];
            public static string Loading => Map[nameof(Loading)];
            public static string Database => Map[nameof(Database)];
            public static string Http => Map[nameof(Http)];
            public static string Machine => Map[nameof(Machine)];
            public static string Log => Map[nameof(Log)];
            public static string MessageQueue => Map[nameof(MessageQueue)];
        }

        public static class Monitor
        {
            public static readonly int TimeToLiveLeniency = int.Parse(ConfigurationManager.AppSettings["Monitor.TimeToLive.Leniency"] ?? "0");
        }
    }
}