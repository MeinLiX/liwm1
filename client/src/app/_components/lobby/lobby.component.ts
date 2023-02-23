import { Component } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LobbyConnectComponent } from 'src/app/_modals/lobby-connect/lobby-connect.component';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent {
  isDetailedVisible = false;
  lobbyConnectModalRef: BsModalRef<LobbyConnectComponent> = new BsModalRef<LobbyConnectComponent>();

  constructor(public lobbyService: LobbyService, private modalService: BsModalService) { }

  openLobbyModal() {
    const config = {};
    this.lobbyConnectModalRef = this.modalService.show(LobbyConnectComponent, config);
  }
}