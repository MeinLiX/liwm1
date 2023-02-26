import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { LobbyUser } from 'src/app/_models/user';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-lobbyUser-card',
  templateUrl: './lobbyUser-card.component.html',
  styleUrls: ['./lobbyUser-card.component.css']
})
export class LobbyUserCardComponent implements OnInit {
  @Input() lobbyUser?: LobbyUser;
  photo?: Photo;

  constructor(private photosService: PhotosService) { }

  ngOnInit(): void {
    if (this.lobbyUser) {
      this.photosService.getPhotoById(this.lobbyUser.photoId).pipe(take(1)).subscribe({
        next: photo => {
          if (photo) this.photo = photo;
        }
      });
    }
  }

  kickUser() {
    
  }
}
