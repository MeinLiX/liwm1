import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { ApproveLobbyJoinRequestComponent } from 'src/app/_modals/approve-lobby-join-request/approve-lobby-join-request.component';
import { LobbyConnectComponent } from 'src/app/_modals/lobby-connect/lobby-connect.component';
import { LobbySettingsComponent } from 'src/app/_modals/lobby-settings/lobby-settings.component';
import { Lobby } from 'src/app/_models/lobby';
import { LobbyConnectMode } from 'src/app/_models/lobbyConnectMode';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnInit {
  lobby: Lobby | null = null;
  user: User | null = null;
  isDetailedVisible = false;
  lobbyConnectModalRef: BsModalRef<LobbyConnectComponent> = new BsModalRef<LobbyConnectComponent>();
  approveLobbyJoinRequestModalRef: BsModalRef<ApproveLobbyJoinRequestComponent> = new BsModalRef<ApproveLobbyJoinRequestComponent>();

  constructor(private lobbyService: LobbyService, public accountService: AccountService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.user = user;
          this.lobbyService.connectToLobby(user, '', LobbyConnectMode.None);
        }
      }
    });
    this.lobbyService.lobby$.subscribe({
      next: lobby => {
        this.lobby = lobby;
      }
    });
  }

  openLobbyModal() {
    const config = {};
    this.lobbyConnectModalRef = this.modalService.show(LobbyConnectComponent, config);
  }

  openRequestsModal() {
    const config = {};
    this.approveLobbyJoinRequestModalRef = this.modalService.show(ApproveLobbyJoinRequestComponent, config);
  }

  openLobbySettingsModal() {
    if (this.lobby) {
      const config = {
        class: 'modal-dialog-centered',
        initialState: {
          lobby: this.lobby
        }
      };
      this.modalService.show(LobbySettingsComponent, config);
    }
  }

  leaveLobby() {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.lobbyService.leaveLobby(user.username);
          this.isDetailedVisible = false;
        }
      }
    });
  }
}