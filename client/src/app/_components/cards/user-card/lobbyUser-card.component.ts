import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { LobbyUser } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { LobbyService } from 'src/app/_services/lobby.service';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-lobbyUser-card',
  templateUrl: './lobbyUser-card.component.html',
  styleUrls: ['./lobbyUser-card.component.css']
})
export class LobbyUserCardComponent implements OnInit {
  @Input() lobbyUser?: LobbyUser;
  @Input() lobbyCreatorName?: string;
  currentUsername?: string;
  photo?: Photo;

  constructor(private photosService: PhotosService, private accountService: AccountService, private lobbyService: LobbyService) { }

  ngOnInit(): void {
    if (this.lobbyUser) {
      this.photosService.getPhotoById(this.lobbyUser.photoId).pipe(take(1)).subscribe({
        next: photo => {
          if (photo) this.photo = photo;
        }
      });
    }

    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.currentUsername = user?.username;
      }
    });
  }

  kickUser() {
    if (this.lobbyUser) {
      this.lobbyService.kickUser(this.lobbyUser.username);
    }
  }
}
