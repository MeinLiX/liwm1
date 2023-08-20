import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameMode } from '../_models/gameMode';
import { DataRestResponseResult } from '../_models/restResponseResult';

@Injectable({
  providedIn: 'root'
})
export class GameModesService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getGameModes() {
    return this.http.post<DataRestResponseResult<GameMode[]>>(this.baseUrl + 'games', {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }

  getGameModeByName(name: string) {
    return this.http.post<DataRestResponseResult<GameMode[]>>(this.baseUrl + 'games?name=' + name, {}).pipe(
      map(response => {
        return response.data?.at(0);
      })
    );
  }

  getGameModeById(id: number) {
    return this.http.post<DataRestResponseResult<GameMode>>(this.baseUrl + 'games?id=' + id, {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }

  getGameModesRange(start: number, count: number) {
    return this.http.post<DataRestResponseResult<GameMode[]>>(this.baseUrl + 'games?start=' + start + '&count=' + count, {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }
}