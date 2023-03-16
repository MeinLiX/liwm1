import { Component, OnInit, ViewChild } from '@angular/core';
import { delay } from 'rxjs';
import { Car } from 'src/app/_models/car';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent implements OnInit {

  readonly carWidth = 40;
  readonly carHeight = 65;

  canvas?: HTMLCanvasElement;
  ctx?: CanvasRenderingContext2D | null;
  cars?: Car[];

  isGamePlaying = false;

  constructor(private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');

      if (this.ctx) {
        this.lobbyService.lobby$.subscribe({
          next: lobby => {
            const countOfCars = lobby ? lobby.users.length : 1;
            for (let i = 0; i < countOfCars; i++) {
              this.createCar();
              if (this.cars && this.cars.length > 0) {
                this.drawCar(this.cars[i]);
                this.drawRoad(this.cars[i]);
              }
            }
          }
        });

        this.ctx.font = '72px serif';
        this.ctx.fillText('Tap to start', 355, 350);
      }
    }
  }

  async onClick() {
    if (!this.isGamePlaying) {
      this.isGamePlaying = true;

      const sleep = (ms: number) => new Promise(r => setTimeout(r, ms));

      if (this.ctx && this.canvas) {
        for (let counter = 3; counter > 0; counter--) {
          this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

          this.ctx.font = '72px serif';
          this.ctx.fillText(counter.toString(), 485, 350);

          await sleep(1000);
        }

        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.drawCars();
      }
    } else {

    }
  }

  private drawCars() {
    if (this.cars) {
      for (let i = 0; i < this.cars.length; i++) {
        this.drawCar(this.cars[i]);
        this.drawRoad(this.cars[i]);
      }
    }
  }

  private createCar() {
    if (this.cars && this.cars.length > 1) {
      const isLastIndexEven = (this.cars.length - 1) % 2 === 0;
      if (isLastIndexEven) {
        const car = this.cars.length - 3 !== -1
          ? this.cars[this.cars.length - 3]
          : this.cars[0];
        this.cars.push(new Car(car.x + (this.carWidth * 1.5), car.y));
      } else {
        const car = this.cars[this.cars.length - 3];
        this.cars.push(new Car(car.x - (this.carWidth * 1.5), car.y));
      }
    } else {
      const cars = [new Car(480, 600)];
      this.cars = cars;
    }
  }

  private drawRoad(car: Car): void {
    if (this.ctx) {
      this.ctx.beginPath();

      this.ctx.moveTo(car.x - this.carWidth * 0.5, 0);
      this.ctx.lineTo(car.x - this.carWidth * 0.5, 700);
      this.ctx.stroke();

      this.ctx.moveTo(car.x + this.carWidth * 1.5, 0);
      this.ctx.lineTo(car.x + this.carWidth * 1.5, 700);
      this.ctx.stroke();

      for (let i = 0; i <= 700; i += 40) {
        this.ctx.moveTo(car.x + this.carWidth * 0.5, i + this.carWidth * 0.5);
        this.ctx.lineTo(car.x + this.carWidth * 0.5, i + this.carWidth);
        this.ctx.stroke();
      }

      this.ctx.closePath();
    }
  }

  private drawCar(car: Car): void {
    if (this.ctx) {
      this.ctx.beginPath();
      this.ctx.fillRect(car.x, car.y, this.carWidth, this.carHeight);
      this.ctx.stroke();
      this.ctx.closePath();
    }
  }
}