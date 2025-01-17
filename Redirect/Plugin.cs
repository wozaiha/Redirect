﻿using Dalamud.Data;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;

namespace Redirect {
    public class Plugin : IDalamudPlugin, IDisposable {
        
        public string Name => "Redirect";
        private const string CommandName = "/redirect";
        private Configuration Configuration { get; set; }
        private PluginUI PluginUi { get; } = null!;
        private Actions Actions { get; } = null!;
        private GameHooks Hooks { get; } = null!;
        public DalamudPluginInterface Interface => Services.Interface;
        public DataManager DataManager => Services.DataManager;
        public CommandManager CommandManager => Services.CommandManager;


        public Plugin([RequiredVersion("1.0")] DalamudPluginInterface i) {
            Services.Initialize(i);
            
            try {
                Configuration = Interface.GetPluginConfig() as Configuration ?? new Configuration();
            } 
            catch(Exception) {
                PluginLog.Error("Failed to load plugin configuration.");
                Configuration = new Configuration();
            }

            Actions = new();
            Hooks = new(Configuration, Actions);
            PluginUi = new(this, Configuration, Hooks, Actions);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
                HelpMessage = "Adjust targeting priority for any action"
            });
        }

        public void Dispose() {
            Hooks.Dispose();
            PluginUi.Dispose();
            Configuration.Save();
            CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args) {
            PluginUi.MainWindowVisible = true;
        }

    }
}
