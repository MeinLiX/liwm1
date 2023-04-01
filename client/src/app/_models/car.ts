export class Car {
    x: number;
    y: number;
    dx: number;
    dy: number;
    lap: number;

    constructor(x: number, y: number) {
        this.x = x;
        this.y = y;
        this.dx = 0;
        this.dy = 0;
        this.lap = 1;
    }
}