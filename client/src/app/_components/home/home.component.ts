import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { GameMode } from 'src/app/_models/gameMode';
import { GameModesService } from 'src/app/_services/game-modes.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  gameModes?: GameMode[];

  constructor(private gameModesService: GameModesService) { }

  ngOnInit(): void {
    this.gameModesService.getGameModes().pipe(take(1)).subscribe({
      next: games => {
        if (games) {
          this.gameModes = games;
        }
      }
    });
  }
}