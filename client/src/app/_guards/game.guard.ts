import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map, take } from 'rxjs';
import { LobbyService } from '../_services/lobby.service';

@Injectable({
  providedIn: 'root'
})
export class GameGuard implements CanActivate {

  constructor(private lobbyService: LobbyService) { }

  canActivate(): Observable<boolean> {
    return this.lobbyService.lobby$.pipe(
      map(lobby => {
        let result = false;

        if (lobby) {
          result = lobby.users.length > 0;
        }

        return result;
      })
    );
  }
}