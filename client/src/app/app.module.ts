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

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    TextInputComponent,
    GamesHomeComponent,
    NotFoundComponent
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
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
