import { Injectable } from '@angular/core';
import { environment } from 'src/enviroments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { LobbyConnectMode } from '../_models/lobbyConnectMode';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  constructor(private toastr: ToastrService) { }

  createHubConnection(user: User, lobbyName: string, lobbyConnectMode: LobbyConnectMode) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'lobby?lobbyName=' + lobbyName + '&lobbyConnectMode=' + lobbyConnectMode, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserAlreadyInLobby', () => {
      this.toastr.error('User already in lobby');
      this.stopHubConnection();
    });
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}