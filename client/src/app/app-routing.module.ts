import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GamesHomeComponent } from './_components/games-home/games-home.component';
import { RacingComponent } from './_components/games/racing/racing.component';
import { HomeComponent } from './_components/home/home.component';
import { NotFoundComponent } from './_components/not-found/not-found.component';
import { AuthGuard } from './_guards/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'games', component: GamesHomeComponent },
      { path: 'racing', component: RacingComponent }
    ]
  },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
