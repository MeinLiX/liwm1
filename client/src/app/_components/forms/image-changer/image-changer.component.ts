import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { take } from 'rxjs';
import { PhotoChoosingComponent } from 'src/app/_modals/photo-choosing/photo-choosing.component';
import { Photo } from 'src/app/_models/photo';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-image-changer',
  templateUrl: './image-changer.component.html',
  styleUrls: ['./image-changer.component.css']
})
export class ImageChangerComponent implements OnInit {
  @Output() updatePhoto = new EventEmitter<Photo>();
  @Input() choosenPhoto?: Photo;
  photos?: Photo[];
  photoChoosingModalRef: BsModalRef<PhotoChoosingComponent> = new BsModalRef<PhotoChoosingComponent>();

  constructor(private photosService: PhotosService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.photosService.getPhotos().pipe(take(1)).subscribe({
      next: photos => {
        if (photos) {
          this.photos = photos;
          if (!this.choosenPhoto) {
            this.getRandomPhoto();
          }
        }
      }
    });
  }

  getRandomPhoto() {
    if (this.photos) {
      let id: number = 0;
      do {
        id = this.randomIntFromInterval(1, this.photos.length);
      } while (id === this.choosenPhoto?.id);

      this.photosService.getPhotoById(id).pipe(take(1)).subscribe({
        next: photo => {
          if (photo) {
            this.choosenPhoto = photo;
            this.updatePhoto.emit(this.choosenPhoto);
          }
        }
      });
    }
  }

  randomIntFromInterval(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1) + min)
  }

  showPhotosModal() {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        photos: this.photos?.filter(p => p.id !== this.choosenPhoto?.id)
      }
    };
    this.photoChoosingModalRef = this.modalService.show(PhotoChoosingComponent, config);
    this.photoChoosingModalRef.onHide?.subscribe({
      next: () => {
        const choosenPhoto = this.photoChoosingModalRef.content?.result;
        if (choosenPhoto && choosenPhoto !== this.choosenPhoto) {
          this.choosenPhoto = choosenPhoto;
          this.updatePhoto.emit(this.choosenPhoto);
        }
      }
    });
  }
}
