# **graylog-utils-library**

## **Introduction**

C# base .net core `Graylog` utility class library. Main purpose to store logs in graylog server for better visualization and analyze of application logs

## Installation

- Clone <https://github.com/mdrajibul/graylog-utils-library.git>
- Checkout branch **master** (if not default)
- Open visual studio
- Build the project
- Add Graylog library depency in project.json file as "RAJ.GrayLog": "1.0.0-\*"

## Usage

Please add below json properties in appsettings.json under `Targets`

```json
{
  "TargetType": "Gelf",
  "Name": "graylog",
  "Endpoint": "",
  "Facility": "CURRENT_PROJECT",
  "SendLastFormatParameter": "true"
}
```

Your application class should be as below(as example)

```cs
using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using RAJ.GrayLog;

namespace RAJ.Common
{
    public static class Configure
    {
        public static void Nlog(AppLogConfiguration appLogConfiguration)
        {
            var loggingConfiguration = new LoggingConfiguration();
            var targetDictionary = new Dictionary<string, NLog.Targets.Target>();
            foreach (var target in appLogConfiguration.Targets)
            {
                NLog.Targets.Target configurerdTarget = null;

                switch (target.TargetType)
                {
                    case "File":
                        configurerdTarget = GetFileTarget(target);
                        break;
                    case "Console":
                        configurerdTarget = GetConsoleTarget(target);
                        break;
                    // Gralog implementation
                    case "Gelf":
                        configurerdTarget = GetGelfTarget(target);
                        break;
                    default:
                        configurerdTarget = new NullTarget(target.Name);
                        break;
                }

                loggingConfiguration.AddTarget(configurerdTarget);
                targetDictionary.Add(target.Name, configurerdTarget);
            }

            foreach (var rule in appLogConfiguration.Rules)
            {
                loggingConfiguration.LoggingRules.Add(GetRule(rule, targetDictionary[rule.WriteTo]));
            }

            LogManager.Configuration = loggingConfiguration;
        }

        private static ConsoleTarget GetConsoleTarget(Target target)
        {
            var consoleTarget = new ConsoleTarget(target.Name)
            {
                DetectConsoleAvailable = target.DetectConsole,
                Layout = target.Layout,
            };

            return consoleTarget;
        }

        private static LoggingRule GetRule(Rule rule, NLog.Targets.Target target)
        {
            return new LoggingRule(rule.Name, NLog.LogLevel.FromString(rule.MinLevel), target);
        }

        private static FileTarget GetFileTarget(Target target)
        {
            var logDirectory = Environment.GetEnvironmentVariable("LOG_DIRECTORY");
            var fileTarget = new FileTarget(target.Name)
            {
                FileName = string.IsNullOrEmpty(logDirectory) ? target.FileName : Path.Combine(logDirectory, target.FileName),
                Layout = target.Layout
            };
            return fileTarget;
        }
        // Gralog implementation
        private static GelfTarget GetGelfTarget(Target target)
        {
            var gelfTarget = new GelfTarget();
            gelfTarget.Name = target.Name;
            gelfTarget.EndPoint = target.EndPoint;
            gelfTarget.Facility = target.Facility;
            gelfTarget.SendLastFormatParameter = target.SendLastFormatParameter;

            return gelfTarget;
        }
    }
}

```
