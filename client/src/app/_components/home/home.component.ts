import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Photo } from 'src/app/_models/photo';
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

  constructor(private accountService: AccountService, private fb: FormBuilder) {
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
    console.log('register');
  }

  login() {
    console.log('login');
  }

}