﻿using Helion.Util.Consoles.Commands;
using Helion.Util.Consoles;
using Helion.Window.Input;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System;
using Helion.Util.Extensions;
using Helion.Util;

namespace Helion.Client;

public partial class Client
{
    [ConsoleCommand("bind", "Binds a key to a command. Replaces any existing binding to the specified key")]
    private void BindCommand(ConsoleCommandEventArgs args)
    {
        ExecuteBind(args, true);
    }

    [ConsoleCommand("bindadd", "Binds a command to a key")]
    private void BindAddCommand(ConsoleCommandEventArgs args)
    {
        ExecuteBind(args, false);
    }

    private void ExecuteBind(ConsoleCommandEventArgs args, bool removeExisting)
    {
        if (args.Args.Count == 0)
        {
            LogKeyBindings();
            return;
        }

        if (args.Args.Count < 2)
        {
            Log.Error("Bind requries two arguments");
            return;
        }

        string key = args.Args[0];
        string command = args.Args[1];

        if (!GetInputKey(key, out var inputKey))
            return;

        var inputCommands = GetAvailableInputCommands();
        if (!CheckAvailableInputCommands(inputCommands, command))
            return;

        m_config.Keys.Remove(inputKey.Value);
        m_config.Keys.Add(inputKey.Value, command);
    }

    [ConsoleCommand("unbind", "Unbinds a key with an optional specific command")]
    private void UnbindCommand(ConsoleCommandEventArgs args)
    {
        if (args.Args.Count == 0)
        {
            LogKeyBindings();
            return;
        }

        if (args.Args.Count < 1)
        {
            Log.Error("Bind requries one argument");
            return;
        }

        if (!GetInputKey(args.Args[0], out var inputKey))
            return;

        if (args.Args.Count == 1)
        {
            if (!m_config.Keys.Remove(inputKey.Value))
                Log.Error($"{inputKey} has no commands");
            return;
        }

        string command = args.Args[1];

        var inputCommands = GetAvailableInputCommands();
        if (!CheckAvailableInputCommands(inputCommands, command))
            return;

        if (!m_config.Keys.Remove(inputKey.Value, command))
            Log.Error($"{inputKey} does not have ${command}");
    }

    [ConsoleCommand("inputkeys", "List all input keys")]
    private void InputKeys(ConsoleCommandEventArgs args)
    {
        var values = Enum.GetValues(typeof(Key));
        foreach (var value in values)
            Log.Info(value);
    }


    [ConsoleCommand("inputcommands", "List all input commands")]
    private void InputCommands(ConsoleCommandEventArgs args)
    {
        var inputCommands = GetAvailableInputCommands();
        foreach (var inputCommand in inputCommands)
            Log.Info(inputCommand);
    }

    private static bool GetInputKey(string key, [NotNullWhen(true)] out Key? inputKey)
    {
        if (!Enum.TryParse(typeof(Key), key, true, out var parsedKey) || parsedKey == null)
        {
            inputKey = null;
            Log.Error($"Invalid key: {key}");
            Log.Info("Use inputkeys to view all available keys");
            return false;
        }

        inputKey = (Key)parsedKey;
        return true;
    }

    private static IList<string> GetAvailableInputCommands()
    {
        var properties = typeof(Constants.Input).GetFields();
        return properties.Select(x => x.Name).OrderBy(x => x).ToArray();
    }

    private static bool CheckAvailableInputCommands(IList<string> inputCommands, string command)
    {
        if (!inputCommands.Any(x => x.EqualsIgnoreCase(command)))
        {
            Log.Error($"Invalid command: {command}");
            Log.Info("Use inputcommands to view all available commands");
            return false;
        }

        return true;
    }

    private void LogKeyBindings()
    {
        Log.Info("Key bindings");
        foreach (var item in m_config.Keys.GetKeyMapping())
            foreach (var value in item.Value)
                Log.Info($"{item.Key}: {value}");
    }
}
