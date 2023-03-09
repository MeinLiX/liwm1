import { Component, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-racing',
  templateUrl: './racing.component.html',
  styleUrls: ['./racing.component.css']
})
export class RacingComponent {
  @ViewChild('canvas') canvas: any;
  @ViewChild('speed') speed: any;

}
