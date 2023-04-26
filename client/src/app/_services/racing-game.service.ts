import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/enviroments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Car } from '../_models/car';
import { RacingTransmissionRange } from '../_models/racingTransmissionRange';

@Injectable({
  providedIn: 'root'
})
export class RacingGameService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  private playerCarSource = new BehaviorSubject<Car | null>(null);
  playerCar$ = this.playerCarSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  connectToGame(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'racing-game', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));
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
}
