namespace Domain.Models.Constants;

public static class LobbyHubMethodNameConstants
{
    public const string NoLobbyWithSuchName = "NoLobbyWithSuchName";
    public const string LobbyAlreadyExists = "LobbyAlreadyExists";
    public const string LobbyCreate = "LobbyCreate";
    public const string NoLobbyWithSuchUser = "NoLobbyWithSuchUser";
    public const string NoUserWithSuchName = "NoUserWithSuchName";
    public const string JoinRequestSent = "JoinRequestSent";
    public const string JoinRequestReceived = "JoinRequestReceived";
    public const string SuccessfulyLeftLobby = "SuccessfulyLeftLobby";
    public const string UserLeftLobby = "UserLeftLobby";
    public const string NoUserWasProvided = "NoUserWasProvided";
    public const string NoPermisionToApproveJoin = "NoPermisionToApproveJoin";
    public const string NoPermisionOwnerLobby = "NoPermisionOwnerLobby";
    public const string NoSuchPendingJoinRequestWithProvidedName = "NoSuchPendingJoinRequestWithProvidedName";
    public const string UserJoined = "UserJoined";
    public const string LobbyWasDeleted = "LobbyWasDeleted";
    public const string UserAlreadyInLobby = "UserAlreadyInLobby";
    public const string LobbyNameMustBeProvidedInQuery = "LobbyNameMustBeProvidedInQuery";
    public const string LobbyConnectModeMustBeProvidedInQuery = "LobbyConnectModeMustBeProvidedInQuery";
    public const string HttpContextMustBeProvided = "HttpContextMustBeProvided";
    public const string LobbyForUserFound = "LobbyForUserFound";
    public const string LobbyForUserNotFound = "LobbyForUserNotFound";
    public const string PendingConnectionRemoved = "PendingConnectionRemoved";
    public const string UserJoinDenied = "UserJoinDenied";
    public const string UserKicked = "UserKicked";
    public const string UHaveBeenKicked = "UHaveBeenKicked";
}