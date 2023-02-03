import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Photo } from 'src/app/_models/photo';
import { UserLogin, UserRegister } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  registerForm: FormGroup = new FormGroup({});
  photos?: Photo[];
  userLogin?: UserLogin;
  userRegister?: UserRegister;

  constructor(private accountService: AccountService, private fb: FormBuilder, private router: Router) {
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
        next: () => this.navigateToGamesHome()
      });
    }
  }

  login() {
    this.userLogin = { ...this.loginForm.value };
    if (this.userLogin) {
      this.accountService.login(this.userLogin).subscribe({
        next: () => this.navigateToGamesHome()
      });
    }
  }

  navigateToGamesHome() {
    this.router.navigateByUrl('/games');
  }
}