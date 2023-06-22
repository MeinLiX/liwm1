import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/enviroments/environment';
import { LobbyUser, User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Car, BackendCar } from '../_models/car';
import { RacingTransmissionRange } from '../_models/racingTransmissionRange';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class RacingGameService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  private playerCarSource = new BehaviorSubject<Car | null>(null);
  playerCar$ = this.playerCarSource.asObservable();

  private carsSource = new BehaviorSubject<Car[]>([]);
  cars$ = this.carsSource.asObservable();

  public onCarRecieved?: (car: Car) => void;
  public onCarBoosted?: (car: Car) => void;
  public onRaceStarting?: () => void;
  public onRaceFinished?: (ratedPlayers: LobbyUser[]) => void;
  public onCarReadyStateUpdated?: (car: Car) => void;

  constructor(private toastr: ToastrService) { }

  connectToGame(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'racing-game', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('CarCreated', (receivedCars: BackendCar[], receivedCar: BackendCar) => {
      const car = new Car(0, 0, receivedCar.id, receivedCar.racerName);
      this.playerCarSource.next(car);

      console.log('before car creating');
      console.log(receivedCars);
      console.log(receivedCars.length);

      let cars = receivedCars.map(c => new Car(0, 0, c.id, c.racerName));
      cars = cars.filter(c => c.id !== car.id);
      cars.push(car);
      console.log(cars);
      this.carsSource.next(cars);
    });

    this.hubConnection.on('RecievedNewRacingCar', (receivedCar: BackendCar) => {
      const car = new Car(0, 0, receivedCar.id, receivedCar.racerName);
      this.carsSource.value.push(car);

      if (this.onCarRecieved) {
        this.onCarRecieved(car);
      }
    });

    this.hubConnection.on('CarHasBeenDeleted', () => {
      this.toastr.warning('You have left race');
    });

    this.hubConnection.on('OtherCarWithIdHasBeenDeleted', (carId: number) => {
      if (this.carsSource.value) {
        this.carsSource.next(this.carsSource.value.filter(c => c.id == carId));
      }
    });

    this.hubConnection.on('CarReadyStateUpdated', (receivedCar: BackendCar) => {
      const car = new Car(0, 0, receivedCar.id, receivedCar.racerName);
      this.toastr.success(car.racerName + ' is ' + (car.isReady ? 'ready' : 'is not ready'));
      if (this.carsSource.value) {
        const foundCar = this.carsSource.value.find(c => c.id == car.id);
        if (foundCar) {
          foundCar.isReady = car.isReady;

          if (this.onCarReadyStateUpdated) {
            this.onCarReadyStateUpdated(car);
          }
        }
      }
    });

    this.hubConnection.on('CarFinishedRacing', (receivedCar: BackendCar) => {
      const car = new Car(0, 0, receivedCar.id, receivedCar.racerName);
      this.toastr.success(car.racerName + ' has finished');
      if (this.carsSource.value) {
        const carFromArray = this.carsSource.value.find(c => c.id == car.id);
        if (carFromArray) {
          carFromArray.isFinished = car.isFinished;
        }
      }
    });

    this.hubConnection.on('CarBoosted', (receivedCar: BackendCar) => {
      const car = new Car(0, 0, receivedCar.id, receivedCar.racerName);
      if (this.onCarBoosted) {
        this.onCarBoosted(car);
      }
    });

    this.hubConnection.on('GameAlreadyStarted', () => {
      this.toastr.warning('Race already starter\nYour only option to watch');
    });

    this.hubConnection.on('GameStarting', () => {
      if (this.onRaceStarting) {
        this.onRaceStarting();
      }
    });

    this.hubConnection.on('FinishedSuccessfully', () => {
      this.toastr.success('You have finished');
    });

    this.hubConnection.on('GameFinished', (ratedPlayers: LobbyUser[]) => {
      if (this.onRaceFinished) {
        this.onRaceFinished(ratedPlayers);
      }
    });
  }

  updateCarReadyState(isReady: boolean) {
    this.playerCar$.pipe(take(1)).subscribe({
      next: async car => {
        if (car) {
          await this.hubConnection?.invoke('UpdateCarReadyStateAsync', car.id, isReady)
            .catch(error => console.log(error));
        }
      }
    });
  }

  boostCar(racingTransmissionRange: RacingTransmissionRange) {
    this.playerCar$.pipe(take(1)).subscribe({
      next: async car => {
        if (car) {
          await this.hubConnection?.invoke('BoostCarAsync', car.id, racingTransmissionRange)
            .catch(error => console.log(error));
        }
      }
    });
  }

  finishRacing() {
    this.playerCar$.pipe(take(1)).subscribe({
      next: async car => {
        if (car) {
          await this.hubConnection?.invoke('FinishRacingAsync', car.id)
            .catch(error => console.log(error));
        }
      }
    });
  }

  addCarForSoloGame(car: Car) {
    this.carsSource.next([car]);
    this.playerCarSource.next(car);
  }
}