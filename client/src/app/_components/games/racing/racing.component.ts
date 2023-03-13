import { Component, OnInit, ViewChild } from '@angular/core';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent implements OnInit {
  canvas?: HTMLCanvasElement;
  ctx?: CanvasRenderingContext2D | null;

  constructor(private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');

      this.lobbyService.lobby$.subscribe({
        next: lobby => {
          const countOfCars = lobby ? lobby.users.length : 1;
          for (let i = 0; i < countOfCars; i++) {
            this.drawCar();
            this.drawRoad();
          }
        }
      });
    }
  }

  private drawRoad(): void {

  }

  private drawCar(): void {
    if (this.ctx) {
      this.ctx.rect(140, 100, 25, 35);
      this.ctx.stroke();
    }
  }
}