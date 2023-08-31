import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameMode } from 'src/app/_models/gameMode';
import { Lobby } from 'src/app/_models/lobby';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-game-card',
  templateUrl: './game-card.component.html',
  styleUrls: ['./game-card.component.css']
})
export class GameCardComponent implements OnInit {
  @Input() gameMode?: GameMode;
  lobby: Lobby | null = null;
  user: User | null = null;

  constructor(public accountService: AccountService, private router: Router, private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.lobbyService.lobby$.subscribe({
      next: lobby => this.lobby = lobby
    });

    this.accountService.currentUser$.subscribe({
      next: user => this.user = user
    });
  }

  playPractiseGame() {
    if (this.gameMode) {
      this.router.navigate([this.gameMode.name.toLowerCase().replace(' ', '-')], {
        queryParams: {
          isPractise: true
        }
      });
    }
  }

  async playGame() {
    if (this.gameMode) {
      await this.router.navigate([this.gameMode.name.toLowerCase().replace(' ', '-')], {
        queryParams: {
          isPractise: false
        }
      });
      this.lobbyService.changeGameMode(this.gameMode.name);
    }
  }
}