using System;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace Costura_Decompressor
{
    public static class Logger
    {
        public static void Log(string text, LogType type)
        {
            const string template = "{0} {1}";
            switch (type)
            {
                case LogType.Success:
                {
                    Formatter[] success =
                    {
                        new Formatter("[>]", Color.Yellow),
                        new Formatter(text, Color.White)
                    };
                    Console.WriteLineFormatted(template, Color.Gray, success);
                    break;
                }
                case LogType.Info:
                {
                    Formatter[] info =
                    {
                        new Formatter("[i]", Color.DodgerBlue),
                        new Formatter(text, Color.White)
                    };
                    Console.WriteLineFormatted(template, Color.Gray, info);
                    break;
                }
                case LogType.Error:
                {
                    Formatter[] error =
                    {
                        new Formatter("[error]", Color.Red),
                        new Formatter(text, Color.Crimson)
                    };
                    Console.WriteLineFormatted(template, Color.Gray, error);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void Success(string message) => Log(message, LogType.Success);

        public static void Error(string message) => Log(message, LogType.Error);

        public static void Info(string message) => Log(message, LogType.Info);


        public enum LogType
        {
            Success = 0,
            Info = 1,
            Error = 3
        }
    }
}