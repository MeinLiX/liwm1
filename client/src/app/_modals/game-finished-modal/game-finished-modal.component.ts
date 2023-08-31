import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { GameMode } from 'src/app/_models/gameMode';
import { LobbyUser } from 'src/app/_models/user';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-game-finished-modal',
  templateUrl: './game-finished-modal.component.html',
  styleUrls: ['./game-finished-modal.component.css']
})
export class GameFinishedModalComponent implements OnInit {
  players?: LobbyUser[];
  gameMode?: GameMode;

  constructor(public modalRef: BsModalRef, private photosService: PhotosService) { }

  ngOnInit(): void {
    console.log(this.gameMode);
    if (this.players) {
      for (let i = 0; i < this.players.length; i++) {
        this.photosService.getPhotoById(this.players[i].photoId).subscribe({
          next: photo => {
            if (this.players && photo) {
              this.players[i].photoUrl = photo.url;
            }
          }
        });
      }
    }
  }
}