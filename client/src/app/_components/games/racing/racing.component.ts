import { Component, OnInit, ViewChild } from '@angular/core';
import { Car } from 'src/app/_models/car';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent implements OnInit {
  canvas?: HTMLCanvasElement;
  ctx?: CanvasRenderingContext2D | null;
  cars?: Car[];

  constructor(private lobbyService: LobbyService) { }

  ngOnInit(): void {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');

      this.lobbyService.lobby$.subscribe({
        next: lobby => {
          const countOfCars = lobby ? lobby.users.length : 1;
          for (let i = 0; i < countOfCars; i++) {
            this.createCar();
            if (this.cars && this.cars.length > 0) {
              this.drawCar(this.cars[this.cars.length - 1]);
            }
            this.drawRoad();
          }
        }
      });
    }
  }

  private createCar() {
    if (this.cars && this.cars.length > 1) {
      let isLastIndexEven = (this.cars.length - 1) % 2 === 0;
      if (isLastIndexEven) {
        let car = this.cars.length - 3 !== -1
          ? this.cars[this.cars.length - 3]
          : this.cars[0];
        this.cars.push(new Car(car.x + (40 * 1.5), car.y));
      } else {
        let car = this.cars[this.cars.length - 3];
        this.cars.push(new Car(car.x - (40 * 1.5), car.y));
      }
    } else {
      let cars = [new Car(480, 600)];
      this.cars = cars;
    }
  }

  private drawRoad(): void {

  }

  private drawCar(car: Car): void {
    if (this.ctx) {
      this.ctx.rect(car.x, car.y, 40, 65);
      this.ctx.stroke();
    }
  }
}