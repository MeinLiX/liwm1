import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/enviroments/environment';
import { DataRestResponseResult } from '../_models/restResponseResult';
import { AnonymousLogin, User, UserLogin, UserRegister } from '../_models/user';

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

  anonymousLogin(model: AnonymousLogin) {
    return this.http.post<DataRestResponseResult<User>>(this.baseUrl + 'account/anonymous', model).pipe(
      map(response => this.getAnSetUserFromResponse(response))
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
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
}
