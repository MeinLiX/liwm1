import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Lobby } from 'src/app/_models/lobby';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-lobby-settings',
  templateUrl: './lobby-settings.component.html',
  styleUrls: ['./lobby-settings.component.css']
})
export class LobbySettingsComponent {
  lobby?: Lobby | null;

  constructor(public modalRef: BsModalRef, private lobbyService: LobbyService) { }

  async cancelGame() {
    await this.lobbyService.cancelCurrentGame();
  }
}
