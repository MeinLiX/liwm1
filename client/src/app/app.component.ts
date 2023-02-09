import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  @HostListener('window: beforeunload', ['$event'])

  beforeUnloadHandler($event: any) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user && user.roles.includes('Anonymous')) {
          this.accountService.logout();
        }
      }
    });
  }

  constructor(public accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
    this.router.navigateByUrl('/games');
  }
}
