import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TabsModule.forRoot(),
    NgxSpinnerModule.forRoot({
      type: 'ball-climbing-dot'
    })
  ],
  exports: [
    NgxSpinnerModule,
    TabsModule
  ]
})
export class SharedModule { }
