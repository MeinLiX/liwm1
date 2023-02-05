import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TabDirective } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { UserLogin, UserRegister } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  registerForm: FormGroup = new FormGroup({});
  photos?: Photo[];
  choosenPhoto?: Photo;
  userLogin?: UserLogin;
  userRegister?: UserRegister;

  constructor(private accountService: AccountService, private photoService: PhotosService, private fb: FormBuilder, private router: Router, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.initializeForms();
  }

  initializeForms() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern('(.*[a-z].*)'), Validators.pattern('(.*[A-Z].*)'), Validators.pattern('(.*\\d.*)'),]],
    });
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern('(.*[a-z].*)'), Validators.pattern('(.*[A-Z].*)'), Validators.pattern('(.*\\d.*)'),]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    };
  }

  register() {
    this.userRegister = { ...this.registerForm.value, photoId: 1 };
    if (this.userRegister) {
      this.accountService.register(this.userRegister).subscribe({
        next: () => this.navigateToGamesHome(),
        error: error => this.toastr.error(error.message)
      });
    }
  }

  login() {
    this.userLogin = { ...this.loginForm.value };
    if (this.userLogin) {
      this.accountService.login(this.userLogin).subscribe({
        next: () => this.navigateToGamesHome(),
        error: error => this.toastr.error(error.message)
      });
    }
  }

  navigateToGamesHome() {
    this.router.navigateByUrl('/games');
  }

  onSelect(data: TabDirective): void {
    if (data.heading === 'Sign Up') {
      if (this.photos) {
        this.getRandomPhotoIfUndefined();
      }
      else {
        this.photoService.getPhotos().pipe(take(1)).subscribe({
          next: photos => {
            if (photos) {
              this.photos = photos;
              this.getRandomPhotoIfUndefined();
            }
          }
        });
      }
    }
  }

  getRandomPhotoIfUndefined() {
    if (!this.choosenPhoto) {
      this.getRandomPhoto();
    }
  }

  getRandomPhoto() {
    if (this.photos) {
      let id: number = 0;
      do {
        id = this.randomIntFromInterval(1, this.photos.length);
      } while (id === this.choosenPhoto?.photoId);

      this.photoService.getPhotoById(id).pipe(take(1)).subscribe({
        next: photo => {
          if (photo) {
            this.choosenPhoto = photo;
          }
        }
      });
    }
  }

  randomIntFromInterval(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1) + min)
  }
}