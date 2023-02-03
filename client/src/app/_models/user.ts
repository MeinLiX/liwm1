export interface User {
    username: string;
    token: string;
    photoUrl: string;
    roles: string[];
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