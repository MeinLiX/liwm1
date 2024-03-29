import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Lobby } from '../_models/lobby';
import { DataRestResponseResult, RestResponseResult } from '../_models/restResponseResult';
import { User, UserLogin, UserLogout, UserRegister } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: UserLogin) {
    return this.http.post<DataRestResponseResult<User>>(this.baseUrl + 'account/login', model).pipe(
      map(response => this.getAnSetUserFromResponse(response))
    );
  }

  logout() {
    if (this.currentUserSource.value) {
      const modelForLogout: UserLogout = {
        username: this.currentUserSource.value.username
      };
      this.apiLogout(modelForLogout).pipe(take(1)).subscribe();
    }
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  private apiLogout(model: UserLogout) {
    return this.http.post<RestResponseResult>(this.baseUrl + 'account/logout', model);
  }

  register(model: UserRegister) {
    return this.http.post<DataRestResponseResult<User>>(this.baseUrl + 'account/register', model).pipe(
      map(response => this.getAnSetUserFromResponse(response))
    );
  }

  private getAnSetUserFromResponse(response: DataRestResponseResult<User>) {
    const user = response.data;
    if (user) {
      this.setCurrentUser(user);
    }
    return user;
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  private getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }

  addLobbyToUser(lobby: Lobby) {
    if (this.currentUserSource.value) {
      this.currentUserSource.value.lobby = lobby;
    }
  }
}