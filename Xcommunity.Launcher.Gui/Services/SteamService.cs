using System;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;
using Xcommunity.Launcher.Core;

namespace Xcommunity.Launcher.Gui.Services;

public class SteamService
{
    public Task<DetailedModData?> GetModDetails(ulong modId, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            SteamApiWrapper.Init();

            var queryHandle = SteamUGC.CreateQueryUGCDetailsRequest([new PublishedFileId_t(modId)], 1);
            try
            {
                var waitHandle = new ManualResetEventSlim();
                var onQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((_, _) => { waitHandle.Set(); });

                SteamUGC.SetReturnLongDescription(queryHandle, true);
                SteamUGC.SetReturnChildren(queryHandle, true);

                var apiCall = SteamUGC.SendQueryUGCRequest(queryHandle);
                onQueryCompleted.Set(apiCall);

                do
                {
                    SteamApiWrapper.RunCallbacks();
                } while (!waitHandle.Wait(100));

                if (!SteamUGC.GetQueryUGCResult(queryHandle, 0, out var result)) return null;

                return new DetailedModData
                {
                    Name = result.m_rgchTitle,
                    DateCreated = DateTimeOffset.FromUnixTimeSeconds(result.m_rtimeCreated),
                    DateUpdated = DateTimeOffset.FromUnixTimeSeconds(result.m_rtimeUpdated),
                    DateAdded = DateTimeOffset.FromUnixTimeSeconds(result.m_rtimeAddedToUserList),
                    Description = result.m_rgchDescription,
                    Author = GetAuthor(result.m_ulSteamIDOwner)
                };
            }
            finally
            {
                SteamUGC.ReleaseQueryUGCRequest(queryHandle);
            }
        }, cancellationToken);
    }

    private static string GetAuthor(ulong steamId)
    {
        var waitHandle = new ManualResetEvent(false);
        _ = Callback<PersonaStateChange_t>.Create(delegate { waitHandle.Set(); });
        var success = SteamFriends.RequestUserInformation(new CSteamID(steamId), true);

        if (success)
        {
            waitHandle.WaitOne(5000);
            waitHandle.Reset();
        }

        return SteamFriends.GetFriendPersonaName(new CSteamID(steamId));
    }
}

public record DetailedModData
{
    public required string Name { get; init; }
    public DateTimeOffset DateCreated { get; init; }
    public DateTimeOffset DateUpdated { get; init; }
    public DateTimeOffset DateAdded { get; init; }
    public string? Description { get; init; }
    public string? Author { get; init; }
}