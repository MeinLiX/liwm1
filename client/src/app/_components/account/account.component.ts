import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  user?: User;
  photo?: Photo;
  isDetailedFormVisible: boolean = false;

  constructor(private accountService: AccountService, private photosService: PhotosService, private router: Router) { }

  ngOnInit(): void {
    this.getUserAndPhoto();
  }

  getUserAndPhoto() {
    this.accountService.currentUser$.subscribe({
      next: user => {
        if (user) {
          this.user = user;
          this.photosService.getPhotoById(user.photoId).pipe(take(1)).subscribe({
            next: photo => {
              if (photo) {
                this.photo = photo;
              }
            }
          });
        }
      }
    });
  }

  updatePhoto(photo: Photo) {
    if (photo && this.photo?.id == photo.id) {
      this.photo = photo;
      //TODO: Add photo updating in backend
    }
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
    this.isDetailedFormVisible = false;
  }
}