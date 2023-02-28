import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-approve-lobby-join-request',
  templateUrl: './approve-lobby-join-request.component.html',
  styleUrls: ['./approve-lobby-join-request.component.css']
})
export class ApproveLobbyJoinRequestComponent implements OnInit {
  pendingConnections?: string[];

  constructor(public modalRef: BsModalRef, private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.lobbyService.lobby$.pipe(take(1)).subscribe({
      next: lobby => {
        this.pendingConnections = lobby?.pendingConnections;
      }
    });
  }

  approveUserJoin(approveUsername: string, isJoinApproved: boolean) {
    this.lobbyService.approveUserJoin(approveUsername, isJoinApproved);
    if (this.pendingConnections) {
      this.pendingConnections = this.pendingConnections?.filter(c => c != approveUsername);
      if (this.pendingConnections.length <= 0) {
        this.modalRef.hide();
      }
    }
  }
}
