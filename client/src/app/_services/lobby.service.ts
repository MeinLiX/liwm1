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
      .withUrl(this.hubUrl + 'lobby?lobbyName=' + lobbyName + '&lobbyConnectMode=' + lobbyConnectMode, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('LobbyAlreadyExists', () => {
      this.toastr.error("Lobby with same name already exists");
      this.stopHubConnection();
    })

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

    this.hubConnection.on('LobbyForUserFound', (lobby: Lobby) => {
      this.setLobby(lobby);
    });

    this.hubConnection.on('LobbyForUserNotFound', () => {
      this.stopHubConnection();
    });

    this.hubConnection.on('JoinRequestReceived', (lobby: Lobby) => {
      this.setLobby(lobby);
      this.toastr.info('New join request recieved');
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

  async approveUserJoin(approveUsername: string, isJoinApproved: boolean) {
    const lobbyName = this.lobbySource.value?.lobbyName;
    if (lobbyName) {
      console.log(lobbyName);
      const newLobby = await this.hubConnection?.invoke<Lobby>('ApproveUserJoinAsync', lobbyName, approveUsername, isJoinApproved)
        .catch(error => console.log(error));
      console.log(newLobby);
      if (newLobby) {
        this.setLobby(newLobby);
      }
    }
  }
}