import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GamesHomeComponent } from './_components/games-home/games-home.component';
import { RacingComponent } from './_components/games/racing/racing.component';
import { HomeComponent } from './_components/home/home.component';
import { NotFoundComponent } from './_components/not-found/not-found.component';
import { AuthGuard } from './_guards/auth.guard';
import { GameGuard } from './_guards/game.guard';
import { WordsBattleComponent } from './_components/games/words-battle/words-battle.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'games', component: GamesHomeComponent },
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
