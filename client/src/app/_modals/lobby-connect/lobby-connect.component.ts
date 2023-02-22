import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-lobby-connect',
  templateUrl: './lobby-connect.component.html',
  styleUrls: ['./lobby-connect.component.css']
})
export class LobbyConnectComponent {

  constructor(public modalRef: BsModalRef) { }

}
