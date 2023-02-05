import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-choosing',
  templateUrl: './photo-choosing.component.html',
  styleUrls: ['./photo-choosing.component.css']
})
export class PhotoChoosingComponent {
  photos?: Photo[];
  result?: Photo;

  constructor(public modalRef: BsModalRef) { }

  choosePhoto(photo: Photo) {
    this.result = photo;
    this.modalRef.hide();
  }
}
