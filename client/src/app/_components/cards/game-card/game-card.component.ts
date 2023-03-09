import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { GameMode } from 'src/app/_models/gameMode';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-game-card',
  templateUrl: './game-card.component.html',
  styleUrls: ['./game-card.component.css']
})
export class GameCardComponent {
  @Input() game?: GameMode;

  constructor(public accountService: AccountService, private router: Router) { }

  playSoloGame() {
    if (this.game) {
      this.router.navigateByUrl(this.game.name.toLowerCase());
    }
  }
}