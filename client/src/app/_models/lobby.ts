import { Game } from "./game";
import { GameMode } from "./gameMode";
import { LobbyUser } from "./user";

export interface Lobby {
    lobbyName: string;
    lobbyCreator: LobbyUser;
    users: LobbyUser[];
    pendingConnections: string[];
    gameMode: GameMode;
    currentGame?: Game;
    previousGames?: Game[];
}