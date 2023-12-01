import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { LobbyService } from './_services/lobby.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(public accountService: AccountService, public lobbyService: LobbyService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (userString) {
      const user: User = JSON.parse(userString);
      const decodedToken = JSON.parse(atob(user.token.split('.')[1]));
      if (Date.now() <= decodedToken.exp * 1000) {
        this.accountService.setCurrentUser(user);
        this.router.navigateByUrl('/games');
      }
      else {
        this.router.navigateByUrl('/');
        localStorage.removeItem('user');
        this.toastr.warning('Your session has expired. Regain access')
      }
    }
  }
}