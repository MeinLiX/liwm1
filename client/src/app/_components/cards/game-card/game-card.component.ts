import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameMode } from 'src/app/_models/gameMode';
import { Lobby } from 'src/app/_models/lobby';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-game-card',
  templateUrl: './game-card.component.html',
  styleUrls: ['./game-card.component.css']
})
export class GameCardComponent implements OnInit {
  @Input() game?: GameMode;
  lobby: Lobby | null = null;

  constructor(public accountService: AccountService, private router: Router, private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.lobbyService.lobby$.subscribe({
      next: lobby => this.lobby = lobby
    });
  }

  playPractiseGame() {
    if (this.game) {
      this.router.navigate([this.game.name.toLowerCase().replace(' ', '-')], {
        queryParams: {
          isPractise: true
        }
      });
    }
  }

  playGame() {
    if (this.game) {
      this.lobbyService.changeGameMode(this.game.name);
    }
  }
}