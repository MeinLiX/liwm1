import { Component, OnInit } from '@angular/core';
import { timeout } from 'rxjs';
import { Car } from 'src/app/_models/car';
import { RacingTransmissionRange } from 'src/app/_models/racingTransmissionRange';
import { LobbyService } from 'src/app/_services/lobby.service';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent implements OnInit {

  readonly carWidth = 40;
  readonly carHeight = 65;
  readonly transmissionGUIWidth = 30;
  readonly transmissionGUIHeight = 200;
  readonly transmissionBallRadius = 10;
  readonly interval = 15;

  canvas?: HTMLCanvasElement;
  ctx?: CanvasRenderingContext2D | null;
  cars?: Car[];

  isGamePlaying = false;
  positionLineY = 0;
  transmission = 0;
  transmissionDelayTime = 1;

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
        this.ctx.fillText('Tap to start', this.canvas.width / 2 - 155, this.canvas.height / 2);
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
          this.ctx.fillText(counter.toString(), this.canvas.width / 2 - 25, this.canvas.height / 2);

          await sleep(1000);
        }

        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.drawCars();

        this.resetPositionLineY();
        this.drawTransmissionNumber();

        setInterval(() => {
          this.drawSpeedText();
          this.drawTransmissionGUI();
          this.drawTransmissionPosition(this.positionLineY);
          this.positionLineY -= this.transmissionGUIHeight / 100 / this.transmissionDelayTime;
          if (this.canvas && this.positionLineY < this.canvas.height / 2 - this.transmissionGUIHeight / 2.5) {
            this.resetPositionLineY();
            this.incrementTransmission();
            this.drawTransmissionNumber();
            this.accelerateCar();
          }
        }, this.interval);

        setInterval(() => {
          this.moveCars();
          this.drawCars();
        }, this.interval);
      }
    } else {
      this.incrementTransmission();
      this.drawTransmissionNumber();
      this.accelerateCar();
      this.resetPositionLineY();
    }
  }

  private resetPositionLineY() {
    if (this.canvas) {
      this.positionLineY = this.canvas.height / 2 + this.transmissionGUIHeight / 5 + this.transmissionGUIHeight / 2.5;
    }
  }

  private accelerateCar() {
    if (this.cars && this.canvas && this.ctx) {
      let addedSpeed = 0;

      if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Rare)) {
        addedSpeed = 1.2;
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Bad)) {
        addedSpeed = -1;
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Medium)) {
        addedSpeed = 0.2;
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Good)) {
        addedSpeed = 1;
      }

      const currentCar = this.cars[0];
      if (currentCar.dy + addedSpeed > 0) {
        currentCar.dy += addedSpeed * this.transmission;
      } else {
        currentCar.dy = 1 * this.transmission;
      }
    }
  }

  private incrementTransmission() {
    if (this.transmissionDelayTime < 2) {
      this.transmissionDelayTime += 0.25;
    }

    this.transmission++;
    if (this.transmission >= 10) {
      this.transmission--;
    }
  }

  private drawTransmissionNumber() {
    if (this.ctx && this.canvas) {
      this.ctx.beginPath();

      const x = this.canvas.width - this.transmissionGUIWidth * 1.5 + 48 / 10;
      const y = this.canvas.height / 2 - this.transmissionGUIHeight / 2.5 - 15;

      this.ctx.clearRect(x - 17, y - 40, 48, 48);

      this.ctx.font = '48px serif';
      this.ctx.fillStyle = '#000';
      this.ctx.fillText(this.transmission.toString(), x, y);

      this.ctx.closePath();
    }
  }

  private drawSpeedText() {
    if (this.ctx && this.canvas && this.cars) {
      this.ctx.beginPath();

      const car = this.cars[0];

      const x = this.canvas.width - this.transmissionGUIWidth * 1.5 + 3;
      const y = this.canvas.height / 2 + this.transmissionGUIHeight / 1.5 + 15;

      this.ctx.clearRect(x - 12, y - 20, 100, 58);

      this.ctx.fillStyle = '#000';

      this.ctx.font = '24px serif';
      this.ctx.fillText((Math.round(car.dy * 10)).toString(), x, y);

      this.ctx.font = '16px serif';
      this.ctx.fillText('km/h', x - 3, y + 15);

      this.ctx.closePath();
    }
  }

  private moveCars() {
    if (this.canvas && this.ctx) {
      this.ctx.clearRect(0, 0, this.canvas.width - this.transmissionGUIWidth * 1.5 - this.transmissionBallRadius * 3, this.canvas.height);

      if (this.cars) {
        for (let i = 0; i < this.cars.length; i++) {
          const car = this.cars[i];
          car.dy += 0.5 * this.transmission / 100 / this.interval;
          car.y -= car.dy;

          if (car.y < 0) {
            car.y = this.canvas.height - 100;
          }
        }
      }
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
    if (this.canvas) {
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
        const cars = [new Car(this.canvas.width / 2 - this.carWidth * 0.5, this.canvas.height - 100)];
        this.cars = cars;
      }
    }
  }

  private drawRoad(car: Car): void {
    if (this.ctx && this.canvas) {
      this.ctx.beginPath();

      this.ctx.moveTo(car.x - this.carWidth * 0.5, 0);
      this.ctx.lineTo(car.x - this.carWidth * 0.5, 700);
      this.ctx.stroke();

      this.ctx.moveTo(car.x + this.carWidth * 1.5, 0);
      this.ctx.lineTo(car.x + this.carWidth * 1.5, 700);
      this.ctx.stroke();

      for (let i = 0; i <= this.canvas.height; i += 40) {
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
      this.ctx.fillStyle = '#000';
      this.ctx.fillRect(car.x, car.y, this.carWidth, this.carHeight);
      this.ctx.closePath();
    }
  }

  private drawTransmissionGUI(): void {
    if (this.ctx && this.canvas) {
      const x = this.canvas.width - this.transmissionGUIWidth * 1.5;
      //red part
      this.ctx.fillStyle = '#E74C3C';
      this.ctx.fillRect(x, this.canvas.height / 2 + this.transmissionGUIHeight / 5, this.transmissionGUIWidth, this.transmissionGUIHeight / 2.5);
      this.ctx.fillRect(x, this.canvas.height / 2 - this.transmissionGUIHeight / 2.5, this.transmissionGUIWidth, this.transmissionGUIHeight / 5);
      //yellow part
      this.ctx.fillStyle = '#FFA533';
      this.ctx.fillRect(x, this.canvas.height / 2 - this.transmissionGUIHeight / 10, this.transmissionGUIWidth, this.transmissionGUIHeight / 2.5);
      //green part
      this.ctx.fillStyle = '#00BB08';
      this.ctx.fillRect(x, this.canvas.height / 2 - this.transmissionGUIHeight / 5, this.transmissionGUIWidth, this.transmissionGUIHeight / 10);
      //blue part (rare)
      this.ctx.fillStyle = '#5834eb';
      this.ctx.fillRect(x, this.canvas.height / 2 - this.transmissionGUIHeight / 6, this.transmissionGUIWidth, this.transmissionGUIHeight / 50);
    }
  }

  private drawTransmissionPosition(y: number): void {
    if (this.ctx && this.canvas) {
      const x = this.canvas.width - this.transmissionGUIWidth * 1.5 - this.transmissionBallRadius * 1.8;
      this.ctx.clearRect(x - this.transmissionBallRadius, 0, 2.5 * this.transmissionBallRadius, this.canvas.height);

      this.ctx.beginPath();
      this.ctx.moveTo(x, y);
      this.ctx.lineTo(x + this.transmissionBallRadius * 1.5, y);
      this.ctx.stroke();
      this.ctx.closePath();

      this.ctx.beginPath();
      this.ctx.arc(x, y, this.transmissionBallRadius, 0, Math.PI * 2);

      if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Rare)) {
        this.ctx.fillStyle = '#5834eb';
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Bad)) {
        this.ctx.fillStyle = '#E74C3C';
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Medium)) {
        this.ctx.fillStyle = '#FFA533';
      } else if (this.isYInRange(this.positionLineY, RacingTransmissionRange.Good)) {
        this.ctx.fillStyle = '#00BB08';
      }

      this.ctx.fill();
      this.ctx.closePath();
    }
  }

  private isYInRange(y: number, racingTransmissionRange: RacingTransmissionRange): boolean {
    let result = false;

    if (this.canvas) {
      switch (racingTransmissionRange) {
        case RacingTransmissionRange.Bad:
          result = this.positionLineY > this.canvas.height / 2 + this.transmissionGUIHeight / 5 || (this.positionLineY > this.canvas.height / 2 - this.transmissionGUIHeight / 2.5 && this.positionLineY < this.canvas.height / 2 - this.transmissionGUIHeight / 5);
          break;
        case RacingTransmissionRange.Medium:
          result = this.positionLineY > this.canvas.height / 2 - this.transmissionGUIHeight / 10;
          break;
        case RacingTransmissionRange.Good:
          result = this.positionLineY > this.canvas.height / 2 - this.transmissionGUIHeight / 5;
          break;
        case RacingTransmissionRange.Rare:
          result = this.positionLineY > this.canvas.height / 2 - this.transmissionGUIHeight / 6 && this.positionLineY < this.canvas.height / 2 - this.transmissionGUIHeight / 6 + this.transmissionGUIHeight / 50;
          break;
      }
    }

    return result;
  }
}