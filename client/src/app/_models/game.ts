import { GameMode } from "./gameMode";
import { GameState } from "./gameState";
import { LobbyUser } from "./user";

export interface Game {
    mode: GameMode;
    players: LobbyUser[];
    ratingPlayers: LobbyUser[];
    createdAt: Date;
    state: GameState;
}