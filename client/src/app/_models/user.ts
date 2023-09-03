import { GameStats } from "./gameStats";
import { Lobby } from "./lobby";

export interface User {
    username: string;
    token: string;
    photoId: number;
    roles: string[];
    lobby?: Lobby;
    isAnonymous: boolean;
}

export interface UserLogin {
    username: string;
    password?: string;
    photoId: number;
}

export interface UserRegister {
    username: string;
    password: string;
    photoId: number;
}

export interface UserLogout {
    username: string;
}

export interface LobbyUser {
    photoId: number;
    photoUrl?: string;
    username: string;
    isAnonymous: boolean;
    stats?: GameStats;
}