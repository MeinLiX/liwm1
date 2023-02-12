import { Lobby } from "./lobby";

export interface User {
    username: string;
    token: string;
    photoId: number;
    roles: string[];
    lobby: Lobby;
}

export interface UserLogin {
    username: string;
    password: string;
}

export interface UserRegister {
    username: string;
    password: string;
    photoId: number;
}

export interface AnonymousLogin {
    username: string;
    photoId: number;
}