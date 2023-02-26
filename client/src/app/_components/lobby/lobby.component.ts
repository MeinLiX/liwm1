import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { LobbyConnectComponent } from 'src/app/_modals/lobby-connect/lobby-connect.component';
import { Lobby } from 'src/app/_models/lobby';
import { LobbyConnectMode } from 'src/app/_models/lobbyConnectMode';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnInit {
  lobby?: Lobby;
  isDetailedVisible = false;
  lobbyConnectModalRef: BsModalRef<LobbyConnectComponent> = new BsModalRef<LobbyConnectComponent>();

  constructor(private lobbyService: LobbyService, private accountService: AccountService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.lobbyService.connectToLobby(user, '', LobbyConnectMode.None);
        }
      }
    });
    this.lobbyService.lobby$.subscribe({
      next: lobby => {
        if (lobby) this.lobby = lobby;
      }
    });
  }

  openLobbyModal() {
    const config = {};
    this.lobbyConnectModalRef = this.modalService.show(LobbyConnectComponent, config);
  }
}