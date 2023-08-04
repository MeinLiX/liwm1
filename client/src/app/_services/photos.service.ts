import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/photo';
import { DataRestResponseResult } from '../_models/restResponseResult';

@Injectable({
  providedIn: 'root'
})
export class PhotosService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPhotoById(id: number) {
    return this.http.post<DataRestResponseResult<Photo[]>>(this.baseUrl + 'photos?id=' + id, {}).pipe(
      map(response => {
        let photo: Photo | null = null;
        if (response.data) {
          photo = response.data[0];
        }
        return photo;
      })
    );
  }

  getPhotosRange(start: number, count: number) {
    return this.http.post<DataRestResponseResult<Photo[]>>(this.baseUrl + 'photos?start=' + start + '&count=' + count, {}).pipe(
      map(response => this.photoReturn(response.data))
    );
  }

  getPhotos() {
    return this.http.post<DataRestResponseResult<Photo[]>>(this.baseUrl + 'photos', {}).pipe(
      map(response => this.photoReturn(response.data))
    );
  }

  private photoReturn(data: Photo[] | undefined) {
    if (data) {
      return data;
    }
    return null;
  }
}
