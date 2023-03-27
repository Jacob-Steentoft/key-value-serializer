

// ReSharper disable StringLiteralTypo

namespace KeyValueSerializer.Benchmarks.Models;

// https://community.bistudio.com/wiki/Arma_3:_Server_Config_File
public class ArmaServerOptions
{
	[KeyFileName("passwordAdmin")]
	public string? PasswordAdmin { get; set; }

	[KeyFileName("password")]
	public string? Password { get; set; }

	[KeyFileName("serverCommandPassword")]
	public string? ServerCommandPassword { get; set; }

	[KeyFileName("hostname")]
	public string? Hostname { get; set; }

	[KeyFileName("maxPlayers")]
	public uint? MaxPlayers { get; set; }

	[KeyFileName("motd[]")]
	public string[]? Motd { get; set; }
	
	[KeyFileName("motdInterval")]
	public uint? MotdInterval { get; set; }

	[KeyFileName("admins[]")]
	public string[]? Admins { get; set; }

	[KeyFileName("headlessClients[]")]
	public string[]? HeadlessClients { get; set; }

	[KeyFileName("localClient[]")]
	public string[]? LocalClient { get; set; }

	[KeyFileName("filePatchingExceptions")]
	public string[]? FilePatchingExceptions { get; set; }

	[KeyFileName("voteThreshold")]
	public double? VoteThreshold { get; set; }

	[KeyFileName("voteMissionPlayers")]
	public uint? VoteMissionPlayers { get; set; }

	[KeyFileName("kickDuplicate")]
	public uint? KickDuplicate { get; set; }

	[KeyFileName("loopback")]
	public bool? Loopback { get; set; }

	[KeyFileName("upnp")]
	public bool? Upnp { get; set; }

	[KeyFileName("allowedFilePatching")]
	public uint? AllowedFilePatching { get; set; }

	[KeyFileName("allowedLoadFileExtensions[]")]
	public string[]? AllowedLoadFileExtensions { get; set; }

	[KeyFileName("allowedPreprocessFileExtensions[]")]
	public string[]? AllowedPreprocessFileExtensions { get; set; }

	[KeyFileName("allowedHTMLLoadExtensions[]")]
	public string[]? AllowedHtmlLoadExtensions { get; set; }

	[KeyFileName("allowedHTMLLoadURIs[]")]
	public string[]? AllowedHtmlLoadUrIs { get; set; }
	
	[KeyFileName("onUserConnected")]
	public string? OnUserConnected { get; set; }
	
	[KeyFileName("onUserDisconnected")]
	public string? OnUserDisconnected { get; set; }
	
	[KeyFileName("doubleIdDetected")]
	public string? DoubleIdDetected { get; set; }

	[KeyFileName("onUnsignedData")]
	public string? OnUnsignedData { get; set; }
	
	[KeyFileName("onHackedData")]
	public string? OnHackedData { get; set; }
	
	[KeyFileName("onDifferentData")]
	public string? OnDifferentData { get; set; }
	
	[KeyFileName("disconnectTimeout")]
	public uint? DisconnectTimeout { get; set; }

	[KeyFileName("maxdesync")]
	public uint? MaxDesync { get; set; }

	[KeyFileName("maxping")]
	public uint? MaxPing { get; set; }

	[KeyFileName("maxpacketloss")]
	public uint? MaxPacketLoss { get; set; }

	[KeyFileName("kickClientsOnSlowNetwork[]")]
	public uint[]? KickClientsOnSlowNetwork { get; set; }

	[KeyFileName("enablePlayerDiag")]
	public uint? EnablePlayerDiag { get; set; }

	[KeyFileName("callExtReportLimit")]
	public uint? CallExtReportLimit { get; set; }

	[KeyFileName("votingTimeOut")]
	public uint? VotingTimeOut { get; set; }

	[KeyFileName("roleTimeOut")]
	public uint? RoleTimeOut { get; set; }

	[KeyFileName("briefingTimeOut")]
	public uint? BriefingTimeOut { get; set; }

	[KeyFileName("debriefingTimeOut")]
	public uint? DebriefingTimeOut { get; set; }

	[KeyFileName("lobbyIdleTimeout")]
	public uint? LobbyIdleTimeout { get; set; }

	[KeyFileName("missionsToServerRestart")]
	public uint? MissionsToServerRestart { get; set; }

	[KeyFileName("missionsToShutdown")]
	public uint? MissionsToShutdown { get; set; }

	[KeyFileName("autoSelectMission")]
	public bool? AutoSelectMission { get; set; }

	[KeyFileName("randomMissionOrder")]
	public bool? RandomMissionOrder { get; set; }

	[KeyFileName("verifySignatures")]
	public uint? VerifySignatures { get; set; }
	
	[KeyFileName("requiredSecureId")]
	public uint? RequiredSecureId { get; set; }

	[KeyFileName("drawingInMap")]
	public bool? DrawingInMap { get; set; }

	[KeyFileName("disableVoN")]
	public uint? DisableVoN { get; set; }

	[KeyFileName("vonCodecQuality")]
	public uint? VonCodecQuality { get; set; }

	[KeyFileName("vonCodec")]
	public uint? VonCodec { get; set; }

	[KeyFileName("skipLobby")]
	public bool? SkipLobby { get; set; }

	[KeyFileName("allowProfileGlasses")]
	public bool? AllowProfileGlasses { get; set; }

	[KeyFileName("zeusCompositionScriptLevel")]
	public uint? ZeusCompositionScriptLevel { get; set; }

	[KeyFileName("logFile")]
	public string? LogFile { get; set; }

	[KeyFileName("BattlEye")]
	public uint? BattlEye { get; set; }
	
	[KeyFileName("battleyeLicense")]
	public uint? BattlEyeLicense { get; set; }

	[KeyFileName("timeStampFormat")]
	public string? TimeStampFormat { get; set; }

	[KeyFileName("forceRotorLibSimulation")]
	public uint? ForceRotorLibSimulation { get; set; }

	[KeyFileName("persistent")]
	public uint? Persistent { get; set; }

	[KeyFileName("requiredBuild")]
	public uint? RequiredBuild { get; set; }

	[KeyFileName("statisticsEnabled")]
	public uint? StatisticsEnabled { get; set; }

	[KeyFileName("forcedDifficulty")]
	public string? ForcedDifficulty { get; set; }

	[KeyFileName("missionWhitelist[]")]
	public string[]? MissionWhitelist { get; set; }

	[KeyFileName("steamProtocolMaxDataSize")]
	public uint? SteamProtocolMaxDataSize { get; set; }

	[KeyFileName("armaUnitsTimeout")]
	public uint? ArmaUnitsTimeout { get; set; }
}
