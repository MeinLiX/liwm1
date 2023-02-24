import { Injectable } from '@angular/core';
import { environment } from 'src/enviroments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { LobbyConnectMode } from '../_models/lobbyConnectMode';
import { BehaviorSubject } from 'rxjs';
import { Lobby } from '../_models/lobby';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private lobbySource = new BehaviorSubject<Lobby | null>(null);
  lobby$ = this.lobbySource.asObservable();

  constructor(private toastr: ToastrService, private accountService: AccountService) { }

  connectToLobby(user: User, lobbyName: string, lobbyConnectMode: LobbyConnectMode) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'lobby', {
        accessTokenFactory: () => user.token
      })
      .build(); 

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserAlreadyInLobby', () => {
      this.toastr.error('User already in lobby');
      this.stopHubConnection();
    });

    this.hubConnection.on('JoinRequestSent', () => {
      this.toastr.success('Your join request was sent');
    });

    this.hubConnection.on('UserJoined', (lobby: Lobby) => {
      const lastLobby = this.lobbySource.value;
      if (lastLobby) {
        const newUser = lobby.users.find(u => !lastLobby.users.includes(u));
        if (newUser) {
          this.toastr.info('New user has joined: ' + newUser.username)
        }
      }

      this.setLobby(lobby);
    });

    this.hubConnection.on('LobbyCreate', (lobby: Lobby) => {
      this.setLobby(lobby);
      this.toastr.success('Lobby was created with name: ' + lobby.lobbyName);
    });
  }

  private setLobby(lobby: Lobby) {
    this.lobbySource.next(lobby);
    this.accountService.addLobbyToUser(lobby);
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  async approveUserJoin(lobbyName: string, approveUserName: string, isJoinApproved: boolean) {
    return this.hubConnection?.invoke('ApproveUserJoinAsync', { lobbyName, approveUserName, isJoinApproved })
      .catch(error => console.log(error));
  }
}