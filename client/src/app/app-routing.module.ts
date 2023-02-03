import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GamesHomeComponent } from './_components/games-home/games-home.component';
import { HomeComponent } from './_components/home/home.component';
import { AuthGuard } from './_guards/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'games', component: GamesHomeComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
