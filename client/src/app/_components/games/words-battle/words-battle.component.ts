import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';

@Component({
  selector: 'app-words-battle',
  templateUrl: './words-battle.component.html',
  styleUrls: ['./words-battle.component.css']
})
export class WordsBattleComponent implements OnInit {

  private canvas?: HTMLCanvasElement;
  private ctx?: CanvasRenderingContext2D | null;

  private isPractise = false;

  private words: string[] | null = null;
  private selectedWord: string = '';

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  onClick() {
    this.canvas = document.getElementById("canvas") as HTMLCanvasElement;
    if (this.canvas) {
      this.ctx = this.canvas.getContext('2d');
    }

    this.route.queryParams.pipe(take(1)).subscribe({
      next: params => {
        this.isPractise = params['isPractise'] === 'true';
      }
    });
  }

  @HostListener('document:keypress', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    console.log(event.key);
    //TODO: Add changing selected word and deleting letters from already selected
  }
}
