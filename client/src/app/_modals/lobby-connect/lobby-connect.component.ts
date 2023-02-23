import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { LobbyConnectMode } from 'src/app/_models/lobbyConnectMode';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-lobby-connect',
  templateUrl: './lobby-connect.component.html',
  styleUrls: ['./lobby-connect.component.css']
})
export class LobbyConnectComponent {
  lobbyConnectMode?: LobbyConnectMode;
  lobbyName?: string;

  constructor(public modalRef: BsModalRef, private lobbyService: LobbyService, private accountService: AccountService) { }

  connectToLobby() {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user && this.lobbyConnectMode && this.lobbyName) {
          this.lobbyService.connectToLobby(user, this.lobbyName, this.lobbyConnectMode);
        }
      }
    });
  }
}
