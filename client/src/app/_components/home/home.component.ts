import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { GameMode } from 'src/app/_models/gameMode';
import { GamesService } from 'src/app/_services/games.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
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