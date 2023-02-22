import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { LobbyConnectMode } from 'src/app/_models/lobbyConnectMode';

@Component({
  selector: 'app-lobby-connect',
  templateUrl: './lobby-connect.component.html',
  styleUrls: ['./lobby-connect.component.css']
})
export class LobbyConnectComponent {
  lobbyConnectMode?: LobbyConnectMode;

  constructor(public modalRef: BsModalRef) { }

  connectToLobby() {

  }
}
