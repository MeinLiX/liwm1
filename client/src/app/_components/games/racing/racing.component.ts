import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BsModalService } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { GameFinishedModalComponent } from 'src/app/_modals/game-finished-modal/game-finished-modal.component';
import { Car } from 'src/app/_models/car';
import { RacingTransmissionRange } from 'src/app/_models/racingTransmissionRange';
import { LobbyUser } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { RacingGameService } from 'src/app/_services/racing-game.service';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent implements OnInit, OnDestroy {

  private readonly carWidth = 40;
  private readonly carHeight = 65;
  private readonly transmissionGUIWidth = 30;
  private readonly transmissionGUIHeight = 200;
  private readonly transmissionBallRadius = 10;
  private readonly interval = 15;
  private readonly maxLap = 25;
  private readonly rareSpeedBost = 0.7;
  private readonly badSpeedBost = -0.9;
  private readonly mediumSpeedBost = 0.1;
  private readonly goodSpeedBost = 0.4;

  private canvas?: HTMLCanvasElement;
  private ctx?: CanvasRenderingContext2D | null;

  private isGamePlaying = false;
  private positionLineY = 0;
  private transmission = 0;
  private transmissionDelayTime = 1;

  private isPractise = true;

  constructor(private accountService: AccountService, private racingGameService: RacingGameService, private route: ActivatedRoute, private modalService: BsModalService) { }

  ngOnDestroy(): void {
    this.transmission = 0;
    this.isGamePlaying = false;
    this.positionLineY = 0;
    this.transmissionDelayTime = 1;
  }

  ngOnInit(): void {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');
    }

    this.route.queryParams.pipe(take(1)).subscribe({
      next: params => {
        if (this.ctx && this.canvas) {
          this.isPractise = params['isPractise'] === 'true';

          if (!this.isPractise) {
            this.accountService.currentUser$.pipe(take(1)).subscribe({
              next: user => {
                if (user) {
                  this.racingGameService.connectToGame(user);
                }
              }
            });
            this.racingGameService.cars$.subscribe({
              next: cars => {
                if (cars && cars.length > 0 && this.canvas) {
                  for (let i = 0; i < cars.length; i++) {
                    const car = cars[i];
                    const lastCar: Car | undefined = cars[i - 1];
                    const isLastIndexEven = (cars.length - 1) % 2 === 0;

                    if (lastCar) {
                      if (isLastIndexEven) {
                        car.x = lastCar.x + (this.carWidth * 2.5);
                      } else {
                        car.x = lastCar.x - (this.carWidth * 2.5);
                      }
                    } else {
                      car.x = this.canvas.width / 2 - this.carWidth * 0.5;
                    }

                    car.y = this.canvas.height - this.carHeight * 2;
                  }

                  this.drawCars();
                }
              }
            });

            this.racingGameService.onCarRecieved = this.addRecievedCar.bind(this);
            this.racingGameService.onRaceStarting = this.onStartGame.bind(this);
            this.racingGameService.onCarBoosted = this.onCarBoosted.bind(this);
            this.racingGameService.onCarReadyStateUpdated = this.onCarReadyStateUpdated.bind(this);
            this.racingGameService.onRaceFinished = this.onRaceFinished.bind(this);

            this.ctx.font = '72px serif';
            this.ctx.fillText('Tap to ready', this.canvas.width / 2 - 155, this.canvas.height / 2);
          } else {
            this.accountService.currentUser$.pipe(take(1)).subscribe({
              next: user => {
                if (user && this.canvas) {
                  const car = new Car(this.canvas.width / 2 - this.carWidth * 0.5, this.canvas.height - 100, 0, user.username);
                  this.racingGameService.addCarForSoloGame(car);
                  this.drawCar(car);
                  this.drawRoad(car);
                }
              }
            });

            this.ctx.font = '72px serif';
            this.ctx.fillText('Tap to start', this.canvas.width / 2 - 155, this.canvas.height / 2);
          }
        }
      }
    });
  }

  private addRecievedCar(car: Car) {
    this.racingGameService.cars$.pipe(take(1)).subscribe({
      next: cars => {
        if (cars && cars.length > 0 && this.canvas) {
          const lastCar: Car | undefined = cars[cars.length - 1];
          const isLastIndexEven = (cars.length - 1) % 2 === 0;

          if (lastCar) {
            if (isLastIndexEven) {
              car.x = lastCar.x + (this.carWidth * 2.5);
            } else {
              car.x = lastCar.x - (this.carWidth * 2.5);
            }
          } else {
            car.x = this.canvas.width / 2 - this.carWidth * 0.5;
          }

          car.y = this.canvas.height - this.carHeight * 2;

          this.drawCar(car);
          this.drawRoad(car);
        }
      }
    });
  }

  private onCarReadyStateUpdated(car: Car) {
    //TODO: Add view to showing which players are ready
  }

  private onCarBoosted(car: Car, cars: Car[]) {
    const foundCar = cars.find(c => c.id == car.id);
    if (foundCar) {
      foundCar.transmission++;
      foundCar.boostMode = car.boostMode;

      let addedSpeed = this.getCarCooridnatesSpeed(foundCar.boostMode);

      addedSpeed *= this.transmission / (foundCar.boostMode === RacingTransmissionRange.Bad ? 1 : 4);
      car.dy += addedSpeed;
      if (this.getCarSpeed(car.dy) < 0) {
        car.dy = 0.1;
      }
    }
  }

  private getCarCooridnatesSpeed(boostMode: RacingTransmissionRange) {
    let addedSpeed;

    switch (boostMode) {
      case RacingTransmissionRange.Bad:
        addedSpeed = this.badSpeedBost;
        break;
      case RacingTransmissionRange.Rare:
        addedSpeed = this.rareSpeedBost;
        break;
      case RacingTransmissionRange.Good:
        addedSpeed = this.goodSpeedBost;
        break;
      case RacingTransmissionRange.Medium:
        addedSpeed = this.mediumSpeedBost;
        break;
    }

    return addedSpeed;
  }

  async onClick() {
    if (!this.isGamePlaying) {
      if (this.isPractise) {
        this.isGamePlaying = true;
        await this.onStartGame();
      } else {
        this.racingGameService.playerCar$.pipe(take(1)).subscribe({
          next: car => {
            if (car) {
              car.isReady = !car.isReady;
              this.racingGameService.updateCarReadyState(car.isReady);
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

  private async onStartGame() {
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
        this.racingGameService.playerCar$.pipe(take(1)).subscribe({
          next: car => {
            if (car && car.lap >= this.maxLap) {
              this.resetPositionLineY();
              this.drawTransmissionPosition(this.positionLineY);
              this.transmission = 0;
              this.drawTransmissionNumber();
              return;
            }
          }
        });

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
    this.racingGameService.playerCar$.pipe(take(1)).subscribe({
      next: car => {
        if (car && this.canvas && this.ctx) {
          let addedSpeed = 0;

          let racingTransmissionRange;
          if (this.isYInRange(RacingTransmissionRange.Rare)) {
            addedSpeed = this.rareSpeedBost;
            racingTransmissionRange = RacingTransmissionRange.Rare;
          } else if (this.isYInRange(RacingTransmissionRange.Bad)) {
            addedSpeed = this.badSpeedBost;
            racingTransmissionRange = RacingTransmissionRange.Bad;
          } else if (this.isYInRange(RacingTransmissionRange.Medium)) {
            addedSpeed = this.mediumSpeedBost;
            racingTransmissionRange = RacingTransmissionRange.Medium;
          } else if (this.isYInRange(RacingTransmissionRange.Good)) {
            addedSpeed = this.goodSpeedBost;
            racingTransmissionRange = RacingTransmissionRange.Good;
          }

          if (!this.isPractise) {
            this.racingGameService.boostCar(racingTransmissionRange ?? 0);
          }

          addedSpeed *= this.transmission / (racingTransmissionRange === RacingTransmissionRange.Bad ? 1 : 4);
          car.dy += addedSpeed;
          if (this.getCarSpeed(car.dy) < 0) {
            car.dy = 0.1;
          }

          car.boostMode = racingTransmissionRange ?? 0;
        }
      }
    });
  }

  private incrementTransmission() {
    if (this.transmissionDelayTime < 2) {
      this.transmissionDelayTime += 0.25;
    }

    this.transmission++;
    if (this.transmission >= 10) {
      this.transmission--;
    }

    this.racingGameService.cars$.pipe(take(1)).subscribe({
      next: cars => {
        if (cars) {
          const car = cars[0];
          if (car) {
            car.transmission = this.transmission;
          }
        }
      }
    })
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
    this.racingGameService.playerCar$.pipe(take(1)).subscribe({
      next: car => {
        if (this.ctx && this.canvas && car) {
          this.ctx.beginPath();

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
    });
  }

  private getCarSpeed(dy: number): number {
    return Math.round(dy * 15);
  }

  private moveCars() {
    this.racingGameService.cars$.pipe(take(1)).subscribe({
      next: cars => {
        if (this.canvas && this.ctx) {
          this.ctx.clearRect(0, 0, this.canvas.width - this.transmissionGUIWidth * 1.5 - this.transmissionBallRadius * 3, this.canvas.height);

          if (cars) {
            for (let i = 0; i < cars.length; i++) {
              const car = cars[i];
              if (car.lap <= this.maxLap) {
                car.dy += this.getCarCooridnatesSpeed(car.boostMode === 0 ? 1 : car.boostMode) / 8 * car.transmission / this.interval;
                car.y -= car.dy;

                if (car.y < 0) {
                  car.y = this.canvas.height - 100;
                  car.lap++;

                  if (car.lap >= this.maxLap) {
                    car.y = -this.carHeight;
                    car.isFinished = true;

                    if (i == 0) {
                      this.racingGameService.finishRacing();
                    }
                  }
                }
              } else {
                car.dy = 0;
              }
            }

            this.drawLapNumber(cars[0].lap);
          }
        }
      }
    });
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
    this.racingGameService.cars$.pipe(take(1)).subscribe({
      next: cars => {
        if (cars) {
          for (let i = 0; i < cars.length; i++) {
            this.drawCar(cars[i]);
            this.drawRoad(cars[i]);
          }
        }
      }
    });
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

  private onRaceFinished(ratedPlayers: LobbyUser[]) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: account => {
        if (account && account.lobby) {
          const config = {
            class: 'modal-dialog-centered',
            initialState: {
              players: ratedPlayers,
              gameMode: account.lobby.gameMode
            }
          };
          this.modalService.show(GameFinishedModalComponent, config);
        }
      }
    });
  }
}