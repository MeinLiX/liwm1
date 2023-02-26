import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './_modules/shared.module';
import { HomeComponent } from './_components/home/home.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './_interceptors/loading.interceptor';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextInputComponent } from './_components/forms/text-input/text-input.component';
import { GamesHomeComponent } from './_components/games-home/games-home.component';
import { NotFoundComponent } from './_components/not-found/not-found.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { PhotoChoosingComponent } from './_modals/photo-choosing/photo-choosing.component';
import { AccountComponent } from './_components/account/account.component';
import { ImageChangerComponent } from './_components/forms/image-changer/image-changer.component';
import { GameCardComponent } from './_components/cards/game-card/game-card.component';
import { LobbyComponent } from './_components/lobby/lobby.component';
import { LobbyConnectComponent } from './_modals/lobby-connect/lobby-connect.component';
import { LobbyUserCardComponent } from './_components/cards/user-card/lobbyUser-card.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    TextInputComponent,
    GamesHomeComponent,
    NotFoundComponent,
    PhotoChoosingComponent,
    AccountComponent,
    ImageChangerComponent,
    GameCardComponent,
    LobbyComponent,
    LobbyConnectComponent,
    LobbyUserCardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
