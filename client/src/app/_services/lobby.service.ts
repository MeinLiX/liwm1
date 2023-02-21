import { Injectable } from '@angular/core';
import { environment } from 'src/enviroments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  constructor() { }

  createHubConnection(user: User, lobbyName: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'lobby?=lobbyName=' + lobbyName, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
  }
}
