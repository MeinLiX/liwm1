import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { Car } from 'src/app/_models/car';
import { RacingTransmissionRange } from 'src/app/_models/racingTransmissionRange';
import { AccountService } from 'src/app/_services/account.service';
import { RacingGameService } from 'src/app/_services/racing-game.service';

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
  readonly maxLap = 10;

  canvas?: HTMLCanvasElement;
  ctx?: CanvasRenderingContext2D | null;
  cars?: Car[];

  isGamePlaying = false;
  positionLineY = 0;
  transmission = 0;
  transmissionDelayTime = 1;

  isPractise = true;

  constructor(private accountService: AccountService, private racingGameService: RacingGameService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');

      if (this.ctx) {
        const practiseString = this.route.snapshot.paramMap.get('isPractise');
        if (practiseString) {
          this.isPractise = JSON.parse(practiseString);
        }

        if (this.isPractise) {
          this.racingGameService.cars$.subscribe({
            next: cars => {
              if (cars) {
                for (let i = 0; i < cars.length; i++) {
                  const isLastIndexEven = (cars.length - 1) % 2 === 0;
                  if (isLastIndexEven) {
                    const car = cars.length - 3 !== -1
                      ? cars[cars.length - 3]
                      : cars[0];
                    cars[i].x = car.x + (this.carWidth * 1.5);
                  } else {
                    const car = cars[cars.length - 3];
                    cars[i].x = car.x - (this.carWidth * 1.5);
                  }
                }

                this.cars = cars;
                this.drawCars();
              }
            }
          });

          this.ctx.font = '72px serif';
          this.ctx.fillText('Tap to ready', this.canvas.width / 2 - 155, this.canvas.height / 2);
        } else {

          this.accountService.currentUser$.pipe(take(1)).subscribe({
            next: user => {
              if (user && this.canvas) {
                this.cars = [new Car(this.canvas.width / 2 - this.carWidth * 0.5, this.canvas.height - 100, 0, user.username)];
                this.drawCar(this.cars[0]);
                this.drawRoad(this.cars[0]);
              }
            }
          });

          this.ctx.font = '72px serif';
          this.ctx.fillText('Tap to start', this.canvas.width / 2 - 155, this.canvas.height / 2);
        }
      }
    }

    this.racingGameService.onRaceStarting = this.startGame;
    this.racingGameService.onCarBoosted = this.carBoosted;
  }

  private carBoosted(car: Car) {
    if (this.cars) {
      const foundCar = this.cars.find(c => c.id == car.id);
      if (foundCar) {
        foundCar.transmission++;
        //TODO: Add speed acceleration
      }
    }
  }

  async onClick() {
    if (!this.isGamePlaying) {
      if (this.isPractise) {
        this.isGamePlaying = true;
        this.startGame();
      } else {
        this.racingGameService.playerCar$.pipe(take(1)).subscribe({
          next: car => {
            if (car) {
              this.racingGameService.updateCarReadyState(!car.isReady);
            }
          }
        });
      }
    } else {
      this.incrementTransmission();
      this.drawTransmissionNumber();
      this.turnTransmissionCar();
      this.resetPositionLineY();
    }
  }

  private async startGame() {
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
        if (this.cars) {
          const playerCar = this.cars[0];
          if (playerCar.lap >= this.maxLap) {
            this.resetPositionLineY();
            this.drawTransmissionPosition(this.positionLineY);
            this.transmission = 0;
            this.drawTransmissionNumber();
            return;
          }
        }

        this.drawSpeedText();
        this.drawTransmissionGUI();
        this.drawTransmissionPosition(this.positionLineY);
        this.positionLineY -= this.transmissionGUIHeight / 20 / (this.transmission + 2) / this.transmissionDelayTime;
        if (this.canvas && this.positionLineY < this.canvas.height / 2 - this.transmissionGUIHeight / 2.5) {
          this.resetPositionLineY();
          this.incrementTransmission();
          this.drawTransmissionNumber();
          this.turnTransmissionCar();
        }
      }, this.interval);

      setInterval(() => {
        this.moveCars();
        this.drawSpeedText();
        this.drawCars();
      }, this.interval);
    }
  }

  private resetPositionLineY() {
    if (this.canvas) {
      this.positionLineY = this.canvas.height / 2 + this.transmissionGUIHeight / 5 + this.transmissionGUIHeight / 2.5;
    }
  }

  private turnTransmissionCar() {
    if (this.cars && this.canvas && this.ctx) {
      let addedSpeed = 0;

      let racingTransmissionRange;
      if (this.isYInRange(RacingTransmissionRange.Rare)) {
        addedSpeed = 0.7;
        racingTransmissionRange = RacingTransmissionRange.Rare;
      } else if (this.isYInRange(RacingTransmissionRange.Bad)) {
        addedSpeed = -1;
        racingTransmissionRange = RacingTransmissionRange.Bad;
      } else if (this.isYInRange(RacingTransmissionRange.Medium)) {
        addedSpeed = 0.1;
        racingTransmissionRange = RacingTransmissionRange.Medium;
      } else if (this.isYInRange(RacingTransmissionRange.Good)) {
        addedSpeed = 0.4;
        racingTransmissionRange = RacingTransmissionRange.Good;
      }

      if (racingTransmissionRange) {
        this.racingGameService.boostCar(racingTransmissionRange);
      }

      const currentCar = this.cars[0];
      if (this.getCarSpeed(currentCar.dy + addedSpeed) > 0) {
        addedSpeed *= this.transmission;
        currentCar.dy += addedSpeed;
      } else {
        currentCar.dy = 1;
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
      this.ctx.fillText((this.getCarSpeed(car.dy)).toString(), x, y);

      this.ctx.font = '16px serif';
      this.ctx.fillText('km/h', x - 3, y + 15);

      this.ctx.closePath();
    }
  }

  private getCarSpeed(dy: number): number {
    return Math.round(dy * 15);
  }

  private moveCars() {
    if (this.canvas && this.ctx) {
      this.ctx.clearRect(0, 0, this.canvas.width - this.transmissionGUIWidth * 1.5 - this.transmissionBallRadius * 3, this.canvas.height);

      if (this.cars) {
        for (let i = 0; i < this.cars.length; i++) {
          const car = this.cars[i];
          if (car.lap <= this.maxLap) {
            car.dy += 0.075 * car.transmission / this.interval;
            car.y -= car.dy;

            if (car.y < 0) {
              car.y = this.canvas.height - 100;
              car.lap++;

              if (car.lap >= this.maxLap) {
                car.y = -this.carHeight;
              }
            }
          } else {
            car.dy = 0;
          }
        }

        this.drawLapNumber(this.cars[0].lap);
      }
    }
  }

  private drawLapNumber(lap: number) {
    if (this.canvas && this.ctx) {
      this.ctx.beginPath();

      const x = this.canvas.width - this.transmissionGUIWidth * 5;
      const y = 50;

      this.ctx.clearRect(x - 48, 0, 200, 60);

      if (lap < this.maxLap) {
        this.ctx.font = '48px serif';
        this.ctx.fillStyle = '#000';
        this.ctx.fillText('Lap: ' + lap, x, y);
      }

      this.ctx.closePath();
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
      console.log(car.y);
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

      if (this.isYInRange(RacingTransmissionRange.Rare)) {
        this.ctx.fillStyle = '#5834eb';
      } else if (this.isYInRange(RacingTransmissionRange.Bad)) {
        this.ctx.fillStyle = '#E74C3C';
      } else if (this.isYInRange(RacingTransmissionRange.Medium)) {
        this.ctx.fillStyle = '#FFA533';
      } else if (this.isYInRange(RacingTransmissionRange.Good)) {
        this.ctx.fillStyle = '#00BB08';
      }

      this.ctx.fill();
      this.ctx.closePath();
    }
  }

  private isYInRange(racingTransmissionRange: RacingTransmissionRange): boolean {
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