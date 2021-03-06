﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Nexus.Client.Commands;
using Nexus.Client.Util;

namespace Nexus.Client.Games.FalloutNV
{
	/// <summary>
	/// Launches Fallout: New Vegas.
	/// </summary>
	public class FalloutNVLauncher : GameLauncherBase
	{
		#region Constructors

		/// <summary>
		/// A simple constructor that initializes the object with the given dependencies.
		/// </summary>
		/// <param name="p_gmdGameMode">>The game mode currently being managed.</param>
		/// <param name="p_eifEnvironmentInfo">The application's envrionment info.</param>
		public FalloutNVLauncher(IGameMode p_gmdGameMode, IEnvironmentInfo p_eifEnvironmentInfo)
			:base(p_gmdGameMode, p_eifEnvironmentInfo)
		{
		}

		#endregion

		/// <summary>
		/// Initializes the game launch commands.
		/// </summary>
		protected override void SetupCommands()
		{
			Trace.TraceInformation("Launch Commands:");
			Trace.Indent();

			ClearLaunchCommands();
			
			string strCommand = GetPlainLaunchCommand();
			Trace.TraceInformation("Plain Command: {0} (IsNull={1})", strCommand, (strCommand == null));
			Image imgIcon = File.Exists(strCommand) ? Icon.ExtractAssociatedIcon(strCommand).ToBitmap() : null;
			AddLaunchCommand(new Command("PlainLaunch", "Launch Fallout: New Vegas", "Launches plain Fallout: New Vegas.", imgIcon, LaunchFalloutNVPlain, true));

			strCommand = GetNvseLaunchCommand();
			Trace.TraceInformation("NVSE Command: {0} (IsNull={1})", strCommand, (strCommand == null));
			if (File.Exists(strCommand))
			{
				imgIcon = Icon.ExtractAssociatedIcon(strCommand).ToBitmap();
				AddLaunchCommand(new Command("NvseLaunch", "Launch NVSE", "Launches Fallout: New Vegas with NVSE.", imgIcon, LaunchFalloutNVNVSE, true));
			}

			strCommand = GetFNV4GbLaunchCommand();
			Trace.TraceInformation("FNV4Gb Command: {0} (IsNull={1})", strCommand, (strCommand == null));
			if (File.Exists(strCommand))
			{
				imgIcon = Icon.ExtractAssociatedIcon(strCommand).ToBitmap();
				AddLaunchCommand(new Command("FNV4GbLaunch", "Launch FNV4Gb", "Launches Fallout: New Vegas with FNV4Gb.", imgIcon, LaunchFalloutNVFNV4Gb, true));
			}

			strCommand = GetCustomLaunchCommand();
			Trace.TraceInformation("Custom Command: {0} (IsNull={1})", strCommand, (strCommand == null));
			imgIcon = File.Exists(strCommand) ? Icon.ExtractAssociatedIcon(strCommand).ToBitmap() : null;
			AddLaunchCommand(new Command("CustomLaunch", "Launch Custom Fallout: New Vegas", "Launches Fallout: New Vegas with custom command.", imgIcon, LaunchFalloutNVCustom, true));

			DefaultLaunchCommand = new Command("Launch Fallout: New Vegas", "Launches Fallout: New Vegas.", LaunchGame);

			Trace.Unindent();
		}

		#region Launch Commands

		#region Custom Command

		/// <summary>
		/// Launches the game with a custom command.
		/// </summary>
		private void LaunchFalloutNVCustom()
		{
			Trace.TraceInformation("Launching Fallout: New Vegas (Custom)...");
			Trace.Indent();

			string strCommand = GetCustomLaunchCommand();
			string strCommandArgs = EnvironmentInfo.Settings.CustomLaunchCommandArguments[GameMode.ModeId];
			if (String.IsNullOrEmpty(strCommand))
			{
				Trace.TraceError("No custom launch command has been set.");
				Trace.Unindent();
				OnGameLaunched(false, "No custom launch command has been set.");
				return;
			}
			Launch(strCommand, strCommandArgs);
		}

		/// <summary>
		/// Gets the custom launch command.
		/// </summary>
		/// <returns>The custom launch command.</returns>
		private string GetCustomLaunchCommand()
		{
			string strCommand = EnvironmentInfo.Settings.CustomLaunchCommands[GameMode.ModeId];
			if (!String.IsNullOrEmpty(strCommand))
			{
				strCommand = Environment.ExpandEnvironmentVariables(strCommand);
				strCommand = FileUtil.StripInvalidPathChars(strCommand);
				if (!Path.IsPathRooted(strCommand))
					strCommand = Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, strCommand);
			}
			return strCommand;
		}

		#endregion

		#region FOSE

		/// <summary>
		/// Launches the game, with NVSE.
		/// </summary>
		private void LaunchFalloutNVNVSE()
		{
			Trace.TraceInformation("Launching Fallout: New Vegas (NVSE)...");
			Trace.Indent();

			string strCommand = GetNvseLaunchCommand();
			Trace.TraceInformation("Command: " + strCommand);

			if (!File.Exists(strCommand))
			{
				Trace.TraceError("NVSE does not appear to be installed.");
				Trace.Unindent();
				OnGameLaunched(false, "NVSE does not appear to be installed.");
				return;
			}
			Launch(strCommand, null);
		}

		/// <summary>
		/// Gets the NVSE launch command.
		/// </summary>
		/// <returns>The NVSE launch command.</returns>
		private string GetNvseLaunchCommand()
		{
			return Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, "nvse_loader.exe");
		}

		#endregion

		#region FNV4Gb

		/// <summary>
		/// Launches the game, with FNV4Gb.
		/// </summary>
		private void LaunchFalloutNVFNV4Gb()
		{
			Trace.TraceInformation("Launching Fallout: New Vegas (FNV4Gb)...");
			Trace.Indent();

			string strCommand = GetFNV4GbLaunchCommand();
			Trace.TraceInformation("Command: " + strCommand);

			if (!File.Exists(strCommand))
			{
				Trace.TraceError("FNV4Gb does not appear to be installed.");
				Trace.Unindent();
				OnGameLaunched(false, "FNV4Gb does not appear to be installed.");
				return;
			}
			Launch(strCommand, null);
		}

		/// <summary>
		/// Gets the FNV4Gb launch command.
		/// </summary>
		/// <returns>The FNV4Gb launch command.</returns>
		private string GetFNV4GbLaunchCommand()
		{
			return Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, "fnv4gb.exe");
		}

		#endregion

		#region Vanilla Launch

		/// <summary>
		/// Launches the game, without FOSE.
		/// </summary>
		private void LaunchFalloutNVPlain()
		{
			Trace.TraceInformation("Launching Fallout: New Vegas (Plain)...");
			Trace.Indent();
			string strCommand = GetPlainLaunchCommand();
			Trace.TraceInformation("Command: " + strCommand);
			Launch(strCommand, null);
		}

		/// <summary>
		/// Gets the plain launch command.
		/// </summary>
		/// <returns>The plain launch command.</returns>
		private string GetPlainLaunchCommand()
		{
			string strCommand = Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, "falloutNV.exe");
			if (!File.Exists(strCommand))
				strCommand = Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, "falloutNVng.exe");
			return strCommand;
		}

		#endregion

		/// <summary>
		/// Launches the game, using NVSE if present.
		/// </summary>
		private void LaunchGame()
		{
			if (!String.IsNullOrEmpty(EnvironmentInfo.Settings.CustomLaunchCommands[GameMode.ModeId]))
				LaunchFalloutNVCustom();
			else if (File.Exists(Path.Combine(GameMode.GameModeEnvironmentInfo.InstallationPath, "nvse_loader.exe")))
				LaunchFalloutNVNVSE();
			else
				LaunchFalloutNVPlain();
		}

		#endregion
	}
}
