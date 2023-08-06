import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './_components/home/home.component';
import { RacingComponent } from './_components/games/racing/racing.component';
import { LoginComponent } from './_components/login/login.component';
import { NotFoundComponent } from './_components/not-found/not-found.component';
import { AuthGuard } from './_guards/auth.guard';
import { GameGuard } from './_guards/game.guard';
import { WordsBattleComponent } from './_components/games/words-battle/words-battle.component';

const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      //TODO: Probably find the way to make this two routes in one line
      { path: 'games', component: HomeComponent },
      { path: 'home', component: HomeComponent },
      { path: 'racing', component: RacingComponent },
      { path: 'words-battle', component: WordsBattleComponent },
      { path: 'racing?isPractise=false', component: RacingComponent, canActivate: [GameGuard] },
      { path: 'words-battle?isPractise=false', component: WordsBattleComponent, canActivate: [GameGuard] }
    ]
  },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
