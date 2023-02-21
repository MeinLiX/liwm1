import { LobbyUser } from "./user";

export interface Lobby {
    lobbyName: string;
    lobbyCreator: LobbyUser;
    users: LobbyUser[];
    pendingConnections: string[];
}