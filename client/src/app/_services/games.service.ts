import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/enviroments/environment';
import { GameMode } from '../_models/gameMode';
import { DataRestResponseResult } from '../_models/restResponseResult';

@Injectable({
  providedIn: 'root'
})
export class GamesService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getGames() {
    return this.http.post<DataRestResponseResult<GameMode[]>>(this.baseUrl + 'games', {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }

  getGameById(id: number) {
    return this.http.post<DataRestResponseResult<GameMode>>(this.baseUrl + 'games?id=' + id, {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }

  getGameRange(start: number, count: number) {
    return this.http.post<DataRestResponseResult<GameMode[]>>(this.baseUrl + 'games?start=' + start + '&count=' + count, {}).pipe(
      map(response => {
        return response.data;
      })
    );
  }
}