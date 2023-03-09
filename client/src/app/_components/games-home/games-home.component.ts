import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { GameMode } from 'src/app/_models/gameMode';
import { GamesService } from 'src/app/_services/games.service';

@Component({
  selector: 'app-games-home',
  templateUrl: './games-home.component.html',
  styleUrls: ['./games-home.component.css']
})
export class GamesHomeComponent implements OnInit {
  games?: GameMode[];

  constructor(private gamesService: GamesService) { }

  ngOnInit(): void {
    this.gamesService.getGames().pipe(take(1)).subscribe({
      next: games => {
        if (games) {
          this.games = games;
        }
      }
    });
  }
}
