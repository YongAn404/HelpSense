﻿using CommandSystem;
using HelpSense.API;
using HelpSense.API.Features.Pool;
using HelpSense.API.Serialization;
using HelpSense.ConfigSystem;
using PlayerRoles;
using PluginAPI.Core;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class InfoCommand : ICommand
    {
        public string Command => "Info";

        public string[] Aliases => ["info"];

        public string Description => "查询在本服务器游玩的时间和击杀人数";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            TranslateConfig TranslateConfig = Plugin.Instance.TranslateConfig;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = TranslateConfig.InfoCommandFailed;
                return false;
            }

            if (player.DoNotTrack || !Plugin.Instance.Config.SavePlayersInfo)
            {
                response = TranslateConfig.InfoCommandFailed;
                return false;
            }

            PlayerLog log = player.GetLog();
            int playedTimes = log.PlayedTimes;
            int hour = playedTimes / 3600;
            int day = hour / 24;
            int minutes = (playedTimes - hour * 3600) / 60;

            int kills = log.PlayerKills;
            int scpKills = log.PlayerSCPKills;
            float playerDamage = log.PlayerDamage;
            int rolePlayed = log.RolePlayed;
            int playerDeath = log.PlayerDeath;
            var shot = log.PlayerShot;

            var sb = StringBuilderPool.Pool.Get();

            sb.AppendLine(TranslateConfig.InfoCommandTitle);
            sb.AppendLine(TranslateConfig.InfoCommandPlayedTime.Replace("%day%" , day.ToString()).Replace("%hour%", hour.ToString()).Replace("%minutes%", minutes.ToString()));
            sb.AppendLine(TranslateConfig.InfoCommandRolePlayed.Replace("%rolePlayed%" , rolePlayed.ToString()));
            sb.AppendLine(TranslateConfig.InfoCommandKillsDamages.Replace("%kills%" , kills.ToString()).Replace("%scpKills%", scpKills.ToString()).Replace("%playerDamage%", playerDamage.ToString()).Replace("%playerDeath%", playerDeath.ToString()));
            sb.AppendLine(TranslateConfig.InfoCommandShot.Replace("%shot%" , shot.ToString()));

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);

            return true;
        }
    }
}
